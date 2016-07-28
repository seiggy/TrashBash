using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SpriteSheetRuntime;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics;
using TrashBash.ScreenSystem;
using FarseerGames.FarseerPhysics.Factories;

namespace TrashBash.Objects.Weapons
{
    public class ConcussionGrenade
    {
        Texture2D texture;
        SpriteSheet animation;
        Body shotBody;
        Geom shotGeom;
        Vector2 shotOrigin;
        Vector2 blastOrigin;
        bool fired;
        bool exploded;
        int activeTimer;
        Vector2 blastPosition;
        int timer = 0;

        PhysicsSimulator simulator;

        public Body Body
        {
            get { return this.shotBody; }
        }

        public Geom Geom
        {
            get { return this.shotGeom; }
        }

        public void Reset()
        {
            fired = false;
            exploded = false;
            activeTimer = 0;
            timer = 0;
            simulator.Remove(Geom);
            simulator.Remove(Body);
        }

        public void LoadContent(ScreenManager screenManager)
        {
            // load animation here

            // set blast origin as well

            texture = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/conc_shot");
            uint[] data = new uint[texture.Width * texture.Height];
            texture.GetData(data);

            Vertices verts = Vertices.CreatePolygon(data, texture.Width, texture.Height);
            shotOrigin = verts.GetCentroid();
            verts.SubDivideEdges(5);

            shotBody = BodyFactory.Instance.CreatePolygonBody(verts, 1.0f);

            shotGeom = GeomFactory.Instance.CreatePolygonGeom(shotBody, verts, 0);
            shotGeom.CollisionGroup = 0;
            shotGeom.CollidesWith = CollisionCategory.All;
            shotGeom.CollisionCategories = CollisionCategory.All;
            shotGeom.RestitutionCoefficient = 1.0f;

            fired = false;
            exploded = false;
            activeTimer = 0;
        }

        public bool Fire(Ship player, PhysicsSimulator simulator)
        {
            this.simulator = simulator;
            simulator.Add(Body);
            simulator.Add(Geom);
            if (!fired)
            {
                Vector2 dir = new Vector2(10000 * (float)(Math.Cos(player.Body.Rotation -
                    (MathHelper.PiOver2))), 10000 * (float)Math.Sin(player.Body.Rotation -
                    (MathHelper.PiOver2)));
                Vector2 pos = new Vector2(player.Body.Position.X +
                    ((float)(Math.Cos(player.Body.Rotation - MathHelper.PiOver2)) * 70),
                    player.Body.Position.Y + ((float)Math.Sin(player.Body.Rotation -
                    (MathHelper.PiOver2))) * 70);
                shotBody.Position = pos;
                shotBody.ApplyForce(dir);
                fired = true;
                return true;
            }
            return false;
        }

        public bool Update(GameTime gameTime)
        {
            if (!exploded)
            {
                timer += gameTime.ElapsedGameTime.Milliseconds;
                if (timer > 3000)
                {
                    exploded = true;
                    return false;
                }
            }
            if (exploded)
            {
                blastPosition = Body.Position;
                activeTimer += 1;
                Vector2 min = Vector2.Subtract(blastPosition, new Vector2(300, 300));
                Vector2 max = Vector2.Add(blastPosition, new Vector2(300, 300));

                AABB aabb = new AABB(min, max);

                foreach (Body body in simulator.BodyList)
                {
                    if (aabb.Contains(body.Position))
                    {
                        Vector2 fv = body.Position;
                        fv = Vector2.Subtract(fv, blastPosition);
                        fv.Normalize();
                        fv = Vector2.Multiply(fv, 50000);
                        body.ApplyForce(fv);
                    }
                }
                if(activeTimer > 2)
                    return true;
            }
            return false;
        }

        public void Draw(ScreenManager screenManager)
        {
            if (fired && !exploded)
            {
                screenManager.SpriteBatch.Draw(texture, shotBody.Position, null,
                    Color.White, shotBody.Rotation, shotOrigin, 1, SpriteEffects.None, 0);
            }
            else
            {
                if (exploded)
                {
                    //screenManager.SpriteBatch.Draw(animation.Texture, blastPosition,
                        //animation.SourceRectangle(activeTimer), Color.White, 0.0f,
                        //holeOrigin,
                        //2f, SpriteEffects.None, 0);
                }
            }
        }
    }
}
