using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerGames.FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics;
using TrashBash.ScreenSystem;
using FarseerGames.FarseerPhysics.Factories;

namespace TrashBash.Objects
{
    class Land
    {
        private Body landBody;
        private Vector2 landOrigin;
        private Texture2D landTexture;
        private Geom landGeom;
        private CollisionCategory collidesWith = CollisionCategory.All;
        private CollisionCategory collisionCategory = CollisionCategory.All;
        private Vector2 position;

        public Land(Vector2 position)
        {
            this.position = position;
        }

        public Body Body
        {
            get { return this.landBody; }
        }

        public CollisionCategory CollisionCategory
        {
            get { return this.collisionCategory; }
            set { this.collisionCategory = value; }
        }

        public CollisionCategory CollidesWith
        {
            get { return this.collidesWith; }
            set { this.collidesWith = value; }
        }

        public Texture2D LandTexture
        {
            get { return this.landTexture; }
            set { this.landTexture = value; }
        }

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public Vector2 Origin
        {
            get { return this.landOrigin; }
            set { this.landOrigin = value; }
        }


        public void Load(ScreenManager screenManager, PhysicsSimulator simulator, string name)
        {
            try
            {
                landTexture = screenManager.ContentManager.Load<Texture2D>("Content/Land/Collision/" + name);
            }
            catch (Exception e)
            {
                landTexture = screenManager.ContentManager.Load<Texture2D>("Content/Land/blank");
            }
            finally
            {
                uint[] data = new uint[landTexture.Width * landTexture.Height];
                landTexture.GetData(data);

                Vertices verts = Vertices.CreatePolygon(data, landTexture.Width, landTexture.Height);
                if (verts.Count > 1)
                {
                    landOrigin = verts.GetCentroid();
                    verts.SubDivideEdges(10);

                    landBody = BodyFactory.Instance.CreateRectangleBody(simulator, LandTexture.Width, LandTexture.Height, 1.0f);
                    landBody.IsStatic = true;
                    landBody.Position = position;

                    landGeom = GeomFactory.Instance.CreatePolygonGeom(simulator, landBody, verts, 0);
                    landGeom.CollisionGroup = 1;
                    landGeom.CollidesWith = collidesWith;
                    landGeom.CollisionCategories = collisionCategory;
                }
            }
        }

        public void UpdatePos()
        {
            // position = new Vector2(position.X - landOrigin.X, position.Y - landOrigin.Y);
            if (landBody != null)
            {
                landBody.Position = position;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (landBody != null)
            {
                spriteBatch.Draw(landTexture, landBody.Position, null, Color.White,
                    landBody.Rotation, landOrigin, 1, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(landTexture, position, null, Color.White,
                    0.0f, landOrigin, 1, SpriteEffects.None, 0);
            }
        }
    }
}
