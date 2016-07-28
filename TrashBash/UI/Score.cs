using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrashBash.ScreenSystem;

namespace TrashBash.UI
{
    class Score
    {
        private Texture2D texture;
        private Vector2 position;
        private string text = "000000";

        public Score()
        {

        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public void LoadContent(ScreenManager screenManager)
        {
            texture = screenManager.ContentManager.Load<Texture2D>("Content/UI/numbers");
        }

        public void DrawScore(ScreenManager screenManager, int score)
        {
            string s = score.ToString();
            char[] c = new char[text.Length];
            s.CopyTo(0, c, text.Length - s.Length, s.Length);
            for (int i = 0; i < text.Length; i++)
            {
                Vector2 pos = new Vector2(position.X + ((texture.Width / 10) * i), position.Y);
                screenManager.SpriteBatch.Draw(texture, pos, GetSourceRectangle(c[i]), Color.White);
            }
        }

        private Rectangle GetSourceRectangle(char p)
        {
            int x = 0;
            switch (p)
            {
                case '0':
                    x = 0;
                    break;
                case '1':
                    x = texture.Width / 10;
                    break;
                case '2':
                    x = (texture.Width / 10) * 2;
                    break;
                case '3':
                    x = (texture.Width / 10) * 3;
                    break;
                case '4':
                    x = (texture.Width / 10) * 4;
                    break;
                case '5':
                    x = (texture.Width / 10) * 5;
                    break;
                case '6':
                    x = (texture.Width / 10) * 6;
                    break;
                case '7':
                    x = (texture.Width / 10) * 7;
                    break;
                case '8':
                    x = (texture.Width / 10) * 8;
                    break;
                case '9':
                    x = (texture.Width / 10) * 9;
                    break;
            }
            Rectangle r = new Rectangle(x, 0, (texture.Width / 10), texture.Height);
            return r;
        }
    }
}
