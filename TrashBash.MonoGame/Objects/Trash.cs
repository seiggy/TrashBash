using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashBash.MonoGame.Objects
{
    class Trash
    {
        private Body trashBody;
        private Vector2 trashOrigin;
        private Texture2D trashTexture;
        private Shape trashGeom;
        private Category collidesWith = Category.All;
        private Category collisionCategory = Category.All;
        private Vector2 position;
        private uint scoreValue;

        public Trash(Vector2 position)
        {
            this.position = position;
        }

        public Trash()
        {
            this.position = Vector2.Zero;
        }

        public uint ScoreValue
        {
            get { return this.scoreValue; }
            set { this.scoreValue = value; }
        }

        public Body Body
        {
            get { return this.trashBody; }
        }

        public Category CollisionCategory
        {
            get { return this.collisionCategory; }
            set { this.collisionCategory = value; }
        }

        public Category CollidesWith
        {
            get { return this.collidesWith; }
            set { this.collidesWith = value; }
        }

        public Shape Geom
        {
            get { return this.trashGeom; }
        }
        
        public void ApplyForce(Vector2 force)
        {
            trashBody.ApplyForce(force);
        }

        public void ApplyTorque(float torque)
        {
            trashBody.ApplyTorque(torque);
        }

        public void Load(GraphicsDevice device, ContentManager content, World simulator, string name, int mass)
        {
            trashTexture = content.Load<Texture2D>("Content/Objects/" + name);
            uint[] data = new uint[trashTexture.Width * trashTexture.Height];
            trashTexture.GetData(data);
            
            Vertices verts = PolygonTools.CreatePolygon(data, trashTexture.Width, false);
            trashOrigin = verts.GetCentroid();
            
            trashBody = BodyFactory.CreatePolygon(simulator, verts, mass);
            trashBody.Position = position;

            var trashGeom = new PolygonShape(verts, mass);
            trashGeom.RestitutionCoefficient = 0.4f;
            trashGeom.FrictionCoefficient = 0.2f;
            trashGeom.CollisionGroup = 0;
            trashGeom.CollisionCategories = collisionCategory;
            trashGeom.CollidesWith = collidesWith;
            trashGeom.Name = "trash";
            trashGeom.Tag = this;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(trashTexture, trashBody.Position, null, Color.White,
                trashBody.Rotation, trashOrigin, 1, SpriteEffects.None, 0);
        }
    }
}
