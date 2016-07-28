using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using TrashBash.ScreenSystem;
using TrashBash.SoundSystem;

namespace TrashBash
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TrashBash : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        private ScreenManager screenManager;

        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;

        public TrashBash()
        {
            Window.Title = "Trash Bash";
            graphics = new GraphicsDeviceManager(this);
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
#if !XBOX
            // Window mode
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.IsFullScreen = false;
            
            IsMouseVisible = false;

#else
            // Xbox 360 Code
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
#endif

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            screenManager.AddScreen(new LogoScreen());
        }

        public ScreenManager ScreenManager
        {
            get { return this.screenManager; }
            set { this.screenManager = value; }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            screenManager.GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
