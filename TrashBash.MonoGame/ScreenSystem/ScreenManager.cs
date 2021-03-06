﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashBash.MonoGame.ScreenSystem
{
    public class ScreenManager : DrawableGameComponent
    {
        private Texture2D _blankTexture;
        private IGraphicsDeviceService _graphicsDeviceService;
        private InputState _inpuet = new InputState();
        private List<GameScreen> _screens = new List<GameScreen>();
        private List<GameScreen> _screensToUpdate = new List<GameScreen>();
        private SpriteFont _spriteFonts;

        public ScreenManager(Game game) : base(game)
        {
            ContentManager = new ContentManager(game.Services);
            _graphicsDeviceService = (IGraphicsDeviceService)game.Services.GetService(
                typeof(IGraphicsDeviceService));

            game.Exiting += Game_Exiting;
            TraceEnabled = false;

            if (_graphicsDeviceService == null)
                throw new InvalidOperationException("No graphics device service.");
        }

        public SpriteFonts SpriteFonts
        {
            get; set;
        }

        /// <summary>
        /// Expose access to our Game instance (this is protected in the
        /// default <see cref="GameComponent"/>, but we want to make it public).
        /// </summary>
        public new Game Game
        {
            get { return base.Game; }
        }

        /// <summary>
        /// Expose access to our graphics device (this is protected in the
        /// default <see cref="DrawableGameComponent"/>, but we want to make it public).
        /// </summary>
        public new GraphicsDevice GraphicsDevice
        {
            get { return base.GraphicsDevice; }
        }

        /// <summary>
        /// A content manager used to load data that is shared between multiple
        /// screens. This is never unloaded, so if a screen requires a large amount
        /// of temporary data, it should create a local content manager instead.
        /// </summary>
        public ContentManager ContentManager { get; private set; }

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        public Vector2 ScreenCenter
        {
            get
            {
                return new Vector2(_graphicsDeviceService.GraphicsDevice.Viewport.Width / 2f,
                                   _graphicsDeviceService.GraphicsDevice.Viewport.Height / 2f);
            }
        }

        public int ScreenWidth
        {
            get { return _graphicsDeviceService.GraphicsDevice.Viewport.Width; }
        }

        public int ScreenHeight
        {
            get { return _graphicsDeviceService.GraphicsDevice.Viewport.Height; }
        }

        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled { get; set; }

        private void Game_Exiting(object sender, EventArgs e)
        {
            //Make sure to dispose ALL screens when the game is forcefully closed
            //We do this to ensure that open resources and threads created by screens are closed.
            foreach (GameScreen screen in _screens)
            {
                screen.Dispose();
            }

            _screens.Clear();
            _screensToUpdate.Clear();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            _spriteFonts = new SpriteFonts(ContentManager);

            foreach (GameScreen screen in _screens)
            {
                screen.Initialize();
            }

            // Player 1 keyboard controlls
            InputState.P1Controller.keyAction = Keys.Space;
            InputState.P1Controller.keyBack = Keys.Escape;
            InputState.P1Controller.keyDown = Keys.S;
            InputState.P1Controller.keyLeft = Keys.A;
            InputState.P1Controller.keyPause = Keys.Enter;
            InputState.P1Controller.keyRight = Keys.D;
            InputState.P1Controller.keyThrust = Keys.W;
            InputState.P1Controller.keyReverse = Keys.S;
            InputState.P1Controller.keyTractor = Keys.Space;
            InputState.P1Controller.keyUp = Keys.W;
            InputState.P1Controller.keyWeaponA = Keys.Z;
            InputState.P1Controller.keyWeaponB = Keys.X;
            InputState.P1Controller.keyWeaponC = Keys.C;

            //Player 2 keyboard controlls
            InputState.P2Controller.keyAction = Keys.Enter;
            InputState.P2Controller.keyBack = Keys.End;
            InputState.P2Controller.keyDown = Keys.Down;
            InputState.P2Controller.keyLeft = Keys.Left;
            InputState.P2Controller.keyPause = Keys.Subtract;
            InputState.P2Controller.keyRight = Keys.Right;
            InputState.P2Controller.keyThrust = Keys.Up;
            InputState.P2Controller.keyReverse = Keys.Down;
            InputState.P2Controller.keyTractor = Keys.NumPad1;
            InputState.P2Controller.keyUp = Keys.Up;
            InputState.P2Controller.keyWeaponA = Keys.NumPad4;
            InputState.P2Controller.keyWeaponB = Keys.NumPad5;
            InputState.P2Controller.keyWeaponC = Keys.NumPad6;


            //Player 1 game pad controlls
            InputState.P1Controller.joyAction = Buttons.A;
            InputState.P1Controller.joyBack = Buttons.Back;
            InputState.P1Controller.joyMove = Buttons.LeftStick;
            InputState.P1Controller.joyPause = Buttons.Start;
            InputState.P1Controller.joyTractor = Buttons.A;
            InputState.P1Controller.joyTriggerThrust = Buttons.RightTrigger;
            InputState.P1Controller.joyTriggerReverse = Buttons.LeftTrigger;
            InputState.P1Controller.joyWeaponA = Buttons.X;
            InputState.P1Controller.joyWeaponB = Buttons.Y;
            InputState.P1Controller.joyWeaponC = Buttons.B;

            //Player 2 game pad controlls (same as Player ones)
            InputState.P2Controller.joyAction = Buttons.A;
            InputState.P2Controller.joyBack = Buttons.Back;
            InputState.P2Controller.joyMove = Buttons.LeftStick;
            InputState.P2Controller.joyPause = Buttons.Start;
            InputState.P2Controller.joyTractor = Buttons.A;
            InputState.P2Controller.joyTriggerThrust = Buttons.RightTrigger;
            InputState.P2Controller.joyTriggerReverse = Buttons.LeftTrigger;
            InputState.P2Controller.joyWeaponA = Buttons.X;
            InputState.P2Controller.joyWeaponB = Buttons.Y;
            InputState.P2Controller.joyWeaponC = Buttons.B;


            base.Initialize();
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _blankTexture = ContentManager.Load<Texture2D>("Content/Common/blank");

            // Tell each of the _screens to load their content.
            foreach (GameScreen screen in _screens)
            {
                screen.LoadContent();
            }
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            ContentManager.Unload();

            // Tell each of the _screens to unload their content.
            foreach (GameScreen screen in _screens)
            {
                screen.UnloadContent();
            }
        }

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            _input.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();

            foreach (SignedInGamer gamer in SignedInGamer.SignedInGamers)
            {
                gamer.Presence.PresenceMode = GamerPresenceMode.WastingTime;
            }

            for (int i = 0; i < _screens.Count; i++)
                _screensToUpdate.Add(_screens[i]);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are _screens waiting to be updated.
            while (_screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];

                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                bool otherScreenHasFocusClone = otherScreenHasFocus;
                bool coveredByOtherScreenClone = coveredByOtherScreen;

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle _input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(_input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // _screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }

                // POINT OF INTEREST
                // This is moved here from the screen's update to be able to handle
                // the user input before starting the physics processing on the other thread,
                // as the HandleInput functions interact with the simulator
                screen.UpdatePhysics(gameTime, coveredByOtherScreenClone, otherScreenHasFocusClone);
            }

            // Print debug trace?
            if (TraceEnabled)
                TraceScreens();
        }


        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        private void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in _screens)
                screenNames.Add(screen.GetType().Name);

            Trace.WriteLine(string.Join(", ", screenNames.ToArray()));
        }

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < _screens.Count; i++)
            {
                if (_screens[i].ScreenState == ScreenState.Hidden)
                    continue;

                _screens[i].Draw(gameTime);
            }
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.Initialize();
            // If we have a graphics device, tell the screen to load content.
            if ((_graphicsDeviceService != null) &&
                (_graphicsDeviceService.GraphicsDevice != null))
            {
                screen.LoadContent();
            }

            _screens.Add(screen);
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use <see cref="GameScreen"/>.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if ((_graphicsDeviceService != null) &&
                (_graphicsDeviceService.GraphicsDevice != null))
            {
                screen.UnloadContent();
            }

            _screens.Remove(screen);
            _screensToUpdate.Remove(screen);

            screen.Dispose();
        }

        /// <summary>
        /// Helper draws a translucent black full screen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(int alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            SpriteBatch.Begin();

            SpriteBatch.Draw(_blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             new Color(0, 0, 0, (byte)alpha));

            SpriteBatch.End();
        }
    }
}
