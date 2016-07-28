/// <filename>Level1.cs</filename>
/// <company>QuirkFace Games</company
/// <author>Zachary Way</author>
using TrashBash.ScreenSystem;
using System.Text;
using System.Threading;
using FarseerGames.FarseerPhysics;
using Microsoft.Xna.Framework;
using TrashBash.Objects;
using Microsoft.Xna.Framework.Input;
using System;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Springs;
using Microsoft.Xna.Framework.Graphics;
using TrashBash.SoundSystem;
using Microsoft.Xna.Framework.Audio;
using TrashBash.Objects.Weapons;
using System.Collections.Generic;
using FarseerGames.FarseerPhysics.Collisions;
using TrashBash.UI;
using System.Diagnostics;
using SpriteSheetRuntime;

namespace TrashBash.Levels
{
    /// <summary>
    /// Level 1 class. This allows us to change the way
    /// the gameplay works on some basic levels between
    /// each "stage" of the game.
    /// </summary>
    public class Level1 : GameScreen
    {
        /// <summary>
        /// Ship Object for Player 1.
        /// TODO: refactor and create Player 2
        /// as well
        /// </summary>
        private Ship p1Ship;

        /// <summary>
        /// Ship object for PLayer 2
        /// </summary>
        private Ship p2Ship;

        /// <summary>
        /// Array of trash Objects. Look into using
        /// a BST or some faster dictionary type
        /// object for resource pooling the trash.
        /// </summary>
        Pool<Trash> trash = new Pool<Trash>();
        List<Trash> trashToDraw = new List<Trash>();

        /// <summary>
        /// large trash objects
        /// </summary>
        Trash[] largeTrash = new Trash[3];

        /// <summary>
        /// Land objects
        /// </summary>
        private Land[,] land;

        private Texture2D[,] background;
        private Texture2D[,] effectLayer;
        private Vector2[,] bgPos;

        private Base p1Base;
        private Base p2Base;

        Camera p1Camera;
        Camera p2Camera;

        UI.HUD hud = new UI.HUD();

        Pool<Laser> p1Bullets = new Pool<Laser>();
        List<Laser> p1BulletsToDraw = new List<Laser>();
        int p1MaxBullets = 10;

        Pool<Laser> p2Bullets = new Pool<Laser>();
        List<Laser> p2BulletsToDraw = new List<Laser>();
        int p2MaxBullets = 10;

        Pool<BlackHole> bholeWeapon = new Pool<BlackHole>();
        List<BlackHole> bholeToDraw = new List<BlackHole>();
        int bholeMax = 4;

        Pool<ConcussionGrenade> conGrenWeapon = new Pool<ConcussionGrenade>();
        List<ConcussionGrenade> conGrenToDraw = new List<ConcussionGrenade>();
        int conGrenMax = 4;

        Pool<EmpWeapon> empWeapon = new Pool<EmpWeapon>();
        List<EmpWeapon> empToDraw = new List<EmpWeapon>();
        int empMax = 4;

        private bool loading = true;

        List<PowerUp> powerUps = new List<PowerUp>();
        int powerUpCount = 5;

        float rotAmt = .068f;

        Texture2D loadScreen;

        Score p1Score = new Score();
        Score p2Score = new Score();

        Stopwatch levelTime;
        bool started = false;

        Texture2D countdown;
        Rectangle[] countRects = new Rectangle[10];
        int countSecs = 0;

        /// <summary>
        /// Dispose...
        /// </summary>
        public override void Dispose()
        {
            //physicsProcessor.Dispose();
            //physicsProcessor = null;
            //physicsThread.Join();
            //physicsThread = null;
            base.Dispose();
        }

        /// <summary>
        /// Initialize our variables before the stage begins...
        /// We may need to create some sort of "Loading" screen
        /// </summary>
        public override void Initialize()
        {
            // Init Physics engine
            InitPhysics();

            // Init the players
            InitPlayers();

            // init the landmass
            land = new Land[11, 11];
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    land[i, j] = new Land(Vector2.Zero);
                }
            }

            background = new Texture2D[11, 11];
            effectLayer = new Texture2D[11, 11];
            bgPos = new Vector2[11, 11];

            InitScoreLocations();

            // init the border. Comment out in final
            // border = new Border(ScreenManager.ScreenWidth, ScreenManager.ScreenHeight, 30, ScreenManager.ScreenCenter);

            base.Initialize();
        }

        #region Init Methods
        /// <summary>
        /// Initializes the Vector2 objects that determines where each score digit is drawn
        /// </summary>
        public void InitScoreLocations()
        {
            ////////////////////////////////////////////
            // Score Locations                        //
            ////////////////////////////////////////////
            p1Score.Position = new Vector2(100, 660);
            p2Score.Position = new Vector2(1000, 660);
        }

        /// <summary>
        /// Initializes the physics engine
        /// </summary>
        public void InitPhysics()
        {
            // Init the Physics Engine. Pass Vector2 representing gravity
            // direction and strength
            PhysicsSimulator = new PhysicsSimulator(new Vector2(0, 40));
            PhysicsSimulator.InactivityController.Enabled = true;
            PhysicsSimulator.InactivityController.ActivationDistance = 800;
            PhysicsSimulator.InactivityController.MaxIdleTime = 2000;
            PhysicsSimulator.AllowedPenetration = 0f;
            PhysicsSimulator.BiasFactor = .4f;

            // Physics Sim View. This is used for debugging. Can be removed
            // or disabled in final release
            PhysicsSimulatorView = new PhysicsSimulatorView(PhysicsSimulator);
        }

        /// <summary>
        /// Initializes the player objects
        /// </summary>
        public void InitPlayers()
        {
            // create the player one p1Ship object
            p1Ship = new Ship(new Vector2(1880, 2190));
            p1Camera = new Camera(new Vector2(400, 500));
            p1Camera.Zoom = .75f;

            p1Base = new Base(new Vector2(1880, 2275), p1Ship);

            // create the player two p2ship object
            p2Ship = new Ship(new Vector2(3700, 2190));
            p2Camera = new Camera(new Vector2(500, 500));
            p2Camera.Zoom = .75f;

            p2Base = new Base(new Vector2(3700, 2275), p2Ship);
        }
        #endregion

        /// <summary>
        /// Load Art Assets
        /// </summary>
        public override void LoadContent()
        {
            SoundManager.Initialize(ScreenManager.ContentManager);

            loadScreen = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/LoadingScreen");

            // load the player objects
            LoadPlayers();

            // load the weapon objects
            LoadWeapons();

            // load the trash objects
            LoadTrash();

            // load the land
            LoadLand();

            hud.LoadContent(ScreenManager);

            hud.Player1 = p1Ship;
            hud.Player2 = p2Ship;

            LoadPowerups();

            countdown = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/countdown");

            for (int i = 0; i < 10; i++)
            {
                countRects[i] = new Rectangle(i * 80, 0, 80, 80);
            }

            base.LoadContent();
        }

        #region Content Loaders
        /// <summary>
        /// Loads and inits the powerups for the level
        /// </summary>
        private void LoadPowerups()
        {
            Random rand = new Random();

            for (int i = 0; i < powerUpCount; i++)
            {
                PowerUp p = new PowerUp();
                int w = rand.Next() % 2;
                WeaponList we = WeaponList.None;
                switch (w)
                {
                    case 0:
                        we = WeaponList.BlackHole;
                        break;
                    case 1:
                        we = WeaponList.EMPWeapon;
                        break;
                    case 2:
                        we = WeaponList.ConcussionGrenade;
                        break;
                }
                p.LoadContent(ScreenManager, we);
                powerUps.Add(p);
                PhysicsSimulator.Add(p.Body);
                PhysicsSimulator.Add(p.Geom);
            }

            powerUps[0].Position = new Vector2(2740, 1330);
            powerUps[1].Position = new Vector2(2800, 2600);
            powerUps[2].Position = new Vector2(1840, 4750);
            powerUps[3].Position = new Vector2(4000, 4680);
            powerUps[4].Position = new Vector2(4300, 3750);
        }

        /// <summary>
        /// Loads the landmass content
        /// </summary>
        private void LoadLand()
        {
            for (int i = 0; i < 11; i++)
            {
                string s = "";
                switch (i)
                {
                    case 0:
                        s = "a";
                        break;
                    case 1:
                        s = "b";
                        break;
                    case 2:
                        s = "c";
                        break;
                    case 3:
                        s = "d";
                        break;
                    case 4:
                        s = "e";
                        break;
                    case 5:
                        s = "f";
                        break;
                    case 6:
                        s = "g";
                        break;
                    case 7:
                        s = "h";
                        break;
                    case 8:
                        s = "i";
                        break;
                    case 9:
                        s = "j";
                        break;
                    case 10:
                        s = "k";
                        break;
                    default:
                        break;
                }

                for (int j = 0; j < 11; j++)
                {
                    land[i, j].Load(ScreenManager, PhysicsSimulator, s + (j + 1).ToString());
                    land[i, j].Position = new Vector2(512 * j + land[i, j].Origin.X, 512 * i + land[i, j].Origin.Y);
                    land[i, j].UpdatePos();
                }
            }

            for (int i = 0; i < 11; i++)
            {
                string s = "";
                switch (i)
                {
                    case 0:
                        s = "a";
                        break;
                    case 1:
                        s = "b";
                        break;
                    case 2:
                        s = "c";
                        break;
                    case 3:
                        s = "d";
                        break;
                    case 4:
                        s = "e";
                        break;
                    case 5:
                        s = "f";
                        break;
                    case 6:
                        s = "g";
                        break;
                    case 7:
                        s = "h";
                        break;
                    case 8:
                        s = "i";
                        break;
                    case 9:
                        s = "j";
                        break;
                    case 10:
                        s = "k";
                        break;
                    default:
                        break;
                }

                for (int j = 0; j < 11; j++)
                {
                    background[i, j] = ScreenManager.ContentManager.Load<Texture2D>("Content/Land/Background/" + s + (j + 1).ToString());
                    bgPos[i, j] = new Vector2(512 * j, 512 * i);
                    try
                    {
                        effectLayer[i, j] = ScreenManager.ContentManager.Load<Texture2D>("Content/Land/Effect/" + s + (j + 1).ToString());

                    }
                    catch (Exception e)
                    {
                        effectLayer[i, j] = ScreenManager.ContentManager.Load<Texture2D>("Content/Land/blank");

                    }
                }
            }
        }

        /// <summary>
        /// Content loader for trash objects
        /// </summary>
        private void LoadTrash()
        {
            for (int i = 0; i < 25; i++)
            {
                Trash t = new Trash(Vector2.Zero);
                if (i % 2 == 0)
                    t.Load(ScreenManager.GraphicsDevice, ScreenManager.ContentManager, PhysicsSimulator, "box01", 2);
                else
                    t.Load(ScreenManager.GraphicsDevice, ScreenManager.ContentManager, PhysicsSimulator, "box02", 2);
                t.Body.Name = "trash";
                t.ScoreValue = 100;
                trash.Insert(t);
            }

            largeTrash[0] = new Trash(new Vector2(2100, 4160));
            largeTrash[0].Load(ScreenManager.GraphicsDevice, ScreenManager.ContentManager, PhysicsSimulator, "fuelTank", 8);
            largeTrash[0].Body.Name = "trash";
            largeTrash[0].ScoreValue = 200;
            PhysicsSimulator.Add(largeTrash[0].Body);
            PhysicsSimulator.Add(largeTrash[0].Geom);

            largeTrash[2] = new Trash(new Vector2(3910, 3630));
            largeTrash[2].Load(ScreenManager.GraphicsDevice, ScreenManager.ContentManager, PhysicsSimulator, "fuelTank", 8);
            largeTrash[2].Body.Name = "trash";
            largeTrash[2].ScoreValue = 200;
            PhysicsSimulator.Add(largeTrash[2].Body);
            PhysicsSimulator.Add(largeTrash[2].Geom);

            largeTrash[1] = new Trash(new Vector2(1870, 3780));
            largeTrash[1].Load(ScreenManager.GraphicsDevice, ScreenManager.ContentManager, PhysicsSimulator, "brokenShip", 15);
            largeTrash[1].Body.Name = "trash";
            largeTrash[1].ScoreValue = 500;
            PhysicsSimulator.Add(largeTrash[1].Body);
            PhysicsSimulator.Add(largeTrash[1].Geom);

            for (int i = 0; i < 5; i++)
            {
                Trash t = trash.Fetch();
                t.Body.Position = new Vector2(1220 + (i * 80), 3100 - (i * 5));
                trashToDraw.Add(t);
                PhysicsSimulator.Add(t.Body);
                PhysicsSimulator.Add(t.Geom);
            }

            for (int i = 0; i < 5; i++)
            {
                Trash t = trash.Fetch();
                t.Body.Position = new Vector2(1500 + (i * 80), 4920 - (i * 5));
                trashToDraw.Add(t);
                PhysicsSimulator.Add(t.Body);
                PhysicsSimulator.Add(t.Geom);
            }

            for (int i = 0; i < 5; i++)
            {
                Trash t = trash.Fetch();
                t.Body.Position = new Vector2(3500 + (i * 80), 4900 - (i * 5));
                trashToDraw.Add(t);
                PhysicsSimulator.Add(t.Body);
                PhysicsSimulator.Add(t.Geom);
            }

            for (int i = 0; i < 5; i++)
            {
                Trash t = trash.Fetch();
                t.Body.Position = new Vector2(4000 + (i * 80), 2850 - (i * 5));
                trashToDraw.Add(t);
                PhysicsSimulator.Add(t.Body);
                PhysicsSimulator.Add(t.Geom);
            }

            for (int i = 0; i < 5; i++)
            {
                Trash t = trash.Fetch();
                t.Body.Position = new Vector2(2500 + (i * 80), 1500 - (i * 5));
                trashToDraw.Add(t);
                PhysicsSimulator.Add(t.Body);
                PhysicsSimulator.Add(t.Geom);
            }
        }

        /// <summary>
        /// Content loader for the weapons
        /// </summary>
        private void LoadWeapons()
        {
            for (int i = 0; i < p1MaxBullets; i++)
            {
                Laser l = new Laser();
                l.Load(ScreenManager, PhysicsSimulator);
                l.isAvailable = true;
                l.BulletGeom.Tag = l;
                p1Bullets.Insert(l);
            }

            for (int i = 0; i < p2MaxBullets; i++)
            {
                Laser l = new Laser();
                l.Load(ScreenManager, PhysicsSimulator);
                l.isAvailable = true;
                l.BulletGeom.Tag = l;
                p2Bullets.Insert(l);
            }

            for (int i = 0; i < bholeMax; i++)
            {
                BlackHole bhole = new BlackHole();
                bhole.LoadContent(ScreenManager);
                bhole.Geom.Tag = bhole;
                bholeWeapon.Insert(bhole);
            }

            for (int i = 0; i < conGrenMax; i++)
            {
                ConcussionGrenade conGren = new ConcussionGrenade();
                conGren.LoadContent(ScreenManager);
                conGren.Geom.Tag = conGren;
                conGrenWeapon.Insert(conGren);
            }

            for (int i = 0; i < empMax; i++)
            {
                EmpWeapon empWeap = new EmpWeapon();
                empWeap.LoadContent(ScreenManager);
                empWeap.Geom.Tag = empWeap;
                empWeapon.Insert(empWeap);
            }
        }

        /// <summary>
        /// Content loader for the player objects
        /// </summary>
        private void LoadPlayers()
        {
            p1Ship.Load(ScreenManager.GraphicsDevice, ScreenManager.ContentManager, PhysicsSimulator, "ship_1p");
            p1Ship.Body.Name = "p1Ship";
            p1Ship.Body.Tag = p1Ship;
            p1Ship.TractorActive = false;
            p1Ship.Geom.Tag = p1Ship;
            p1Ship.Geom.Name = "player1";

            p1Base.Load(ScreenManager, PhysicsSimulator, "trash_1");
            p1Score.LoadContent(ScreenManager);

            p2Ship.Load(ScreenManager.GraphicsDevice, ScreenManager.ContentManager, PhysicsSimulator, "ship_2p");
            p2Ship.Body.Name = "p2Ship";
            p2Ship.Body.Tag = p2Ship;
            p2Ship.TractorActive = false;
            p2Ship.Geom.Tag = p2Ship;
            p2Ship.Geom.Name = "player2";

            p2Base.Load(ScreenManager, PhysicsSimulator, "trash_2");
            p2Score.LoadContent(ScreenManager);
        }
        #endregion

        /// <summary>
        /// Unload any content we loaded.
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// update loop
        /// </summary>
        /// <param name="gameTime">time passed</param>
        /// <param name="otherScreenHasFocus">do we have full focus?</param>
        /// <param name="coveredByOtherScreen">is our screen covered?</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime,
            bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // ensure that the physics processor finishes it's thread on the other
            // processor before we continue this thread as well.
            //if (physicsProcessor != null)
            //{
            //    physicsProcessor.BlockUntilIdle();
            //}
            SoundManager.Update();
            p1Ship.Update(PhysicsSimulator, gameTime);
            p2Ship.Update(PhysicsSimulator, gameTime);

            hud.fuelLeft = (int)p1Ship.Fuel;
            hud.fuelRight = (int)p2Ship.Fuel;
            hud.Player1 = p1Ship;
            hud.Player2 = p2Ship;

            hud.Update();

            for (int i = 0; i < trashToDraw.Count; i++)
            {
                if (trashToDraw[i].Geom.Name == "reset")
                {
                    PhysicsSimulator.Remove(trashToDraw[i].Geom);
                    PhysicsSimulator.Remove(trashToDraw[i].Body);
                    trash.Insert(trashToDraw[i]);
                    trashToDraw.Remove(trashToDraw[i]);
                }
            }

            for (int i = 0; i < largeTrash.Length; i++)
            {
                if (largeTrash[i].Geom.Name == "reset")
                {
                    PhysicsSimulator.Remove(largeTrash[i].Geom);
                    PhysicsSimulator.Remove(largeTrash[i].Body);
                }
            }

            for (int i = 0; i < bholeToDraw.Count; i++)
            {
                bool finished = bholeToDraw[i].Update(gameTime);
                if (finished)
                {
                    bholeToDraw[i].Body.ResetDynamics();
                    bholeToDraw[i].Reset();
                    bholeWeapon.Insert(bholeToDraw[i]);
                    bholeToDraw.Remove(bholeToDraw[i]);
                }
            }

            for (int i = 0; i < conGrenToDraw.Count; i++)
            {
                bool finished = conGrenToDraw[i].Update(gameTime);
                if (finished)
                {
                    conGrenToDraw[i].Body.ResetDynamics();
                    conGrenToDraw[i].Reset();
                    conGrenWeapon.Insert(conGrenToDraw[i]);
                    conGrenToDraw.Remove(conGrenToDraw[i]);
                }
            }

            for (int i = 0; i < empToDraw.Count; i++)
            {
                bool finished = empToDraw[i].Update(gameTime);
                if (finished)
                {
                    empToDraw[i].Body.ResetDynamics();
                    empToDraw[i].Reset();
                    empWeapon.Insert(empToDraw[i]);
                    empToDraw.Remove(empToDraw[i]);
                }
            }

            foreach (PowerUp p in powerUps)
            {
                p.Update(gameTime);
            }

            if (p1Ship.TractorActive)
            {
                if (p1Ship.TractorBeam.Body2.Position.Y < p1Base.Body.Position.Y - 50)
                {
                    if ((p1Ship.TractorBeam.Body2.Position.X < p1Base.Body.Position.X + 5)
                        && (p1Ship.TractorBeam.Body2.Position.X > p1Base.Body.Position.X - 5))
                    {
                        p1Ship.TractorBeam.Body2.LinearVelocity = Vector2.Zero;
                        p1Ship.TractorBeam.Body2.ClearImpulse();
                        p1Ship.TractorBeam.Body2.ClearForce();
                        p1Ship.TractorBeam.Body2.ClearTorque();
                        p1Ship.TractorBeam.Enabled = false;
                        p1Ship.TractorActive = false;
                    }
                }
            }

            if (p2Ship.TractorActive)
            {
                if (p2Ship.TractorBeam.Body2.Position.Y < p2Base.Body.Position.Y - 50)
                {
                    if ((p2Ship.TractorBeam.Body2.Position.X < p2Base.Body.Position.X + 5)
                        && (p2Ship.TractorBeam.Body2.Position.X > p2Base.Body.Position.X - 5))
                    {
                        p2Ship.TractorBeam.Body2.LinearVelocity = Vector2.Zero;
                        p2Ship.TractorBeam.Body2.ClearImpulse();
                        p2Ship.TractorBeam.Body2.ClearForce();
                        p2Ship.TractorBeam.Body2.ClearTorque();
                        p2Ship.TractorBeam.Enabled = false;
                        p2Ship.TractorActive = false;
                    }
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draw our game
        /// </summary>
        /// <param name="gameTime">time passed</param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (loading)
            {
                ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.Draw(loadScreen, Vector2.Zero, Color.White);
                ScreenManager.SpriteBatch.End();
            }
            else
            {
                // store the current viewport width
                int viewPortWidth = ScreenManager.GraphicsDevice.Viewport.Width;

                // get a copy of the current viewport
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

                // set the viewport width to half the screen.
                viewport.Width = viewPortWidth / 2;

                // init first viewport for rendering
                ScreenManager.GraphicsDevice.Viewport = viewport;

                // Render the left side of the game here
                DrawScreen(p1Camera.Translate);

                // move the x of the viewport to the center to start the second half
                viewport.X = viewPortWidth / 2;

                // ensure width hasn't changed
                viewport.Width = viewPortWidth / 2;

                ScreenManager.GraphicsDevice.Viewport = viewport;

                // Render the right side of the screen here
                DrawScreen(p2Camera.Translate);

                // reset the viewport
                viewport.X = 0;
                viewport.Width = viewPortWidth;
                ScreenManager.GraphicsDevice.Viewport = viewport;

                // diagnostic display
                ScreenManager.SpriteBatch.Begin();

                hud.Draw(ScreenManager);
                DrawScores();

                if (countSecs < 11 && countSecs > 0)
                {
                    Vector2 countPos = new Vector2(ScreenManager.ScreenWidth / 4, ScreenManager.ScreenHeight / 2);
                    ScreenManager.SpriteBatch.Draw(countdown, countPos, countRects[10 - countSecs], Color.White);
                    countPos = new Vector2((ScreenManager.ScreenWidth / 4) + (ScreenManager.ScreenWidth / 2),
                        ScreenManager.ScreenHeight / 2);
                    ScreenManager.SpriteBatch.Draw(countdown, countPos, countRects[10 - countSecs], Color.White);
                }

                ScreenManager.SpriteBatch.End();
                base.Draw(gameTime);
            }
        }

        private void DrawScores()
        {
            p1Score.DrawScore(ScreenManager, (int)p1Ship.Score);
            p2Score.DrawScore(ScreenManager, (int)p2Ship.Score);
        }

        private void DrawScreen(Matrix translate)
        {
            // begin spritebatch
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                SpriteSortMode.Immediate,
                SaveStateMode.None,
                translate);

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    ScreenManager.SpriteBatch.Draw(background[i, j], bgPos[i, j], Color.White);
                }
            }

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    land[i, j].Draw(ScreenManager.SpriteBatch);
                }
            }

            foreach (BlackHole b in bholeToDraw)
            {
                b.Draw(ScreenManager);
            }

            foreach (ConcussionGrenade c in conGrenToDraw)
            {
                c.Draw(ScreenManager);
            }

            foreach (EmpWeapon w in empToDraw)
            {
                w.Draw(ScreenManager);
            }

            foreach (PowerUp p in powerUps)
            {
                p.Draw(ScreenManager);
            }

            // Draw the Trash Objects
            foreach (Trash t in trashToDraw)
            {
                t.Draw(ScreenManager.SpriteBatch);
            }

            foreach (Trash t in largeTrash)
            {
                if (t.Geom.Name != "reset")
                    t.Draw(ScreenManager.SpriteBatch);
            }

            // call our player draw method
            // do this second so tractor beam is ontop
            p1Ship.Draw(ScreenManager.SpriteBatch);
            p2Ship.Draw(ScreenManager.SpriteBatch);

            foreach (Laser bullet in p1BulletsToDraw)
            {
                bullet.Draw(ScreenManager.SpriteBatch);
            }

            foreach (Laser bullet in p2BulletsToDraw)
            {
                bullet.Draw(ScreenManager.SpriteBatch);
            }

            p1Base.Draw(ScreenManager.SpriteBatch);
            p2Base.Draw(ScreenManager.SpriteBatch);

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    ScreenManager.SpriteBatch.Draw(effectLayer[i, j], bgPos[i, j], Color.White);
                }
            }

            // DEBUG: Draw border
            // border.Draw(ScreenManager.SpriteBatch);

            // finished drawing
            ScreenManager.SpriteBatch.End();
        }
        int elapsedTime = 0;

        /// <summary>
        /// Update the physics simulation
        /// </summary>
        /// <param name="gameTime">time passed</param>
        /// <param name="otherScreenHasFocus">game has focus</param>
        /// <param name="coveredByOtherScreen">game is covered</param>
        public override void UpdatePhysics(Microsoft.Xna.Framework.GameTime gameTime, bool
            otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (otherScreenHasFocus || coveredByOtherScreen)
            {
                p1Ship.Body.Enabled = false;
                p2Ship.Body.Enabled = false;
            }
            else
            {
                if (!p1Ship.Body.Enabled)
                {
                    p1Ship.Body.Enabled = true;
                    p2Ship.Body.Enabled = true;
                    started = true;
                    levelTime = new Stopwatch();
                    levelTime.Start();
                }
            }
            // if (physicsProcessor != null)
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            if (started)
            {
                if (levelTime.Elapsed.Minutes > 4)
                // if(levelTime.Elapsed.Seconds > 5)
                {
                    // end game
                    ScreenManager.AddScreen(new EndGameScreen((int)p1Ship.Score, (int)p2Ship.Score));
                    PauseScreen.allowRun = false;
                    levelTime.Stop();
                    levelTime.Reset();
                    started = false;
                    ExitScreen();
                }
                if (levelTime.Elapsed.Minutes > 3)
                {
                    countSecs = 60 - levelTime.Elapsed.Seconds;
                }
            }

            if (loading == true)
            {
                PauseScreen.allowRun = false;
                loading = true;
            }
            if (elapsedTime < 100)
            {
                while (elapsedTime > 10)
                {
                    PhysicsSimulator.Update(10 * .0015f);
                    elapsedTime -= 10;
                }
            }
            else
            {
                PhysicsSimulator.Update(80 * .002f);
                elapsedTime = 0;
            }

            if (PhysicsSimulator.ArbiterList.Count < 200)
            {
                loading = false;
                PauseScreen.allowRun = true;
            }
            // physicsProcessor.Iterate(gameTime, DebugViewEnabled);
            p1Camera.Position = p1Ship.Body.Position;
            p2Camera.Position = p2Ship.Body.Position;
            if (p1Ship.TractorActive)
            {
                p1Ship.TracBeamObj.PointA = p1Ship.TractorBeam.Body1.Position;
                p1Ship.TracBeamObj.PointB = p1Ship.TractorBeam.Body2.Position;
                p1Ship.TracBeamObj.Update();
            }
            if (p2Ship.TractorActive)
            {
                p2Ship.TracBeamObj.PointA = p2Ship.TractorBeam.Body1.Position;
                p2Ship.TracBeamObj.PointB = p2Ship.TractorBeam.Body2.Position;
                p2Ship.TracBeamObj.Update();
            }
        }

        /// <summary>
        /// Handle input loop
        /// </summary>
        /// <param name="input">state of controllers</param>
        public override void HandleInput(InputState input)
        {
            // if this is the first time the Level's update has run
            // Then we want to show the Pause screen. Should probably
            // change this to a Loading screen instead.
            if (FirstRun)
            {
                ScreenManager.AddScreen(new PauseScreen(GetTitle(), GetDetails(), this));
                //SetupControls(input);
                FirstRun = false;
            }

            // Adds pause screen when player pauses the game.
            if (input.PauseGame)
            {
                ScreenManager.AddScreen(new PauseScreen(GetTitle(), GetDetails(), this));
            }
            else
            {
                // handle keyboard inputs for all players
                HandleKeyboardInput(input);

                // handle joystick inputs for all players
                HandleJoystickInput(input);
            }
            base.HandleInput(input);
        }

        /// <summary>
        /// Handles joystick input and logic for all players
        /// </summary>
        /// <param name="input">input state</param>
        private void HandleJoystickInput(InputState input)
        {
            float r;
            if (p1Ship.ControlEnabled)
            {
                // Player 1 Joystick
                // determine the amount of thrust TODO: migrate to player class
                if (InputState.P1Controller.joyTriggerThrust == Buttons.LeftTrigger)
                {
                    r = -input.P1CurrentGamePadState.Triggers.Left * 40000;
                    if (r == 0)
                    {
                        r = input.P1CurrentGamePadState.Triggers.Right * 40000;
                    }
                    if (p1Ship.ThrustAmount == 0)
                    {
                        p1Ship.ThrustAmount = r;
                        p1Ship.Thrust();
                    }
                }
                else
                {
                    r = -input.P1CurrentGamePadState.Triggers.Right * 40000;
                    if (r == 0)
                    {
                        r = input.P1CurrentGamePadState.Triggers.Left * 40000;
                    }
                    if (p1Ship.ThrustAmount == 0)
                    {
                        p1Ship.ThrustAmount = r;
                        p1Ship.Thrust();
                    }
                }

                // calculate the force vector
                Vector2 force = Vector2.Zero;
                force += new Vector2(r * (float)(Math.Cos(p1Ship.Body.Rotation + MathHelper.PiOver2)),
                    r * (float)Math.Sin(p1Ship.Body.Rotation + MathHelper.PiOver2));

                // apply the thrusted force vector
                p1Ship.ApplyForce(force);

                if (input.P1CurrentGamePadState.IsButtonDown(InputState.P1Controller.joyWeaponA) && input.P1LastGamePadState.IsButtonUp(InputState.P1Controller.joyWeaponA))
                {
                    if (p1BulletsToDraw.Count < p1MaxBullets)
                    {
                        Vector2 dir = Vector2.Zero;
                        dir += new Vector2(10000 * (float)(Math.Cos(p1Ship.Body.Rotation - (MathHelper.PiOver2))),
                            10000 * (float)Math.Sin(p1Ship.Body.Rotation - (MathHelper.PiOver2)));

                        Laser b = p1Bullets.Fetch();
                        if (b != null)
                        {
                            b.BulletBody.ResetDynamics();
                            b.BulletBody.Enabled = true;
                            Vector2 pos = new Vector2(p1Ship.Body.Position.X + ((float)(Math.Cos(p1Ship.Body.Rotation - MathHelper.PiOver2)) * 70),
                                p1Ship.Body.Position.Y + ((float)Math.Sin(p1Ship.Body.Rotation - (MathHelper.PiOver2))) * 70);
                            b.Fire(pos, dir);
                            b.BulletBody.LinearVelocity += p1Ship.Body.LinearVelocity;
                            b.BulletGeom.OnCollision += OnP1BulletCollision;
                            PhysicsSimulator.Add(b.BulletBody);
                            PhysicsSimulator.Add(b.BulletGeom);
                            p1BulletsToDraw.Add(b);
                        }
                    }
                }

                if (input.P1CurrentGamePadState.IsButtonDown(InputState.P1Controller.joyWeaponB)
                    && input.P1LastGamePadState.IsButtonUp(InputState.P1Controller.joyWeaponB))
                {
                    P1FireWeaponB();
                }
                if (input.P1CurrentGamePadState.IsButtonDown(InputState.P1Controller.joyWeaponC)
                    && input.P1LastGamePadState.IsButtonUp(InputState.P1Controller.joyWeaponC))
                {
                    P1FireWeaponC();
                }

                // rotation
                float rot = 0f;
                if (InputState.P1Controller.joyMove == Buttons.LeftStick)
                {
                    if (input.P1CurrentGamePadState.ThumbSticks.Left.X != 0)
                    {
                        p1Ship.Body.AngularVelocity = 0.0f;
                        p1Ship.Body.Rotation += .015f * input.P1CurrentGamePadState.ThumbSticks.Left.X;
                    }
                    rot = input.P1CurrentGamePadState.ThumbSticks.Left.X * 40000;
                }
                else
                {
                    if (input.P1CurrentGamePadState.ThumbSticks.Right.X != 0)
                    {
                        p1Ship.Body.AngularVelocity = 0.0f;
                        p1Ship.Body.Rotation += .015f * input.P1CurrentGamePadState.ThumbSticks.Right.X;
                    }
                    rot = input.P1CurrentGamePadState.ThumbSticks.Right.X * 40000;
                }
                // p1Ship.ApplyTorque(rot);

                // tractor beam TODO: migrate to player actions
                if (input.P1CurrentGamePadState.IsButtonDown(InputState.P1Controller.joyTractor) &&
                    input.P1LastGamePadState.IsButtonUp(InputState.P1Controller.joyTractor))
                {
                    if (!p1Ship.TractorActive)
                    {
                        AttachShip(true);
                        if (p1Ship.TractorActive)
                            p1Ship.TractorSound = SoundManager.PlaySound("tractorBeam");
                    }
                    else
                    {
                        p1Ship.TractorBeam.Enabled = false;
                        p1Ship.TractorActive = false;
                        PhysicsSimulator.Remove(p1Ship.TractorBeam);
                        SoundManager.StopSound(p1Ship.TractorSound);
                    }
                }
            }

            if (p2Ship.ControlEnabled)
            {
                Vector2 force = Vector2.Zero;
                float rot = 0f;
                // Player 2 Joystick
                // determine the amount of thrust TODO: migrate to player class
                if (InputState.P2Controller.joyTriggerThrust == Buttons.LeftTrigger)
                {
                    r = -input.P2CurrentGamePadState.Triggers.Left * 40000;
                    if (r == 0)
                    {
                        r = input.P2CurrentGamePadState.Triggers.Right * 40000;
                    }
                    if (p2Ship.ThrustAmount == 0)
                    {
                        p2Ship.ThrustAmount = r;
                        p2Ship.Thrust();
                    }
                }
                else
                {
                    r = -input.P2CurrentGamePadState.Triggers.Right * 40000;
                    if (r == 0)
                    {
                        r = input.P2CurrentGamePadState.Triggers.Left * 40000;
                    }
                    if (p2Ship.ThrustAmount == 0)
                    {
                        p2Ship.ThrustAmount = r;
                        p2Ship.Thrust();
                    }
                }

                // calculate the force vector
                force = Vector2.Zero;
                force += new Vector2(r * (float)(Math.Cos(p2Ship.Body.Rotation + MathHelper.PiOver2)),
                    r * (float)Math.Sin(p2Ship.Body.Rotation + MathHelper.PiOver2));

                // apply the thrusted force vector
                p2Ship.ApplyForce(force);

                // rotation
                rot = 0f;
                // rot = input.P2CurrentGamePadState.ThumbSticks.Left.X * 40000;
                if (input.P2CurrentGamePadState.ThumbSticks.Left.X != 0)
                {
                    p2Ship.Body.AngularVelocity = 0.0f;
                    p2Ship.Body.Rotation += .015f * input.P2CurrentGamePadState.ThumbSticks.Left.X;
                }
                // p2Ship.ApplyTorque(rot);

                if (input.P2CurrentGamePadState.IsButtonDown(InputState.P2Controller.joyWeaponA) && input.P2LastGamePadState.IsButtonUp(InputState.P2Controller.joyWeaponA))
                {
                    if (p2BulletsToDraw.Count < p2MaxBullets)
                    {
                        Vector2 dir = Vector2.Zero;
                        dir += new Vector2(10000 * (float)(Math.Cos(p2Ship.Body.Rotation - (MathHelper.PiOver2))),
                            10000 * (float)Math.Sin(p2Ship.Body.Rotation - (MathHelper.PiOver2)));

                        Laser b = p2Bullets.Fetch();
                        if (b != null)
                        {
                            b.BulletBody.ResetDynamics();
                            b.BulletBody.Enabled = true;
                            Vector2 pos = new Vector2(p2Ship.Body.Position.X + ((float)(Math.Cos(p2Ship.Body.Rotation - MathHelper.PiOver2)) * 70),
                                p2Ship.Body.Position.Y + ((float)Math.Sin(p2Ship.Body.Rotation - (MathHelper.PiOver2))) * 70);
                            b.Fire(pos, dir);
                            b.BulletBody.LinearVelocity += p2Ship.Body.LinearVelocity;
                            b.BulletGeom.OnCollision += OnP2BulletCollision;
                            PhysicsSimulator.Add(b.BulletBody);
                            PhysicsSimulator.Add(b.BulletGeom);
                            p2BulletsToDraw.Add(b);
                        }
                    }
                }

                if (input.P2CurrentGamePadState.IsButtonDown(InputState.P2Controller.joyWeaponB)
                    && input.P2LastGamePadState.IsButtonUp(InputState.P2Controller.joyWeaponB))
                {
                    P2FireWeaponB();
                }
                if (input.P2CurrentGamePadState.IsButtonDown(InputState.P2Controller.joyWeaponC)
                    && input.P2LastGamePadState.IsButtonUp(InputState.P2Controller.joyWeaponC))
                {
                    P2FireWeaponC();
                }

                // tractor beam TODO: migrate to player actions
                if (input.P2CurrentGamePadState.IsButtonDown(InputState.P2Controller.joyTractor)
                    && input.P2LastGamePadState.IsButtonUp(InputState.P2Controller.joyTractor))
                {
                    if (!p2Ship.TractorActive)
                    {
                        AttachShip(true);
                        if (p2Ship.TractorActive)
                            p2Ship.TractorSound = SoundManager.PlaySound("tractorBeam");
                    }
                    else
                    {
                        p2Ship.TractorBeam.Enabled = false;
                        p2Ship.TractorActive = false;
                        PhysicsSimulator.Remove(p2Ship.TractorBeam);
                        SoundManager.StopSound(p2Ship.TractorSound);
                    }
                }
            }
        }

        private bool OnP1BulletCollision(Geom g1, Geom g2, ContactList list)
        {
            Laser laser = (Laser)g1.Tag;
            g1.OnCollision -= OnP1BulletCollision;
            g1.Body.Enabled = false;

            if (g2.Name == "player2")
            {
                if (p2Ship.Fuel >= 5)
                    p2Ship.Fuel -= 5;

                // TODO: play goophit sound
                SoundManager.PlaySound("splat");
            }

            PhysicsSimulator.Remove(laser.BulletGeom);
            PhysicsSimulator.Remove(laser.BulletBody);
            p1Bullets.Insert(laser);
            p1BulletsToDraw.Remove(laser);
            return false;
        }

        private bool OnP2BulletCollision(Geom g1, Geom g2, ContactList list)
        {
            Laser laser = (Laser)g1.Tag;
            g1.OnCollision -= OnP2BulletCollision;
            g1.Body.Enabled = false;

            if (g2.Name == "player1")
            {
                if (p1Ship.Fuel >= 5)
                    p1Ship.Fuel -= 5;

                // TODO: play goophit sound
                SoundManager.PlaySound("splat");
            }
            PhysicsSimulator.Remove(laser.BulletGeom);
            PhysicsSimulator.Remove(laser.BulletBody);
            p2Bullets.Insert(laser);
            p2BulletsToDraw.Remove(laser);
            return false;
        }


        /// <summary>
        /// Attaches the closest garbage object to the p1Ship
        /// This is currently being tested.
        /// </summary>
        private void AttachShip(bool playerOne)
        {
            // object for tracking our player one p1Ship body
            Body shipBody = new Body();

            // temp object for determining what trash object
            // we are attaching
            Body trashBody = new Body();

            // first we need to find the p1Ship
            foreach (Body body in PhysicsSimulator.BodyList)
            {
                if (playerOne)
                {
                    if (body.Name == "p1Ship")
                    {
                        // set our shipBody object
                        shipBody = body;

                        // TODO: determine max range of "tractor-beam"
                        float distanceToTrash = 300f;

                        // next we need to find the closest trash object
                        foreach (Body b2 in PhysicsSimulator.BodyList)
                        {
                            if (b2.Name == "trash")
                            {
                                // determine the destance between the two objects
                                float dist = Vector2.Distance(b2.Position, body.Position);

                                // are we the closest trash object?
                                if (dist < distanceToTrash)
                                {
                                    // set local variables to this object
                                    distanceToTrash = dist;
                                    trashBody = b2;
                                }
                            }
                        }// end foreach for trash
                    }
                }
                else
                {
                    if (body.Name == "p2Ship")
                    {
                        // set our shipBody object
                        shipBody = body;

                        // TODO: determine max range of "tractor-beam"
                        float distanceToTrash = 300f;

                        // next we need to find the closest trash object
                        foreach (Body b2 in PhysicsSimulator.BodyList)
                        {
                            if (b2.Name == "trash")
                            {
                                // determine the destance between the two objects
                                float dist = Vector2.Distance(b2.Position, body.Position);

                                // are we the closest trash object?
                                if (dist < distanceToTrash)
                                {
                                    // set local variables to this object
                                    distanceToTrash = dist;
                                    trashBody = b2;
                                }
                            }
                        }// end foreach for trash
                    }
                }
            }// end foreach for p1Ship

            // if we found a p1Ship
            if (shipBody.Name != "" && trashBody.Position != Vector2.Zero)
            {
                if (playerOne)
                {
                    p1Ship.TractorBeam = SpringFactory.Instance.CreateLinearSpring(PhysicsSimulator,
                        shipBody, Vector2.Zero, trashBody, Vector2.Zero, 10.0f, 10.0f);
                    p1Ship.TracBeamObj.PointA = p1Ship.TractorBeam.Body1.Position;
                    p1Ship.TracBeamObj.PointB = p1Ship.TractorBeam.Body2.Position;
                    p1Ship.TractorActive = true;
                    p1Ship.TractorBeam.Broke += new EventHandler<EventArgs>(tractorBeam_Broke);
                    p1Ship.TractorBeam.Breakpoint = 301.0f;
                    p1Ship.TractorBeam.RestLength = 80.0f;
                }
                else
                {
                    p2Ship.TractorBeam = SpringFactory.Instance.CreateLinearSpring(PhysicsSimulator,
                        shipBody, Vector2.Zero, trashBody, Vector2.Zero, 10.0f, 10.0f);
                    p2Ship.TracBeamObj.PointA = p2Ship.TractorBeam.Body1.Position;
                    p2Ship.TracBeamObj.PointB = p2Ship.TractorBeam.Body2.Position;
                    p2Ship.TractorActive = true;
                    p2Ship.TractorBeam.Broke += new EventHandler<EventArgs>(tractorBeam_Broke);
                    p2Ship.TractorBeam.Breakpoint = 301.0f;
                    p2Ship.TractorBeam.RestLength = 80.0f;
                }
            }
        }

        void tractorBeam_Broke(object sender, EventArgs e)
        {
            if (((LinearSpring)sender).Body1.Tag == p1Ship.Body.Tag)
            {
                p1Ship.TractorActive = false;
            }
            else
            {
                p2Ship.TractorActive = false;
            }
            PhysicsSimulator.Remove((LinearSpring)sender);
        }

        /// <summary>
        /// Handle the keyboard input for both players
        /// </summary>
        /// <param name="input"></param>
        private void HandleKeyboardInput(InputState input)
        {
            // force amounts shoule be moved to the p1Ship class as we tweak them
            const float r = 40000;
            const float rotAmount = 60000;

            // vector for calculating the force to apply to the p1Ship
            Vector2 force = Vector2.Zero;
            force.Y = -force.Y;
            float rot = 0f;

            if (p1Ship.ControlEnabled)
            {
                // Player 1
                // tractor-beam enable TODO: migrate to control actions
                if (input.CurrentKeyboardState.IsKeyDown(InputState.P1Controller.keyTractor)
                    && input.LastKeyboardState.IsKeyUp(InputState.P1Controller.keyTractor))
                {
                    if (!p1Ship.TractorActive)
                    {
                        AttachShip(true);
                        if (p1Ship.TractorActive)
                            p1Ship.TractorSound = SoundManager.PlaySound("tractorBeam");
                    }
                    else
                    {
                        p1Ship.TractorBeam.Enabled = false;
                        p1Ship.TractorActive = false;
                        PhysicsSimulator.Remove(p1Ship.TractorBeam);
                        SoundManager.StopSound(p1Ship.TractorSound);
                    }
                }

                // rotate left TODO: migrate to control actions
                if (input.CurrentKeyboardState.IsKeyDown(InputState.P1Controller.keyLeft))
                {
                    p1Ship.Body.AngularVelocity = 0.0f;
                    p1Ship.Body.Rotation -= rotAmt;
                    rot -= rotAmount;
                }

                // rotate right TODO: migrate to control actions
                if (input.CurrentKeyboardState.IsKeyDown(InputState.P1Controller.keyRight))
                {
                    p1Ship.Body.AngularVelocity = 0.0f;
                    p1Ship.Body.Rotation += rotAmt;
                    rot += rotAmount;
                }

                // Thrust TODO: migrate to control actions
                p1Ship.ThrustAmount = 0;
                if (input.CurrentKeyboardState.IsKeyDown(InputState.P1Controller.keyThrust))
                {
                    p1Ship.ThrustAmount = -r;
                    p1Ship.Thrust();
                    // calculate the force vector
                    force = Vector2.Zero;
                    force += new Vector2(p1Ship.ThrustAmount * (float)(Math.Cos(p1Ship.Body.Rotation + MathHelper.PiOver2)),
                        p1Ship.ThrustAmount * (float)Math.Sin(p1Ship.Body.Rotation + MathHelper.PiOver2));
                }

                if (input.CurrentKeyboardState.IsKeyDown(InputState.P1Controller.keyReverse))
                {
                    p1Ship.ThrustAmount = r;
                    p1Ship.Thrust();
                    // calculate the force vector
                    force = Vector2.Zero;
                    force += new Vector2(p1Ship.ThrustAmount * (float)(Math.Cos(p1Ship.Body.Rotation + MathHelper.PiOver2)),
                        p1Ship.ThrustAmount * (float)Math.Sin(p1Ship.Body.Rotation + MathHelper.PiOver2));
                }

                // apply the calculated forces for player 1
                p1Ship.ApplyForce(force);
                // p1Ship.ApplyTorque(rot);

                if (input.CurrentKeyboardState.IsKeyDown(InputState.P1Controller.keyWeaponB)
                    && input.LastKeyboardState.IsKeyUp(InputState.P1Controller.keyWeaponB))
                {
                    P1FireWeaponB();
                }
                if (input.CurrentKeyboardState.IsKeyDown(InputState.P1Controller.keyWeaponC)
                    && input.LastKeyboardState.IsKeyUp(InputState.P1Controller.keyWeaponC))
                {
                    P1FireWeaponC();
                }
            }

            if (p2Ship.ControlEnabled)
            {
                /////////////////
                // player 2    //
                /////////////////
                force = Vector2.Zero;
                rot = 0.0f;

                if (input.CurrentKeyboardState.IsKeyDown(InputState.P1Controller.keyWeaponA) &&
                    input.LastKeyboardState.IsKeyUp(InputState.P1Controller.keyWeaponA))
                {
                    if (p1BulletsToDraw.Count < p1MaxBullets)
                    {
                        Vector2 dir = Vector2.Zero;
                        dir += new Vector2(10000 * (float)(Math.Cos(p1Ship.Body.Rotation - (MathHelper.PiOver2))),
                            10000 * (float)Math.Sin(p1Ship.Body.Rotation - (MathHelper.PiOver2)));

                        Laser b = p1Bullets.Fetch();
                        if (b != null)
                        {
                            b.BulletBody.ResetDynamics();
                            b.BulletBody.Enabled = true;
                            Vector2 pos = new Vector2(p1Ship.Body.Position.X + ((float)(Math.Cos(p1Ship.Body.Rotation - MathHelper.PiOver2)) * 70),
                                p1Ship.Body.Position.Y + ((float)Math.Sin(p1Ship.Body.Rotation - (MathHelper.PiOver2))) * 70);
                            b.Fire(pos, dir);
                            b.BulletBody.LinearVelocity += p1Ship.Body.LinearVelocity;
                            b.BulletGeom.OnCollision += OnP1BulletCollision;
                            PhysicsSimulator.Add(b.BulletBody);
                            PhysicsSimulator.Add(b.BulletGeom);
                            p1BulletsToDraw.Add(b);
                        }
                    }
                }

                // tractor-beam enable TODO: migrate to control actions
                if (input.CurrentKeyboardState.IsKeyDown(InputState.P2Controller.keyTractor) &&
                    input.LastKeyboardState.IsKeyUp(InputState.P2Controller.keyTractor))
                {
                    if (!p2Ship.TractorActive)
                    {
                        AttachShip(false);
                        if (p2Ship.TractorActive)
                            p2Ship.TractorSound = SoundManager.PlaySound("tractorBeam");
                    }
                    else
                    {
                        p2Ship.TractorActive = false;
                        p2Ship.TractorBeam.Enabled = false;
                        PhysicsSimulator.Remove(p2Ship.TractorBeam);
                        SoundManager.StopSound(p2Ship.TractorSound);
                    }
                }

                // rotate left TODO: migrate to control actions
                if (input.CurrentKeyboardState.IsKeyDown(InputState.P2Controller.keyLeft))
                {
                    p2Ship.Body.AngularVelocity = 0.0f;
                    p2Ship.Body.Rotation -= rotAmt;
                    rot -= rotAmount;
                }

                // rotate right TODO: migrate to control actions
                if (input.CurrentKeyboardState.IsKeyDown(InputState.P2Controller.keyRight))
                {
                    p2Ship.Body.AngularVelocity = 0.0f;
                    p2Ship.Body.Rotation += rotAmt;
                    rot += rotAmount;
                }

                // Thrust TODO: migrate to control actions
                p2Ship.ThrustAmount = 0;
                if (input.CurrentKeyboardState.IsKeyDown(InputState.P2Controller.keyThrust))
                {
                    p2Ship.ThrustAmount = -r;
                    p2Ship.Thrust();
                    // calculate the force vector
                    force = Vector2.Zero;
                    force += new Vector2(p2Ship.ThrustAmount * (float)(Math.Cos(p2Ship.Body.Rotation + MathHelper.PiOver2)),
                        p2Ship.ThrustAmount * (float)Math.Sin(p2Ship.Body.Rotation + MathHelper.PiOver2));
                }

                if (input.CurrentKeyboardState.IsKeyDown(InputState.P2Controller.keyReverse))
                {
                    p2Ship.ThrustAmount = r;
                    p2Ship.Thrust();
                    force = Vector2.Zero;
                    force += new Vector2(p2Ship.ThrustAmount * (float)(Math.Cos(p2Ship.Body.Rotation + MathHelper.PiOver2)),
                        p2Ship.ThrustAmount * (float)Math.Sin(p2Ship.Body.Rotation + MathHelper.PiOver2));
                }


                // apply the calculated forces for player 1
                p2Ship.ApplyForce(force);
                // p2Ship.ApplyTorque(rot);

                if (input.CurrentKeyboardState.IsKeyDown(InputState.P2Controller.keyWeaponB)
                    && input.LastKeyboardState.IsKeyUp(InputState.P2Controller.keyWeaponB))
                {
                    P2FireWeaponB();
                }
                if (input.CurrentKeyboardState.IsKeyDown(InputState.P2Controller.keyWeaponC)
                    && input.LastKeyboardState.IsKeyUp(InputState.P2Controller.keyWeaponC))
                {
                    P2FireWeaponC();
                }

                if (input.CurrentKeyboardState.IsKeyDown(InputState.P2Controller.keyWeaponA) &&
                    input.LastKeyboardState.IsKeyUp(InputState.P2Controller.keyWeaponA))
                {
                    if (p2BulletsToDraw.Count < p2MaxBullets)
                    {
                        Vector2 dir = Vector2.Zero;
                        dir += new Vector2(10000 * (float)(Math.Cos(p2Ship.Body.Rotation - (MathHelper.PiOver2))),
                            10000 * (float)Math.Sin(p2Ship.Body.Rotation - (MathHelper.PiOver2)));

                        Laser b = p2Bullets.Fetch();
                        if (b != null)
                        {
                            b.BulletBody.ResetDynamics();
                            b.BulletBody.Enabled = true;
                            Vector2 pos = new Vector2(p2Ship.Body.Position.X + ((float)(Math.Cos(p2Ship.Body.Rotation - MathHelper.PiOver2)) * 70),
                                p2Ship.Body.Position.Y + ((float)Math.Sin(p2Ship.Body.Rotation - (MathHelper.PiOver2))) * 70);
                            b.Fire(pos, dir);
                            b.BulletBody.LinearVelocity += p2Ship.Body.LinearVelocity;
                            b.BulletGeom.OnCollision += OnP2BulletCollision;
                            PhysicsSimulator.Add(b.BulletBody);
                            PhysicsSimulator.Add(b.BulletGeom);
                            p2BulletsToDraw.Add(b);
                        }
                    }
                }
            }
        }

        private void P2FireWeaponB()
        {
            switch (p2Ship.WeaponB)
            {
                case WeaponList.BlackHole:
                    if (p2Ship.WeaponBFired == false)
                    {
                        BlackHole b = bholeWeapon.Fetch();
                        bool remove = b.Fire(p2Ship, PhysicsSimulator);
                        p2Ship.WeaponBFired = true;
                        bholeToDraw.Add(b);
                        p2Ship.BlackHoleList.Add(b);
                    }
                    else
                    {
                        bool remove = p2Ship.BlackHoleList[0].Fire(p2Ship, PhysicsSimulator);
                        if (remove)
                            p2Ship.BlackHoleList.RemoveAt(0);
                        p2Ship.WeaponBFired = false;
                        p2Ship.WeaponB = WeaponList.None;
                    }
                    break;
                case WeaponList.ConcussionGrenade:
                    if (p2Ship.WeaponBFired == false)
                    {
                        ConcussionGrenade c = conGrenWeapon.Fetch();
                        bool remove = c.Fire(p2Ship, PhysicsSimulator);
                        p2Ship.WeaponBFired = true;
                        conGrenToDraw.Add(c);
                        p2Ship.WeaponBFired = false;
                        p2Ship.WeaponB = WeaponList.None;
                    }
                    break;
                case WeaponList.EMPWeapon:
                    if (p2Ship.WeaponBFired == false)
                    {
                        EmpWeapon b = empWeapon.Fetch();
                        bool remove = b.Fire(p2Ship, PhysicsSimulator, p1Ship);
                        p2Ship.WeaponBFired = true;
                        empToDraw.Add(b);
                        p2Ship.EmpList.Add(b);
                    }
                    else
                    {
                        bool remove = p2Ship.EmpList[0].Fire(p2Ship, PhysicsSimulator, p1Ship);
                        if (remove)
                            p2Ship.EmpList.RemoveAt(0);
                        p2Ship.WeaponBFired = false;
                        p2Ship.WeaponB = WeaponList.None;
                    }
                    break;
            }
        }

        private void P2FireWeaponC()
        {
            switch (p2Ship.WeaponC)
            {
                case WeaponList.BlackHole:
                    if (p2Ship.WeaponCFired == false)
                    {
                        BlackHole b = bholeWeapon.Fetch();
                        bool remove = b.Fire(p2Ship, PhysicsSimulator);
                        p2Ship.WeaponCFired = true;
                        bholeToDraw.Add(b);
                        p2Ship.BlackHoleList.Add(b);
                    }
                    else
                    {
                        bool remove = p2Ship.BlackHoleList[0].Fire(p2Ship, PhysicsSimulator);
                        if (remove)
                            p2Ship.BlackHoleList.RemoveAt(0);
                        p2Ship.WeaponCFired = false;
                        p2Ship.WeaponC = WeaponList.None;
                    }
                    break;
                case WeaponList.ConcussionGrenade:
                    if (p2Ship.WeaponCFired == false)
                    {
                        ConcussionGrenade c = conGrenWeapon.Fetch();
                        bool remove = c.Fire(p2Ship, PhysicsSimulator);
                        p2Ship.WeaponCFired = true;
                        conGrenToDraw.Add(c);
                        p2Ship.WeaponCFired = false;
                        p2Ship.WeaponC = WeaponList.None;
                    }
                    break;
                case WeaponList.EMPWeapon:
                    if (p2Ship.WeaponCFired == false)
                    {
                        EmpWeapon b = empWeapon.Fetch();
                        bool remove = b.Fire(p2Ship, PhysicsSimulator, p1Ship);
                        p2Ship.WeaponCFired = true;
                        empToDraw.Add(b);
                        p2Ship.EmpList.Add(b);
                    }
                    else
                    {
                        bool remove = p2Ship.EmpList[0].Fire(p2Ship, PhysicsSimulator, p1Ship);
                        if (remove)
                            p2Ship.EmpList.RemoveAt(0);
                        p2Ship.WeaponCFired = false;
                        p2Ship.WeaponC = WeaponList.None;
                    }
                    break;
            }
        }

        private void P1FireWeaponC()
        {
            switch (p1Ship.WeaponC)
            {
                case WeaponList.BlackHole:
                    if (p1Ship.WeaponCFired == false)
                    {
                        BlackHole b = bholeWeapon.Fetch();
                        bool remove = b.Fire(p1Ship, PhysicsSimulator);
                        p1Ship.WeaponCFired = true;
                        bholeToDraw.Add(b);
                        p1Ship.BlackHoleList.Add(b);
                    }
                    else
                    {
                        bool remove = p1Ship.BlackHoleList[0].Fire(p1Ship, PhysicsSimulator);
                        if (remove)
                            p1Ship.BlackHoleList.RemoveAt(0);
                        p1Ship.WeaponCFired = false;
                        p1Ship.WeaponC = WeaponList.None;
                    }
                    break;
                case WeaponList.ConcussionGrenade:
                    if (p1Ship.WeaponCFired == false)
                    {
                        ConcussionGrenade c = conGrenWeapon.Fetch();
                        bool remove = c.Fire(p1Ship, PhysicsSimulator);
                        p1Ship.WeaponCFired = true;
                        conGrenToDraw.Add(c);
                        p1Ship.WeaponCFired = false;
                        p1Ship.WeaponC = WeaponList.None;
                    }
                    break;
                case WeaponList.EMPWeapon:
                    if (p1Ship.WeaponCFired == false)
                    {
                        EmpWeapon b = empWeapon.Fetch();
                        bool remove = b.Fire(p1Ship, PhysicsSimulator, p2Ship);
                        p1Ship.WeaponCFired = true;
                        empToDraw.Add(b);
                        p1Ship.EmpList.Add(b);
                    }
                    else
                    {
                        bool remove = p1Ship.EmpList[0].Fire(p1Ship, PhysicsSimulator, p2Ship);
                        if (remove)
                            p1Ship.EmpList.RemoveAt(0);
                        p1Ship.WeaponCFired = false;
                        p1Ship.WeaponC = WeaponList.None;
                    }
                    break;
            }
        }
        private void P1FireWeaponB()
        {
            switch (p1Ship.WeaponB)
            {
                case WeaponList.BlackHole:
                    if (p1Ship.WeaponBFired == false)
                    {
                        BlackHole b = bholeWeapon.Fetch();
                        bool remove = b.Fire(p1Ship, PhysicsSimulator);
                        p1Ship.WeaponBFired = true;
                        bholeToDraw.Add(b);
                        p1Ship.BlackHoleList.Add(b);
                    }
                    else
                    {
                        bool remove = p1Ship.BlackHoleList[0].Fire(p1Ship, PhysicsSimulator);
                        if (remove)
                            p1Ship.BlackHoleList.RemoveAt(0);
                        p1Ship.WeaponBFired = false;
                        p1Ship.WeaponB = WeaponList.None;
                    }
                    break;
                case WeaponList.ConcussionGrenade:
                    if (p1Ship.WeaponBFired == false)
                    {
                        ConcussionGrenade c = conGrenWeapon.Fetch();
                        bool remove = c.Fire(p1Ship, PhysicsSimulator);
                        p1Ship.WeaponBFired = true;
                        conGrenToDraw.Add(c);
                        p1Ship.WeaponBFired = false;
                        p1Ship.WeaponB = WeaponList.None;
                    }
                    break;
                case WeaponList.EMPWeapon:
                    if (p1Ship.WeaponBFired == false)
                    {
                        EmpWeapon b = empWeapon.Fetch();
                        bool remove = b.Fire(p1Ship, PhysicsSimulator, p2Ship);
                        p1Ship.WeaponBFired = true;
                        empToDraw.Add(b);
                        p1Ship.EmpList.Add(b);
                    }
                    else
                    {
                        bool remove = p1Ship.EmpList[0].Fire(p1Ship, PhysicsSimulator, p2Ship);
                        if (remove)
                            p1Ship.EmpList.RemoveAt(0);
                        p1Ship.WeaponBFired = false;
                        p1Ship.WeaponB = WeaponList.None;
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the level title. This can be used for various things
        /// </summary>
        /// <returns>String representing the title of the level</returns>
        public static string GetTitle()
        {
            return "Level 1";
        }

        /// <summary>
        /// Gets the details describing the level. Good for pause screens, etc.
        /// </summary>
        /// <returns>String representing the level's description.</returns>
        public static string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Level 1 information");
            sb.AppendLine("P1 Controls:");
            sb.AppendLine("Thrust: " + InputState.P1Controller.keyThrust.ToString());
            sb.AppendLine("Reverse: " + InputState.P1Controller.keyReverse.ToString());
            sb.AppendLine("Fire: " + InputState.P1Controller.keyWeaponA.ToString());
            sb.AppendLine("Tractor: " + InputState.P1Controller.keyTractor.ToString());
            sb.AppendLine("");
            sb.AppendLine("P2 Controls:");
            sb.AppendLine("Thrust: " + InputState.P2Controller.keyThrust.ToString());
            sb.AppendLine("Reverse: " + InputState.P2Controller.keyReverse.ToString());
            sb.AppendLine("Fire: " + InputState.P2Controller.keyWeaponA.ToString());
            sb.AppendLine("Tractor: " + InputState.P2Controller.keyTractor.ToString());

            return sb.ToString();
        }
    }
}
