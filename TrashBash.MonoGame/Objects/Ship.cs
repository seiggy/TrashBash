using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpriteSheetRuntime;
using System.Collections.Generic;
using TrashBash.MonoGame.Objects.Weapons;

namespace TrashBash.MonoGame.Objects
{
    public class Ship
    {
        private Body shipBody;
        private Vector2 shipOrigin;
        private Texture2D shipTexture;
        private Shape shipGeom;
        private Category collidesWith = Category.All;
        private Category collisionCategory = Category.All;
        private Vector2 position;
        private bool tractorActive = false;
        private bool thrustActive = false;
        private TractorBeam tractorBeam;
        private const uint fuelDrain = 1;
        private uint score = 0;
        private uint fuel = 0;
        private bool collide = false;
        private float maxVelocityX = 600f;
        private float maxVelocityY = 600f;
        private float maxRotVelcoty = 3.0f;

        private WeaponList weaponA = WeaponList.None;
        private WeaponList weaponB = WeaponList.None;
        private WeaponList weaponC = WeaponList.None;

        private Vector2 thrustOrigin;
        private Vector2 thrustPosition;

        public List<BlackHole> BlackHoleList = new List<BlackHole>();
        public List<ConcussionGrenade> ConGrenList = new List<ConcussionGrenade>();
        public List<EmpWeapon> EmpList = new List<EmpWeapon>();

        public bool WeaponBFired = false;
        public bool WeaponCFired = false;

        public FarseerPhysics.Dynamics.Joints.RopeJoint TractorBeam;

        public Cue ThrustSound;
        public Cue TractorSound;

        public float ThrustAmount;

        public SpriteSheet thrust1;
        public SpriteSheet thrust2;
        public SpriteSheet thrust3;

        public SpriteSheet empHit;
        public SpriteSheet tbeamEffect;

        int timer, activeTimer = 0;

        int empFrame = 0;
        int tbeamFrame = 0;

        public bool ControlEnabled = true;

        public uint Score
        {
            get { return this.score; }
            set { this.score = value; }
        }

        public uint Fuel
        {
            get { return this.fuel; }
            set { this.fuel = value; }
        }

        public WeaponList WeaponA
        {
            get { return this.weaponA; }
            set { this.weaponA = value; }
        }

        public WeaponList WeaponB
        {
            get { return this.weaponB; }
            set { this.weaponB = value; }
        }

        public WeaponList WeaponC
        {
            get { return this.weaponC; }
            set { this.weaponC = value; }
        }

        public bool ThrustActive
        {
            get { return this.thrustActive; }
            set { this.thrustActive = value; }
        }

        public TractorBeam TracBeamObj
        {
            get { return this.tractorBeam; }
            set { this.tractorBeam = value; }
        }

        public bool TractorActive
        {
            get { return this.tractorActive; }
            set { this.tractorActive = value; }
        }

        public Ship(Vector2 position)
        {
            this.position = position;
        }

        public Body Body
        {
            get { return this.shipBody; }
        }

        public Shape Geom
        {
            get { return this.shipGeom; }
            set { this.shipGeom = value; }
        }

        public Category CollisionCategory
        {
            get { return this.collisionCategory; }
            set { this.collisionCategory = value; }
        }

        public Category CollidesWith
        {
            get { return this.collidesWith; }
            set { this.collidesWith = value; }
        }

        public void ApplyForce(Vector2 force)
        {
            if (fuel > 0)
            {
                shipBody.ApplyForce(force);
            }
            else
            {
                shipBody.ApplyForce(new Vector2(force.X / 2, force.Y / 2));
            }
        }

        public void ApplyTorque(float torque)
        {
            shipBody.ApplyTorque(torque);
        }

        public void Load(GraphicsDevice device, ContentManager content, PhysicsSimulator simulator, string filename)
        {
            shipTexture = content.Load<Texture2D>("Content/Ships/" + filename);
            thrust1 = content.Load<SpriteSheet>("Content/Ships/Thrust1");
            thrust2 = content.Load<SpriteSheet>("Content/Ships/Thrust2");
            thrust3 = content.Load<SpriteSheet>("Content/Ships/Thrust3");
            empHit = content.Load<SpriteSheet>("Content/Weapons/EMPEffect/EMPHitEffect");
            tbeamEffect = content.Load<SpriteSheet>("Content/Weapons/TBeamEffect");

            // change this if the thrust textures change
            thrustOrigin = new Vector2(32, 0);

            tractorBeam = new TractorBeam(Vector2.Zero, Vector2.Zero);
            tractorBeam.Load(device, content);

            uint[] data = new uint[shipTexture.Width * shipTexture.Height];
            shipTexture.GetData(data);

            Vertices verts = Vertices.CreatePolygon(data, shipTexture.Width, shipTexture.Height);
            shipOrigin = verts.GetCentroid();
            verts.SubDivideEdges(8);

            shipBody = BodyFactory.Instance.CreatePolygonBody(simulator, verts, 50);
            shipBody.Position = position;
            // shipBody.RotationalDragCoefficient = 100000000f;
            shipBody.IsAutoIdle = false;

            shipGeom = GeomFactory.Instance.CreatePolygonGeom(simulator, shipBody, verts, 0);

            shipGeom.OnCollision += new Geom.CollisionEventHandler(OnCollide);
            shipGeom.OnSeparation += new Geom.SeparationEventHandler(OnSeperate);

            shipGeom.RestitutionCoefficient = 0.4f;
            shipGeom.FrictionCoefficient = 0.2f;
            shipGeom.CollisionGroup = 0;
            shipGeom.CollisionCategories = collisionCategory;
            shipGeom.CollidesWith = collidesWith;

            fuel = 100;
            score = 0;
        }

        List<Geom> collidingWith = new List<Geom>();

        public bool OnCollide(Geom g1, Geom g2, ContactList contactList)
        {
            if (g2.Name != this.shipGeom.Name && !collidingWith.Contains(g2) && g2.Name != "powerup")
            {
                if (this.shipBody.LinearVelocity.X > 4 || this.shipBody.LinearVelocity.X < -4)
                    SoundManager.PlaySound("ship_crash");
                collidingWith.Add(g2);
            }
            return true;
        }

        public void OnSeperate(Geom g1, Geom g2)
        {

        }

        uint thrustTime = 0;

        private void LimitSpeed(int multiplier)
        {
            if (shipBody.LinearVelocity.X > maxVelocityX / multiplier)
            {
                shipBody.LinearVelocity.X = maxVelocityX / multiplier;
            }
            if (shipBody.LinearVelocity.Y > maxVelocityY / multiplier)
            {
                shipBody.LinearVelocity.Y = maxVelocityY / multiplier;
            }
            if (shipBody.LinearVelocity.X < -maxVelocityX / multiplier)
            {
                shipBody.LinearVelocity.X = -maxVelocityX / multiplier;
            }
            if (shipBody.LinearVelocity.Y < -maxVelocityY / multiplier)
            {
                shipBody.LinearVelocity.Y = -maxVelocityY / multiplier;
            }
            if (shipBody.AngularVelocity > maxRotVelcoty / multiplier)
            {
                shipBody.AngularVelocity = maxRotVelcoty / multiplier;
            }
            if (shipBody.AngularVelocity < -maxRotVelcoty / multiplier)
            {
                shipBody.AngularVelocity = -maxRotVelcoty / multiplier;
            }
        }

        public void Update(PhysicsSimulator physicsSim, GameTime gameTime)
        {
            if (this.fuel > 0)
            {
                LimitSpeed(1);
            }
            else
            {
                LimitSpeed(2);
            }

            if (this.fuel > 200)
            {
                this.fuel = 0;
            }

            if (thrustActive)
            {
                //timer += gameTime.ElapsedGameTime.Milliseconds;

                if (activeTimer < 30)
                    activeTimer++;
                if (activeTimer == 30)
                    activeTimer = 0;

                thrustTime += (uint)gameTime.ElapsedGameTime.Milliseconds;
                if (thrustTime / 500 > 1 && fuel > 0)
                {
                    fuel -= 1;
                    thrustTime -= 500;
                }
            }
            else
            {
                thrustTime = 0;
            }
            if (physicsSim.ArbiterList.Count == 0)
            {
                collidingWith.Clear();
            }
            Geom[] colTemp = new Geom[collidingWith.Count];
            collidingWith.CopyTo(colTemp);
            collidingWith.Clear();
            foreach (Arbiter arbiter in physicsSim.ArbiterList)
            {
                if (arbiter.GeomA == this.shipGeom)
                {
                    if (colTemp.Contains(arbiter.GeomB))
                    {
                        collidingWith.Add(arbiter.GeomB);
                    }
                }
                if (arbiter.GeomB == this.shipGeom)
                {
                    if (colTemp.Contains(arbiter.GeomA))
                    {
                        collidingWith.Add(arbiter.GeomA);
                    }
                }
            }

            if (!TractorActive)
            {
                if (TractorSound != null)
                {
                    if (TractorSound.IsPlaying)
                    {
                        TractorSound.Stop(AudioStopOptions.Immediate);
                    }
                }
            }
        }

        public void Thrust()
        {
            if (ThrustAmount != 0)
            {
                if (!thrustActive)
                {
                    if (ThrustSound != null)
                    {
                        if (!ThrustSound.IsPlaying)
                            ThrustSound = SoundManager.PlaySound("thrust");
                    }
                    else
                    {
                        ThrustSound = SoundManager.PlaySound("thrust");
                    }
                    thrustActive = true;
                }
            }
            else
            {
                if (thrustActive)
                {
                    ThrustSound.Stop(AudioStopOptions.Immediate);
                    thrustActive = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (tractorActive)
                tractorBeam.Draw(spriteBatch);
            spriteBatch.Draw(shipTexture, shipBody.Position, null, Color.White,
                shipBody.Rotation, shipOrigin, 1, SpriteEffects.None, 0);

            if (ThrustAmount < 0)
            {
                thrustPosition = new Vector2(Body.Position.X -
                    ((float)(Math.Cos(Body.Rotation - MathHelper.PiOver2)) * 35),
                    Body.Position.Y - ((float)Math.Sin(Body.Rotation -
                    (MathHelper.PiOver2))) * 35);

                if (ThrustAmount < -3900 && fuel > 0)
                {
                    spriteBatch.Draw(thrust3.Texture, thrustPosition, thrust3.SourceRectangle(activeTimer),
                        Color.White, shipBody.Rotation, thrustOrigin, 1.0f, SpriteEffects.None, 0);
                }
                else
                {
                    if (ThrustAmount < -1300)
                    {
                        spriteBatch.Draw(thrust2.Texture, thrustPosition, thrust2.SourceRectangle(activeTimer),
                            Color.White, shipBody.Rotation, thrustOrigin, 1.0f, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(thrust1.Texture, thrustPosition, thrust1.SourceRectangle(activeTimer),
                            Color.White, shipBody.Rotation, thrustOrigin, 1.0f, SpriteEffects.None, 0);
                    }
                }
            }

            if (!ControlEnabled)
            {
                spriteBatch.Draw(empHit.Texture, shipBody.Position, empHit.SourceRectangle(empFrame),
                    Color.White, shipBody.Rotation, new Vector2(128, 128), 1.0f, SpriteEffects.None, 0);
                empFrame++;
                if (empFrame == 26)
                    empFrame = 0;
            }
            if (TractorActive)
            {
                spriteBatch.Draw(tbeamEffect.Texture, tractorBeam.PointB, tbeamEffect.SourceRectangle(tbeamFrame),
                    Color.White, 0.0f, new Vector2(64, 64), 1.0f, SpriteEffects.None, 0);
                tbeamFrame++;
                if (tbeamFrame == 56)
                    tbeamFrame = 0;
            }
        }
    }
}