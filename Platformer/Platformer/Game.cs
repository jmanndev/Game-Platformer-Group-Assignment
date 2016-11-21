using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Platformer
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        enum EGameState
        {
            GAME,
            CREDITS,
            GAMEOVER,
            PAUSE
        }

        EGameState eGameState = EGameState.CREDITS;
        
        public Random rand = new Random();

        int windowHeight = 800;
        int windowWidth = 1216;

        int hitPoint = 10;
        int killPoint = 30;

        Texture2D levelBackground;

        Texture2D playerTexture;
        Texture2D tileTexture;
        public Texture2D bulletTexture;
        Texture2D orbTexture;
        Texture2D logoTexture;
        Texture2D jasonTexture;
        Texture2D solTexture;
        Texture2D jonoTexture;
   
        Board board;
        Player player1;
        Player player2;
        Player winner;

        Keys actionKey = Keys.P;
        bool actionLastPressed;

        //font
        SpriteFont gameFont;

        int numberOfOrbs = 10;

        List<Player> playerList = new List<Player>();
        List<Orb> orbList = new List<Orb>();
        List<Bullet> bulletList = new List<Bullet>();

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferredBackBufferWidth = windowWidth;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            gameFont = Content.Load<SpriteFont>("font");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tileTexture = Content.Load<Texture2D>("tile");
            playerTexture = Content.Load<Texture2D>("player");
            bulletTexture = Content.Load<Texture2D>("bullet");
            orbTexture = Content.Load<Texture2D>("orb");
            levelBackground = Content.Load<Texture2D>("Castle-Background");
            logoTexture = Content.Load<Texture2D>("logo");
            jasonTexture = Content.Load<Texture2D>("names/Jason");
            solTexture = Content.Load<Texture2D>("names/sol");
            jonoTexture = Content.Load<Texture2D>("names/Jono");

            

            player1 = new Player("P1", this, playerTexture, new Vector2(350, 450), spriteBatch, Color.Red, "WASD");
            player2 = new Player("P2", this, playerTexture, new Vector2(900, 450), spriteBatch, Color.Chartreuse, "DPAD");
            playerList.Add(player1);
            playerList.Add(player2);
            
            board = new Board(spriteBatch, tileTexture, 25, 38);

            for (int i = 0; i < numberOfOrbs; i++)
            {
                GenerateOrb();
            }
        }

        //Update commands
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (eGameState)
            {
                case EGameState.GAME:
                    UpdateGame(gameTime);
                    break;
                case EGameState.GAMEOVER:
                    UpdateGameOver();
                    break;
                case EGameState.CREDITS:
                    UpdateCredits();
                    break;
                case EGameState.PAUSE:
                    UpdateCredits();
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateGame(GameTime gameTime)
        {
            player1.Update(gameTime);
            player2.Update(gameTime);

            for (int i = bulletList.Count - 1; i >= 0; i--)
                bulletList[i].Update();

            for (int i = orbList.Count - 1; i >= 0; i--)
                orbList[i].Update();

            CheckBulletCollisions();
            CheckBulletPlayerCollisions();
            CheckOrbCollisions();

            foreach (Player player in playerList)
            {
                if (player.score >= 20)
                {
                    eGameState = EGameState.GAMEOVER;
                    winner = player;
                }
            }

            if (Keyboard.GetState().IsKeyDown(actionKey) && !actionLastPressed)
            {
                eGameState = EGameState.PAUSE;
                actionLastPressed = true;
            }

            if (Keyboard.GetState().IsKeyUp(actionKey))
            {
                actionLastPressed = false;
            }
        }

        private void UpdateGameOver()
        {
            if (Keyboard.GetState().IsKeyDown(actionKey))
            {
                ResetGame();
                eGameState = EGameState.GAME;
                actionLastPressed = true;
            }
        }

        private void UpdateCredits()
        {
            if (Keyboard.GetState().IsKeyDown(actionKey) && !actionLastPressed)
            {
                eGameState = EGameState.GAME;
                actionLastPressed = true;
            }

            if (Keyboard.GetState().IsKeyUp(actionKey))
            {
                actionLastPressed = false;
            }
        }

        private void ResetGame()
        {
            winner = null;

            orbList = new List<Orb>();
            bulletList = new List<Bullet>();

            foreach (Player player in playerList)
            {
                player.ResetPlayer(NewPlayerPosition());
            }

            for (int i = 0; i < numberOfOrbs; i++)
            {
                GenerateOrb();
            }
        }

        //Draw commmands
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();


            switch (eGameState)
            {
                case EGameState.GAME:
                    DrawGame();
                    DrawScore();
                    break;
                case EGameState.GAMEOVER:
                    DrawGameOver();
                    break;
                case EGameState.CREDITS:
                    DrawCredits();
                    break;
                case EGameState.PAUSE:
                    DrawCredits();
                    DrawScore();
                    break;
                default:
                    break;
            }
            
            base.Draw(gameTime);
            spriteBatch.End();
        }

        private void DrawGame()
        {
            spriteBatch.Draw(levelBackground, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);
            board.Draw();
            player1.Draw();
            player2.Draw();

            for (int i = bulletList.Count - 1; i >= 0; i--)
                bulletList[i].Draw();

            for (int i = orbList.Count - 1; i >= 0; i--)
                orbList[i].Draw();
        }

        private void DrawScore()
        {
            //Player One
            spriteBatch.DrawString(gameFont, "P1 Health: " + player1.health, Vector2.One * 10, Color.White);
            spriteBatch.DrawString(gameFont, "P1 Score: " + player1.score, new Vector2(10, 40), Color.White);
            //Player Two 
            spriteBatch.DrawString(gameFont, "P2 Health: " + player2.health, new Vector2(1060, 10), Color.White);
            spriteBatch.DrawString(gameFont, "P2 Score: " + player2.score, new Vector2(1060, 40), Color.White);
        }

        private void DrawGameOver()
        {
            DrawGame();
            spriteBatch.DrawString(gameFont, "Congratulations, " + winner.name + " you win.", Vector2.One * 400, Color.White);
        }

        private void DrawCredits()
        {
            DrawGame();
            spriteBatch.Draw(logoTexture, new Rectangle(windowWidth /2 - 480,0, 960,600), Color.White);
            spriteBatch.Draw(jasonTexture, new Rectangle(windowWidth / 2 - 480, 400, 200, 100), Color.White);
            spriteBatch.Draw(jonoTexture, new Rectangle(windowWidth / 2 -80, 400, 200, 100), Color.White);
            spriteBatch.Draw(solTexture, new Rectangle(windowWidth / 2 + 250, 400, 200, 100), Color.White);

        }

        //Bullet Collision
        void CheckBulletCollisions()
        {
            for (int i = bulletList.Count - 1; i >= 0; i--)
            {
                foreach (var tile in Board.CurrentBoard.tileGrid)
                {
                    if (tile.isBlocked && bulletList[i].rectangle.Intersects(tile.rectangle))
                    {
                        bulletList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        void CheckBulletPlayerCollisions()
        {
            for (int i = bulletList.Count - 1; i >= 0; i--)
            {
                foreach (var player in playerList)
                {
                    if (player.alive && bulletList[i].owner != player && bulletList[i].rectangle.Intersects(player.rectangle))
                    {
                        if (player.Hit(bulletList[i].damage))
                        {
                            bulletList[i].owner.AddPoints(killPoint);
                        }

                        bulletList[i].owner.AddPoints(hitPoint);
                        bulletList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        //Bullet Spawn
        public void Shoot(Vector2 shootPosition, Vector2 shootDirection, float bulletSpeed, int bulletDamage, Player owner, Color color)
        {
            Bullet b = new Bullet(bulletTexture, shootPosition, spriteBatch, color, bulletSpeed, bulletDamage, shootDirection, owner);
            bulletList.Add(b);
        }

        //Orbs
        void GenerateOrb()
        {
            Vector2 orbPosition;
            Rectangle orbRectangle;

            do 
            {
                orbPosition.X = rand.Next(graphics.PreferredBackBufferWidth);
                orbPosition.Y = rand.Next(3 * tileTexture.Height, graphics.PreferredBackBufferHeight - 2 * tileTexture.Height);
                orbRectangle = new Rectangle((int)orbPosition.X,(int)orbPosition.Y,orbTexture.Width, orbTexture.Height);   
            }
            while (Board.CurrentBoard.checkTileCollision(orbRectangle));

            int colorNo = rand.Next(1, 4);

            Orb o = new Orb(orbTexture, orbPosition, spriteBatch, OrbColor(colorNo), colorNo);
            orbList.Add(o);
        }

        Color OrbColor(int colorNo)
        {
            Color orbColor;

            switch (colorNo)
            {
                case 1:
                    orbColor = Color.Crimson;
                    break;
                case 2:
                    orbColor = Color.Aquamarine;
                    break;
                case 3:
                    orbColor = Color.RoyalBlue;
                    break;
                default:
                    orbColor = Color.RoyalBlue;
                    break;
            }

            return orbColor;
        }

        void CheckOrbCollisions()
        {
            for (int i = orbList.Count - 1; i >= 0; i--)
            {
                foreach (var player in playerList)
                {
                    if (player.rectangle.Intersects(orbList[i].rectangle))
                    {
                        player.OrbEffect(orbList[i].colorNo);
                        orbList.RemoveAt(i);
                        break;

                    }
                }

            }
        }

        public Vector2 NewPlayerPosition()
        {
            Vector2 position;
            Rectangle tempRec;

            do
            {
                position.X = rand.Next(graphics.PreferredBackBufferWidth);
                position.Y = rand.Next(3 * tileTexture.Height, graphics.PreferredBackBufferHeight - 2 * tileTexture.Height);
                tempRec = new Rectangle((int)position.X, (int)position.Y, playerTexture.Width, playerTexture.Height);
            }
            while (Board.CurrentBoard.checkTileCollision(tempRec));

            return position;
        }
    }
}
