using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrashBash.Levels;
using TrashBash.SoundSystem;

namespace TrashBash.ScreenSystem
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    internal class MainMenuScreen : MenuScreen
    {
        Texture2D background;
        Texture2D logo;
        Texture2D start;
        Texture2D startOn;
        Texture2D options;
        Texture2D optionsOn;
        Texture2D help;
        Texture2D helpOn;
        Texture2D exit;
        Texture2D exitOn;
        Texture2D loading;
        bool DrawLoading = false;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
        {
            MenuEntries.Add("Start");
            MenuEntries.Add("Options");
            MenuEntries.Add("Help");
            MenuEntries.Add("Exit");
            LeftBorder = 100;
        }

        public override void LoadContent()
        {
            background = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/TitleScreen");
            start = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/start");
            startOn = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/start_ON");
            options = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/Credits");
            optionsOn = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/options_ON");
            help = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/help");
            helpOn = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/help_ON");
            exit = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/exit");
            exitOn = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/exit_ON");
            logo = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/Logo");
            loading = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/LoadingScreen");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (DrawLoading)
            {
                ScreenManager.AddScreen(new Level1());
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Responds to user menu selections.
        /// </summary>
        protected override void OnSelectEntry(int entryIndex)
        {
            switch (entryIndex)
            {
                case 0:
                    DrawLoading = true;
                    break;
                case 1:
                    ScreenManager.AddScreen(new CreditsScreen());
                    break;
                case 2:
                    ScreenManager.AddScreen(new HelpScreen());
                    break;
                case 3:
                    ScreenManager.Game.Exit();
                    break;
                default:
                    break;
            }

            ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            Rectangle rect = new Rectangle(0, 0, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
            ScreenManager.SpriteBatch.Draw(background, rect, Color.White);
            ScreenManager.SpriteBatch.Draw(start, rect, Color.White);
            ScreenManager.SpriteBatch.Draw(options, rect, Color.White);
            ScreenManager.SpriteBatch.Draw(help, rect, Color.White);
            ScreenManager.SpriteBatch.Draw(exit, rect, Color.White);
            switch (SelectedEntry)
            {
                case 0:
                    ScreenManager.SpriteBatch.Draw(startOn, rect, Color.White);                    
                    break;
                case 1:
                    ScreenManager.SpriteBatch.Draw(optionsOn, rect, Color.White);
                    break;
                case 2:
                    ScreenManager.SpriteBatch.Draw(helpOn, rect, Color.White);
                    break;
                case 3:
                    ScreenManager.SpriteBatch.Draw(exitOn, rect, Color.White);
                    break;
            }

            ScreenManager.SpriteBatch.Draw(logo, rect, Color.White);

            if (DrawLoading)
            {
                ScreenManager.SpriteBatch.Draw(loading, rect, Color.White);
            }

            // base.Draw(gameTime);
            ScreenManager.SpriteBatch.End();
        }
    }
}