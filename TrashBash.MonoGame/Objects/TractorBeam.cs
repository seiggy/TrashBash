using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashBash.MonoGame.Objects
{
    public class TractorBeam
    {
        Texture2D texture;
        SpriteSheet animation;
        Vector2 pointa;
        Vector2 pointb;
        Rectangle size;
        int activeTimer = 0;

        public TractorBeam(Vector2 pointa, Vector2 pointb)
        {
            this.pointa = pointa;
            this.pointb = pointb;
        }

        public Vector2 PointA
        {
            get { return this.pointa; }
            set { this.pointa = value; }
        }

        public Vector2 PointB
        {
            get { return this.pointb; }
            set { this.pointb = value; }
        }

        public void Update()
        {
            size = new Rectangle((int)pointa.X, (int)pointa.Y, (int)Vector2.Distance(pointa, pointb), 128);
            activeTimer++;
            if (activeTimer == 54)
                activeTimer = 0;
        }

        public void Load(GraphicsDevice graphics, ContentManager content)
        {
            texture = content.Load<Texture2D>("Content/Weapons/TractorBeamOld");
            animation = content.Load<SpriteSheet>("Content/Weapons/TractorBeam");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float rotation = (float)Math.Atan2(pointb.Y - pointa.Y, pointb.X - pointa.X) - MathHelper.PiOver2;
            Vector2 scale = new Vector2(1.0f, Vector2.Distance(pointa, pointb) / 256);
            // spriteBatch.Draw(texture, pointa, null, Color.White, rotation, new Vector2(0, texture.Height / 2), scale, SpriteEffects.None, 0);
            spriteBatch.Draw(animation.Texture, pointa, animation.SourceRectangle(activeTimer), Color.White, rotation, new Vector2(64, 0), scale, SpriteEffects.None, 0);
        }
    }
}
