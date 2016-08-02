using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TrashBash.MonoGame.ScreenSystem
{
    class EndGameScreen : MenuScreen
    {
        Texture2D background;
        Texture2D exit;
        Texture2D exit_On;
        Texture2D rematch;
        Texture2D rematch_On;
        Texture2D winner_p1;
        Texture2D winner_p2;

        Score score;

        int p1score;
        int p2score;

        public EndGameScreen()
        {
            MenuEntries.Add("Rematch");
            MenuEntries.Add("Exit");
        }

        public EndGameScreen(int p1Score, int p2Score)
        {
            p1score = p1Score;
            p2score = p2Score;
            MenuEntries.Add("Rematch");
            MenuEntries.Add("Exit");
        }

        public override void LoadContent()
        {
            background = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/EndLevel/base");
            exit = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/EndLevel/exit");
            exit_On = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/EndLevel/exit_ON");
            rematch = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/EndLevel/rematch");
            rematch_On = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/EndLevel/rematch_ON");
            winner_p1 = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/EndLevel/winner_1p");
            winner_p2 = ScreenManager.ContentManager.Load<Texture2D>("Content/UI/EndLevel/winner_2p");

            score = new Score();
            score.LoadContent(ScreenManager);

            base.LoadContent();
        }

        protected override void OnSelectEntry(int entryIndex)
        {
            switch (entryIndex)
            {
                case 0:
                    ScreenManager.AddScreen(new MainMenuScreen());
                    break;
                case 1:
                    ScreenManager.AddScreen(new Level1());
                    break;
            }
            ExitScreen();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.Draw(exit, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.Draw(rematch, Vector2.Zero, Color.White);
            switch (this.SelectedEntry)
            {
                case 0:
                    ScreenManager.SpriteBatch.Draw(exit_On, Vector2.Zero, Color.White);
                    break;
                case 1:
                    ScreenManager.SpriteBatch.Draw(rematch_On, Vector2.Zero, Color.White);
                    break;
            }
            if (p1score > p2score)
            {
                ScreenManager.SpriteBatch.Draw(winner_p1, Vector2.Zero, Color.White);
            }
            else if (p2score > p1score)
            {
                ScreenManager.SpriteBatch.Draw(winner_p2, Vector2.Zero, Color.White);
            }
            else
            {
                ScreenManager.SpriteBatch.Draw(winner_p1, Vector2.Zero, Color.White);
                ScreenManager.SpriteBatch.Draw(winner_p2, Vector2.Zero, Color.White);
            }

            score.Position = new Vector2(360, 350);
            score.DrawScore(ScreenManager, p1score);

            score.Position = new Vector2(760, 350);
            score.DrawScore(ScreenManager, p2score);

            ScreenManager.SpriteBatch.End();
        }
    }
}
