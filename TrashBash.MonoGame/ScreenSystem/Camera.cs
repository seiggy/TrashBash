using Microsoft.Xna.Framework;

namespace TrashBash.MonoGame.ScreenSystem
{
    /// <summary>
    /// Helper object for creating a translation matrix
    /// for rendering splitscreen cameras
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// rotation of the camera
        /// </summary>
        protected float rotation;

        /// <summary>
        /// zoom or scale of the camera
        /// </summary>
        protected float scale;

        /// <summary>
        /// world position of the camera
        /// </summary>
        protected Vector2 position;

        /// <summary>
        /// offset from center
        /// </summary>
        protected Vector2 offset;

        /// <summary>
        /// Gets or sets the rotation of the camera
        /// </summary>
        public float Rotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        /// <summary>
        /// Gets or sets the zoom/scale of the camera (1.0f = normal)
        /// </summary>
        public float Zoom
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        /// <summary>
        /// Gets or sets the camera's position in world coordinates
        /// </summary>
        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public Vector2 Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }

        /// <summary>
        /// Gets the translation matrix for the SpriteBatch.Begin() method
        /// </summary>
        public Matrix Translate
        {
            get
            {
                Vector3 matrixRotOrigin = new Vector3(position, 0);
                return Matrix.CreateTranslation(-matrixRotOrigin)
                    * Matrix.CreateScale(new Vector3((scale * scale * scale),
                        (scale * scale * scale), 0))
                    * Matrix.CreateRotationZ(rotation)
                    * Matrix.CreateTranslation(new Vector3(
                        TrashBash.graphics.GraphicsDevice.Viewport.Width / 2,
                        TrashBash.graphics.GraphicsDevice.Viewport.Height / 2, 0)
                    );
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Camera()
        {
            this.position = Vector2.Zero;
            this.rotation = 0.0f;
            this.scale = 1.0f;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">position of the camera in world coordinates</param>
        public Camera(Vector2 position)
        {
            this.position = position;
            this.rotation = 0.0f;
            this.scale = 1.0f;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">position of the camera in world coordinates</param>
        /// <param name="zoom">camera zoom value (1.0f is default)</param>
        public Camera(Vector2 position, float zoom)
        {
            this.position = position;
            this.rotation = 0.0f;
            this.scale = zoom;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">position of the camera in world coordinates</param>
        /// <param name="rotation">rotation of the camera in radians</param>
        /// <param name="zoom">zoom value for camera (1.0f is default)</param>
        public Camera(Vector2 position, float rotation, float zoom)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = zoom;
        }
    }
}
