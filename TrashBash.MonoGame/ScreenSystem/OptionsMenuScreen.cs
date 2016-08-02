using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TrashBash.MonoGame.SoundSystem;

namespace TrashBash.MonoGame.ScreenSystem
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    internal class OptionsMenuScreen : MenuScreen
    {
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public OptionsMenuScreen()
        {
            MenuEntries.Add("Fullscreen - " + TrashBash.graphics.IsFullScreen.ToString());
            MenuEntries.Add("Resolution - " + TrashBash.graphics.PreferredBackBufferWidth + "x" + TrashBash.graphics.PreferredBackBufferHeight);
            MenuEntries.Add("Edit Controls");
            MenuEntries.Add("Current Song: " + MediaPlayer.Queue.ActiveSong.Artist + "-" + MediaPlayer.Queue.ActiveSong.Name);
            MenuEntries.Add("Back");
            LeftBorder = 100;
        }

        /// <summary>
        /// Responds to user menu selections.
        /// </summary>
        protected override void OnSelectEntry(int entryIndex)
        {
            switch (entryIndex)
            {
                case 0:
                    TrashBash.graphics.ToggleFullScreen();
                    MenuEntries[0] = "Fullscreen - " + TrashBash.graphics.IsFullScreen.ToString();
                    ScreenManager.AddScreen(new OptionsMenuScreen());
                    break;
                case 1:
                    switch (ScreenManager.ScreenWidth)
                    {
                        case 1440:
                            TrashBash.ScreenWidth = 1280;
                            TrashBash.ScreenHeight = 720;
                            break;
                        case 1280:
                            TrashBash.ScreenWidth = 1024;
                            TrashBash.ScreenHeight = 768;
                            break;
                        case 1024:
                            TrashBash.ScreenWidth = 1440;
                            TrashBash.ScreenHeight = 900;
                            break;
                    }
                    TrashBash.graphics.PreferredBackBufferWidth = TrashBash.ScreenWidth;
                    TrashBash.graphics.PreferredBackBufferHeight = TrashBash.ScreenHeight;
                    TrashBash.graphics.ApplyChanges();
                    MenuEntries[1] = "Resolution - " + TrashBash.graphics.PreferredBackBufferWidth + "x" + TrashBash.graphics.PreferredBackBufferHeight;
                    ScreenManager.AddScreen(new OptionsMenuScreen());
                    break;
                case 2:
                    ScreenManager.AddScreen(new OptionsMenuScreen());
                    break;
                case 3:
                    SoundManager.PlayRandomSong();
                    MenuEntries[3] = "Current Song: " + MediaPlayer.Queue.ActiveSong.Artist + "-" + MediaPlayer.Queue.ActiveSong.Name;
                    ScreenManager.AddScreen(new OptionsMenuScreen());
                    break;
                case 4:
                    ScreenManager.AddScreen(new MainMenuScreen());
                    break;
                default:
                    // Default to exit
                    break;
            }

            ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(blendState: BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.DiagnosticSpriteFont,
                                                 "1) Toggle between debug and normal view using either F1 on the keyboard or 'Y' on the controller",
                                                 new Vector2(100, ScreenManager.ScreenHeight - 116), Color.White);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.DiagnosticSpriteFont,
                                                 "2) Keyboard users, use arrows and enter to navigate menus",
                                                 new Vector2(100, ScreenManager.ScreenHeight - 100), Color.White);
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.End();
        }
    }
}
