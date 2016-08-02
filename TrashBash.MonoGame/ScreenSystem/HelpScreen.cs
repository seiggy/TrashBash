using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashBash.MonoGame.ScreenSystem
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
            ScreenManager.SpriteBatch.Begin(blendState: BlendState.AlphaBlend);
            Rectangle rect = new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
            ScreenManager.SpriteBatch.Draw(background, rect, Color.White);
            // base.Draw(gameTime);
            ScreenManager.SpriteBatch.End();
        }
    }
}