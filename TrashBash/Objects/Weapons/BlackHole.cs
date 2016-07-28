using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpriteSheetRuntime;
using TrashBash.ScreenSystem;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics;

namespace TrashBash.Objects.Weapons
{
    public class BlackHole
    {
        Texture2D texture;
        SpriteSheet animation;
        Body shotBody;
        Geom shotGeom;
        Vector2 shotOrigin;
        Vector2 holeOrigin;
        bool fired;
        bool exploded;
        int activeTimer;
        Vector2 blackHolePosition = Vector2.Zero;
        int timer = 0;

        PhysicsSimulator simulator;

        public Body Body
        {
            get { return shotBody; }
        }

        public Geom Geom
        {
            get { return shotGeom; }
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
            animation = screenManager.ContentManager.Load<SpriteSheet>("Content/Weapons/BlackHoleEffect/BlackHole");

            holeOrigin = new Vector2(animation.SourceRectangle(0).Width / 2, animation.SourceRectangle(0).Height / 2);
            texture = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/blackHole_shot");
            uint[] data = new uint[texture.Width * texture.Height];
            texture.GetData(data);

            Vertices verts = Vertices.CreatePolygon(data, texture.Width, texture.Height);
            shotOrigin = verts.GetCentroid();
            verts.SubDivideEdges(5);

            shotBody = BodyFactory.Instance.CreatePolygonBody(verts, 1.0f);
            shotBody.IgnoreGravity = true;

            shotGeom = GeomFactory.Instance.CreatePolygonGeom(shotBody, verts, 0);
            shotGeom.CollisionGroup = 0;
            shotGeom.CollidesWith = CollisionCategory.All;
            shotGeom.CollisionCategories = CollisionCategory.All;
            shotGeom.RestitutionCoefficient = 1.0f;

            fired = false;
            exploded = false;
            activeTimer = 0;
        }

        /// <summary>
        /// returns true if it's time to remove the weapon from inventory
        /// (first time we fire the weapon, second time we explode it...yay!)
        /// </summary>
        /// <returns></returns>
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
                return false;
            }
            else
            {
                exploded = true;
                blackHolePosition = new Vector2(shotBody.Position.X, shotBody.Position.Y);
                return true;
            }
        }

        /// <summary>
        /// returns true if it's finished it's animation
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public bool Update(GameTime gameTime)
        {
            if (exploded)
            {
                timer += gameTime.ElapsedGameTime.Milliseconds;
                if (timer / 50 > 1)
                {
                    activeTimer++;
                    timer -= 50;
                }
                if (activeTimer == 100)
                {
                    exploded = false;
                    fired = false;
                    return true;
                }
                Vector2 min = Vector2.Subtract(blackHolePosition, new Vector2(600, 600));
                Vector2 max = Vector2.Add(blackHolePosition, new Vector2(600, 600));

                AABB aabb = new AABB(min, max);

                foreach (Body body in simulator.BodyList)
                {
                    if (aabb.Contains(body.Position))
                    {
                        Vector2 fv = body.Position;
                        fv = Vector2.Subtract(fv, blackHolePosition);
                        fv.Normalize();
                        fv = Vector2.Multiply(fv, -3000);
                        body.ApplyForce(fv);
                    }
                }
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
                    screenManager.SpriteBatch.Draw(animation.Texture, blackHolePosition,
                        animation.SourceRectangle(activeTimer), Color.White, 0.0f,
                        holeOrigin,
                        2f, SpriteEffects.None, 0);
                }
            }
        }
    }
}
