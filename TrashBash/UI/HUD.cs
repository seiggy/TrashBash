using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TrashBash.ScreenSystem;
using Microsoft.Xna.Framework;
using TrashBash.Objects;
using TrashBash.Objects.Weapons;

namespace TrashBash.UI
{
    public class HUD
    {
        private Texture2D hudl;
        private Texture2D hudr;
        private Texture2D hudbuttons;
        private Texture2D fuell;
        private Texture2D fuelr;
        private Texture2D fuelglass;
        private Texture2D blackHole;
        private Texture2D conLaser;
        private Texture2D empGrenade;
        private Texture2D empLaser;
        private Texture2D revRay;
        private Texture2D stunLaser;

        private Texture2D player1Dot;
        private Texture2D player2Dot;

        private Texture2D minimap;

        public int fuelLeft = 100;
        public int fuelRight = 100;

        private Rectangle fuelLeftSource;
        private Rectangle fuelRightSource;

        public Vector2 fuelLeftPos = Vector2.Zero;
        public Vector2 fuelRightPos = Vector2.Zero;

        public Ship Player1;
        public Ship Player2;

        private Vector2 p1WeaponA = Vector2.Zero;
        private Vector2 p1WeaponB = Vector2.Zero;
        private Vector2 p1WeaponC = Vector2.Zero;

        private Vector2 p2WeaponA = Vector2.Zero;
        private Vector2 p2WeaponB = Vector2.Zero;
        private Vector2 p2WeaponC = Vector2.Zero;

        public HUD()
        {
            fuelLeftPos = new Vector2(9, 507);
            fuelRightPos = new Vector2(1223, 507);

            p1WeaponA = new Vector2(315, 638);
            p1WeaponB = new Vector2(390, 638);
            p1WeaponC = new Vector2(465, 638);

            p2WeaponA = new Vector2(731, 638);
            p2WeaponB = new Vector2(806, 638);
            p2WeaponC = new Vector2(881, 638);
        }

        public void Update()
        {
            float height = 187f * ((float)fuelLeft / 100f);
            float y = 187f - height;

            fuelLeftSource = new Rectangle(0, (int)y, 48, (int)height);
            fuelLeftPos = new Vector2(9, 507 + y);

            height = 187f * ((float)fuelRight / 100f);
            y = 187f - height;
            fuelRightSource = new Rectangle(0, (int)y, 48, (int)height);
            fuelRightPos = new Vector2(1223, 507 + y);
        }

        public void LoadContent(ScreenManager screenManager)
        {
            hudl = screenManager.ContentManager.Load<Texture2D>("Content/UI/HUD_L");
            hudr = screenManager.ContentManager.Load<Texture2D>("Content/UI/HUD_R");
            hudbuttons = screenManager.ContentManager.Load<Texture2D>("Content/UI/HUD_buttons");
            fuell = screenManager.ContentManager.Load<Texture2D>("Content/UI/fuel_L");
            fuelr = screenManager.ContentManager.Load<Texture2D>("Content/UI/fuel_R");
            fuelglass = screenManager.ContentManager.Load<Texture2D>("Content/UI/fuelGlass");
            blackHole = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/Icons/blackHole");
            conLaser = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/Icons/conLaser");
            empGrenade = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/Icons/empGrenade");
            empLaser = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/Icons/empLaser");
            revRay = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/Icons/revRay");
            stunLaser = screenManager.ContentManager.Load<Texture2D>("Content/Weapons/Icons/stunLaser");
            player1Dot = screenManager.ContentManager.Load<Texture2D>("Content/UI/player1dot");
            player2Dot = screenManager.ContentManager.Load<Texture2D>("Content/UI/player2dot");
            minimap = screenManager.ContentManager.Load<Texture2D>("Content/UI/miniMap");
        }

        public void Draw(ScreenManager screenManager)
        {
            screenManager.SpriteBatch.Draw(hudl, Vector2.Zero, Color.White);
            screenManager.SpriteBatch.Draw(hudr, new Vector2(hudl.Width, 0), Color.White);

            // draw weapon icons
            switch (Player1.WeaponB)
            {
                case WeaponList.BlackHole:
                    screenManager.SpriteBatch.Draw(blackHole, p1WeaponB, Color.White);
                    break;
                case WeaponList.ConcussionGrenade:
                    screenManager.SpriteBatch.Draw(revRay, p1WeaponB, Color.White);
                    break;
                case WeaponList.EMPWeapon:
                    screenManager.SpriteBatch.Draw(empGrenade, p1WeaponB, Color.White);
                    break;
            }
            switch (Player1.WeaponC)
            {
                case WeaponList.BlackHole:
                    screenManager.SpriteBatch.Draw(blackHole, p1WeaponC, Color.White);
                    break;
                case WeaponList.ConcussionGrenade:
                    screenManager.SpriteBatch.Draw(revRay, p1WeaponC, Color.White);
                    break;
                case WeaponList.EMPWeapon:
                    screenManager.SpriteBatch.Draw(empGrenade, p1WeaponC, Color.White);
                    break;
            }
            switch (Player2.WeaponB)
            {
                case WeaponList.BlackHole:
                    screenManager.SpriteBatch.Draw(blackHole, p2WeaponB, Color.White);
                    break;
                case WeaponList.ConcussionGrenade:
                    screenManager.SpriteBatch.Draw(revRay, p2WeaponB, Color.White);
                    break;
                case WeaponList.EMPWeapon:
                    screenManager.SpriteBatch.Draw(empGrenade, p2WeaponB, Color.White);
                    break;
            }
            switch (Player2.WeaponC)
            {
                case WeaponList.BlackHole:
                    screenManager.SpriteBatch.Draw(blackHole, p2WeaponC, Color.White);
                    break;
                case WeaponList.ConcussionGrenade:
                    screenManager.SpriteBatch.Draw(revRay, p2WeaponC, Color.White);
                    break;
                case WeaponList.EMPWeapon:
                    screenManager.SpriteBatch.Draw(empGrenade, p2WeaponC, Color.White);
                    break;
            }

            screenManager.SpriteBatch.Draw(hudbuttons, Vector2.Zero, Color.White);
            // add logic for fuel changes
            screenManager.SpriteBatch.Draw(fuell, fuelLeftPos, fuelLeftSource, Color.White);
            screenManager.SpriteBatch.Draw(fuelr, fuelRightPos, fuelRightSource, Color.White);

            screenManager.SpriteBatch.Draw(fuelglass, Vector2.Zero, Color.White);

            Vector2 p1dot = new Vector2((Player1.Body.Position.X * .045f + 517), (Player1.Body.Position.Y * .042f + 499));
            Vector2 p2dot = new Vector2((Player2.Body.Position.X * .045f + 517), (Player2.Body.Position.Y * .042f + 499));
            Vector2 dotOrigin = new Vector2(player1Dot.Width / 2, player2Dot.Width / 2);

            screenManager.SpriteBatch.Draw(minimap, Vector2.Zero, Color.White);

            screenManager.SpriteBatch.Draw(player1Dot, p1dot, null, Color.White, 0.0f, dotOrigin, 1.0f, SpriteEffects.None, 0);
            screenManager.SpriteBatch.Draw(player2Dot, p2dot, null, Color.White, 0.0f, dotOrigin, 1.0f, SpriteEffects.None, 0);
        }
    }
}
