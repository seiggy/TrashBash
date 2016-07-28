using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TrashBash.ScreenSystem
{
    public class HelpScreen : MenuScreen
    {
        Texture2D background;

        public HelpScreen()
        {
            MenuEntries.Add("OK");
        }

        public override void LoadContent()
        {
            background = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/PRELEVEL");
            base.LoadContent();
        }

        protected override void OnSelectEntry(int entryIndex)
        {
            ScreenManager.AddScreen(new MainMenuScreen());
            ExitScreen();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            Rectangle rect = new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
            ScreenManager.SpriteBatch.Draw(background, rect, Color.White);
            // base.Draw(gameTime);
            ScreenManager.SpriteBatch.End();
        }
    }
}
