using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerGames.FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics;
using TrashBash;
using Microsoft.Xna.Framework.Content;
using FarseerGames.FarseerPhysics.Factories;

namespace TrashBash.Objects
{
    class Trash
    {
        private Body trashBody;
        private Vector2 trashOrigin;
        private Texture2D trashTexture;
        private Geom trashGeom;
        private CollisionCategory collidesWith = CollisionCategory.All;
        private CollisionCategory collisionCategory = CollisionCategory.All;
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

        public Geom Geom
        {
            get { return this.trashGeom; }
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

        public void ApplyForce(Vector2 force)
        {
            trashBody.ApplyForce(force);
        }

        public void ApplyTorque(float torque)
        {
            trashBody.ApplyTorque(torque);
        }

        public void Load(GraphicsDevice device, ContentManager content, PhysicsSimulator simulator, string name, int mass)
        {
            trashTexture = content.Load<Texture2D>("Content/Objects/" + name);
            uint[] data = new uint[trashTexture.Width * trashTexture.Height];
            trashTexture.GetData(data);

            Vertices verts = Vertices.CreatePolygon(data, trashTexture.Width, trashTexture.Height);
            trashOrigin = verts.GetCentroid();
            verts.SubDivideEdges(5);

            trashBody = BodyFactory.Instance.CreatePolygonBody(verts, mass);
            trashBody.Position = position;
            trashBody.IsAutoIdle = true;
            trashBody.MinimumVelocity = 25;

            trashGeom = GeomFactory.Instance.CreatePolygonGeom(trashBody, verts, 0);

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
