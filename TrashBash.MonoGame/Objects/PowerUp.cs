using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrashBash.MonoGame.Objects.Weapons;
using TrashBash.MonoGame.ScreenSystem;

namespace TrashBash.MonoGame.Objects
{
    class PowerUp
    {
        Texture2D texture;
        Body body;
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

        public void LoadContent(ScreenManager screenManager, WeaponList weapon, World world)
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

            Vertices verts = PolygonTools.CreatePolygon(data, texture.Width);
            origin = verts.GetCentroid();
            //verts.SubDivideEdges(10);

            body = BodyFactory.CreatePolygon(world, verts, 1.0f);
            body.IgnoreGravity = true;
            body.IsStatic = true;

            body.CollisionGroup = 0;
            body.CollisionCategories = Category.All;
            body.CollidesWith = Category.All;
            body.OnCollision += OnCollide;
        }
        
        public bool OnCollide(Fixture g1, Fixture g2, Contact clist)
        {
            if (enabled)
            {
                if (g1.Body.BodyId == 1 || g1.Body.BodyId == 2)
                {
                    if (((Ship)g1.UserData).WeaponB == WeaponList.None)
                    {
                        ((Ship)g1.UserData).WeaponB = this.weapon;
                    }
                    else if (((Ship)g1.UserData).WeaponC == WeaponList.None)
                    {
                        ((Ship)g1.UserData).WeaponC = this.weapon;
                    }
                    else
                    {
                        if (((Ship)g1.UserData).WeaponBFired == false)
                        {
                            ((Ship)g1.UserData).WeaponB = this.weapon;
                        }
                        else if (((Ship)g1.UserData).WeaponCFired == false)
                        {
                            ((Ship)g1.UserData).WeaponC = this.weapon;
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
