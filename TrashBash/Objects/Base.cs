using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics;
using TrashBash.ScreenSystem;
using FarseerGames.FarseerPhysics.Factories;

namespace TrashBash.Objects
{
    class Base
    {
        private Body baseBody;
        private Geom baseGeom;
        private Geom baseScoreGeom;
        private Vector2 baseOrigin;
        private Texture2D baseTexture;
        private Texture2D collisionTexture;
        private CollisionCategory collidesWith = CollisionCategory.All;
        private CollisionCategory collisionCategory = CollisionCategory.All;
        private Vector2 position;
        private Ship player;

        public Base(Vector2 position, Ship player)
        {
            this.position = position;
            this.player = player;
        }

        public Body Body
        {
            get { return this.baseBody; }
            set { this.baseBody = value; }
        }

        public Geom Geom
        {
            get { return this.baseGeom; }
            set { this.baseGeom = value; }
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

        public Texture2D Texture
        {
            get { return this.baseTexture; }
            set { this.baseTexture = value; }
        }

        public Vector2 Position
        {
            get { return this.position; }
        }

        public Vector2 Origin
        {
            get { return this.baseOrigin; }
        }

        public void Load(ScreenManager screenManager, PhysicsSimulator physicsSimulator, string name)
        {
            baseTexture = screenManager.ContentManager.Load<Texture2D>("Content/Objects/" + name);
            collisionTexture = screenManager.ContentManager.Load<Texture2D>("Content/Objects/trashBin_collision");
            uint[] data = new uint[baseTexture.Width * baseTexture.Height];
            collisionTexture.GetData(data);

            Vertices verts = Vertices.CreatePolygon(data, baseTexture.Width, baseTexture.Height);
            baseOrigin = verts.GetCentroid();
            verts.SubDivideEdges(15);

            baseBody = BodyFactory.Instance.CreatePolygonBody(physicsSimulator, verts, 40f);
            baseBody.IsStatic = true;
            baseBody.Position = position;

            baseGeom = GeomFactory.Instance.CreatePolygonGeom(physicsSimulator, baseBody, verts, 0);
            baseGeom.CollisionGroup = 0;
            baseGeom.CollidesWith = collidesWith;
            baseGeom.CollisionCategories = collisionCategory;
            // baseGeom.OnCollision += new Geom.CollisionEventHandler(OnCollide);

            baseScoreGeom = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, baseBody, 160, 30, new Vector2(0, -24), 0f);
            baseScoreGeom.CollisionGroup = 1;
            baseScoreGeom.CollidesWith = collidesWith;
            baseScoreGeom.CollisionCategories = collisionCategory;
            baseScoreGeom.CollisionResponseEnabled = false;
            baseScoreGeom.OnCollision += new Geom.CollisionEventHandler(OnCollide);
        }

        public bool OnCollide(Geom g1, Geom g2, ContactList contactList)
        {
            if (g1.Name == "trash")
            {
                this.player.Score += ((Trash)g1.Tag).ScoreValue;
                g1.Name = "reset";
                if (player.TractorActive)
                {
                    player.TractorActive = false;
                    player.TractorBeam.Enabled = false;
                }
            }
            else if (g2.Name == "trash")
            {
                this.player.Score += ((Trash)g2.Tag).ScoreValue;
                g2.Name = "reset";
                if (player.TractorActive)
                {
                    player.TractorActive = false;
                    player.TractorBeam.Enabled = false;
                }
            }
            if ((g2.Name == player.Geom.Name || g1.Name == player.Geom.Name) && player.Fuel < 100)
            {
                player.Fuel += 1;
            }

            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(baseTexture, position, null, Color.White,
                baseBody.Rotation, baseOrigin, 1, SpriteEffects.None, 0);
        }
    }
}
