using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TrashBash.MonoGame.ScreenSystem
{
    class CreditsScreen : MenuScreen
    {
        Texture2D background;

        public CreditsScreen()
        {
            MenuEntries.Add("OK");
        }

        public override void LoadContent()
        {
            background = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/CreditScreen");
            base.LoadContent();
        }

        protected override void OnSelectEntry(int entryIndex)
        {
            ScreenManager.AddScreen(new MainMenuScreen());
            ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(blendState: BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();
        }
    }
}
