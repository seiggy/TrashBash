using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using TrashBash.Objects.Weapons;
using Microsoft.Xna.Framework;
using TrashBash.ScreenSystem;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics;

namespace TrashBash.Objects
{
    class PowerUp
    {
        Texture2D texture;
        Body body;
        Geom geom;
        WeaponList weapon;
        Vector2 position;
        Vector2 origin;
        private bool enabled = true;
        private int time = 0;

        public PowerUp()
        {
        }

        public Body Body
        {
            get { return body; }
            set { this.body = value; }
        }

        public Geom Geom
        {
            get { return geom; }
            set { geom = value; }
        }

        public WeaponList Weapon
        {
            get { return weapon; }
            set { this.weapon = value; }
        }

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public void LoadContent(ScreenManager screenManager, WeaponList weapon)
        {
            this.weapon = weapon;
            switch (weapon)
            {
                case WeaponList.None:
                    return;
                case WeaponList.ConcussionGrenade:
                    texture = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/Icons/revRay");
                    break;
                case WeaponList.EMPWeapon:
                    texture = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/Icons/empGrenade");
                    break;
                case WeaponList.BlackHole:
                    texture = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/Icons/blackHole");
                    break;
                default:
                    break;
            }
            uint[] data = new uint[texture.Width * texture.Height];
            texture.GetData(data);

            Vertices verts = Vertices.CreatePolygon(data, texture.Width, texture.Height);
            origin = verts.GetCentroid();
            verts.SubDivideEdges(10);

            body = BodyFactory.Instance.CreatePolygonBody(verts, 1.0f);
            body.IgnoreGravity = true;
            body.IsStatic = true;

            geom = GeomFactory.Instance.CreatePolygonGeom(body, verts, 0);
            geom.Name = "powerup";
            geom.CollisionGroup = 0;
            geom.CollisionCategories = CollisionCategory.All;
            geom.CollidesWith = CollisionCategory.All;
            geom.CollisionResponseEnabled = false; // causing Collision event to not fire?
            geom.OnCollision += new Geom.CollisionEventHandler(OnCollide);
        }

        public bool OnCollide(Geom g1, Geom g2, ContactList clist)
        {
            if (enabled)
            {
                if (g1.Name == "player1" || g1.Name == "player2")
                {
                    if (((Ship)g1.Tag).WeaponB == WeaponList.None)
                    {
                        ((Ship)g1.Tag).WeaponB = this.weapon;
                    }
                    else if (((Ship)g1.Tag).WeaponC == WeaponList.None)
                    {
                        ((Ship)g1.Tag).WeaponC = this.weapon;
                    }
                    else
                    {
                        if (((Ship)g1.Tag).WeaponBFired == false)
                        {
                            ((Ship)g1.Tag).WeaponB = this.weapon;
                        }
                        else if (((Ship)g1.Tag).WeaponCFired == false)
                        {
                            ((Ship)g1.Tag).WeaponC = this.weapon;
                        }
                    }
                    enabled = false;
                }
                if (g2.Name == "player1" || g2.Name == "player2")
                {
                    if (((Ship)g2.Tag).WeaponB == WeaponList.None)
                    {
                        ((Ship)g2.Tag).WeaponB = this.weapon;
                    }
                    else if (((Ship)g2.Tag).WeaponC == WeaponList.None)
                    {
                        ((Ship)g2.Tag).WeaponC = this.weapon;
                    }
                    else
                    {
                        if (((Ship)g2.Tag).WeaponBFired == false)
                        {
                            ((Ship)g2.Tag).WeaponB = this.weapon;
                        }
                        else if (((Ship)g2.Tag).WeaponCFired == false)
                        {
                            ((Ship)g2.Tag).WeaponC = this.weapon;
                        }
                    }
                    enabled = false;
                }
            }
            return false;
        }

        public void Update(GameTime gameTime)
        {
            if (body.Position != position)
            {
                body.Position = position;
            }
            if (!enabled)
            {
                time += gameTime.ElapsedGameTime.Milliseconds;
                if (time > 5000)
                {
                    enabled = true;
                    time = 0;
                }
            }
        }

        public void Draw(ScreenManager screenManager)
        {
            if (enabled)
            {
                screenManager.SpriteBatch.Draw(texture, position, null, Color.White,
                    0.0f, origin, 1, SpriteEffects.None, 0);
            }
        }
    }
}
