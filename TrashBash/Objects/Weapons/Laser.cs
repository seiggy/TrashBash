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

namespace TrashBash.Objects.Weapons
{
    class Laser
    {
        private Body bulletBody;
        private Vector2 bulletOrigin;
        private Texture2D bulletTexture;
        private Geom bulletGeom;
        private CollisionCategory collidesWith = CollisionCategory.All;
        private CollisionCategory collisionCategory = CollisionCategory.All;
        private Vector2 position;

        public bool isAvailable;

        public Laser()
        {

        }

        public Body BulletBody
        {
            get { return this.bulletBody; }
            set { this.bulletBody = value; }
        }

        public Geom BulletGeom
        {
            get { return this.bulletGeom; }
            set { this.bulletGeom = value; }
        }

        public CollisionCategory CollidesWith
        {
            get { return this.collidesWith; }
            set { this.collidesWith = value; }
        }

        public CollisionCategory CollisionCategory
        {
            get { return this.collisionCategory; }
            set { this.collisionCategory = value; }
        }

        public void Load(ScreenManager screenManager, PhysicsSimulator simulator)
        {
            bulletTexture = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/blobShot");
            uint[] data = new uint[bulletTexture.Width * bulletTexture.Height];
            bulletTexture.GetData(data);

            Vertices verts = Vertices.CreatePolygon(data, bulletTexture.Width, bulletTexture.Height);
            bulletOrigin = verts.GetCentroid();
            verts.SubDivideEdges(5);

            bulletBody = BodyFactory.Instance.CreatePolygonBody(verts, 1.0f);
            bulletBody.IgnoreGravity = true;

            bulletGeom = GeomFactory.Instance.CreatePolygonGeom(bulletBody, verts, 0);
            bulletGeom.CollisionGroup = 0;
            bulletGeom.CollidesWith = collidesWith;
            bulletGeom.CollisionCategories = collisionCategory;
        }

        public void Fire(Vector2 position, Vector2 direction)
        {
            this.position = position;
            bulletBody.Position = position;
            bulletBody.ApplyForce(direction);
            isAvailable = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulletTexture, BulletBody.Position, null, Color.White,
                BulletBody.Rotation, bulletOrigin, 1, SpriteEffects.None, 0);
        }
    }
}
