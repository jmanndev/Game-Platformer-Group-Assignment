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
    public class Player : Sprite
    {
        Game game;
        public int score = 0;

        enum EcontrolScheme 
        {
            WASD,
            DPAD
        }

        Keys keyLeft = Keys.A;
        Keys keyRight = Keys.D;
        Keys keyJump = Keys.W;
        Keys keyShoot = Keys.Space;

        public int health;
        int startingHealth = 100;
        public String name;

        public bool alive = true;

        float speed = 5.0f;
        Vector2 direction = Vector2.Zero;

        float jumpSpeed = 15.0f;
        int jumpCount;
        int jumpCountMax = 10;
        int startingJumpCountMax = 10;


        float reverseTime = 1.0f;
        public float reverseCooldownTime = 0.0f;

        float jumpTime = 0.75f;
        public float jumpCooldownTime = 0.0f;

        //current time of bullet
        float bulletTime = 0.35f;
        //cooldown timer for bullet
        float bulletCooldownTime = 0.1f;
        //max allowed bullets
        int bulletGroupMax = 3;
        //current amount of bullets
        int bulletGroup = 0;
        float bulletSpeed = 10.0f;
        int bulletDamage = 30;

        float respawnTime = 0.2f;
        float respawnCooldownTime = 0.0f;

        public Player(String name, Game game, Texture2D playerTexture, Vector2 playerPosition, SpriteBatch spriteBatch, Color playerColor, String a_scheme)
            : base(playerTexture, playerPosition, playerColor, spriteBatch)
        {
            health = startingHealth;
            this.game = game;
            jumpCount = jumpCountMax;
            SetKeys(a_scheme);
            this.name = name;
        }

        void SetKeys(String scheme)
        {
            if (scheme == "WASD")
            {
                keyLeft = Keys.A;
                keyRight = Keys.D;
                keyJump = Keys.W;
                keyShoot = Keys.Space;
            }
            if (scheme == "DPAD")
            {
                keyLeft = Keys.Left;
                keyRight = Keys.Right;
                keyJump = Keys.RightShift;
                keyShoot = Keys.Enter;
            }

        }

        public void Update(GameTime gameTime)
        {
            RespawnTimer(gameTime);

            if (alive)
            {
                base.Update();
                UpdateKeys(gameTime);
                CheckJump();
                SimulateGravity();
                CheckOutsideMap();
                CheckReverseTimer(gameTime);
                CheckJumpTimer(gameTime);
                CheckHealth();
            }
        }

        void Respawn()
        {
            alive = true;
            health = startingHealth;
            position = game.NewPlayerPosition();
        }

        void RespawnTimer(GameTime gameTime)
        {
            respawnCooldownTime -= 0.1f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!alive)
            {
                if (respawnCooldownTime < 0.0f)
                {
                    Respawn();
                }
            }
        }

        //controls
        void UpdateKeys(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(keyLeft))
            {
                direction.X = -1;

                if (!isHittingLeft())
                {
                    UpdatePosition();
                }
            }

            if (Keyboard.GetState().IsKeyDown(keyRight))
            {
                direction.X = 1;

                if (!isHittingRight())
                {
                    UpdatePosition();
                }
            }

            if (Keyboard.GetState().IsKeyDown(keyJump))
            {
                if (isOnFirmGround())
                    Jump();
            }

            bulletCooldownTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(keyShoot))
            {
                Shoot(gameTime);
            }
        }

        //jumping
        void Jump()
        {
            direction.Y = -1;
            jumpCount = 0;
        }

        void CheckJump()
        {
            if (isHittingRoof())
            {
                jumpCount = jumpCountMax;
            }

            if (jumpCount < jumpCountMax)
            {
                if (!isHittingRoof())
                {
                    UpdateVerticalPosition();
                    jumpCount++;
                }
            }
        }

        //movement
        void UpdatePosition()
        {
            if (position.Y < game.graphics.PreferredBackBufferHeight && position.Y > 0 - texture.Height)
                position.X += direction.X * speed;
        }

        void CheckOutsideMap()
        {
            if (position.Y > game.graphics.PreferredBackBufferHeight)
            {
                position.Y = 0 - texture.Height;
            }
            if (position.Y < 0 - texture.Height)
            {
                position.Y = game.graphics.PreferredBackBufferHeight;
            }
        }

        void UpdateVerticalPosition()
        {
            Console.WriteLine("updating jump position");
            position.Y += direction.Y * jumpSpeed;
        }
       
        void SimulateGravity()
        {
            if (!isOnFirmGround())
            {
                position.Y += Board.CurrentBoard.gravity;
            }
        }


        //edge of rectangle checks
        bool isHittingRoof()
        {
            Rectangle onePixelUp = rectangle;
            onePixelUp.Offset(0, -(int)jumpSpeed);
            Console.WriteLine("Roof: " + Board.CurrentBoard.checkTileCollision(onePixelUp));
            return Board.CurrentBoard.checkTileCollision(onePixelUp);
        }

        bool isHittingLeft()
        {
            Rectangle onePixelLeft = rectangle;
            onePixelLeft.Offset(-(int)speed, 0);
            Console.WriteLine("Left: " + Board.CurrentBoard.checkTileCollision(onePixelLeft));
            return Board.CurrentBoard.checkTileCollision(onePixelLeft);
        }

        bool isHittingRight()
        {
            Rectangle onePixelRight = rectangle;
            onePixelRight.Offset((int)speed, 0);
            Console.WriteLine("Right: " + Board.CurrentBoard.checkTileCollision(onePixelRight));
            return Board.CurrentBoard.checkTileCollision(onePixelRight);
        }

        bool isOnFirmGround()
        {
            Rectangle onePixelLower = rectangle;
            onePixelLower.Offset(0, (int)Board.CurrentBoard.gravity);
            Console.WriteLine("Ground: " + Board.CurrentBoard.checkTileCollision(onePixelLower));
            return Board.CurrentBoard.checkTileCollision(onePixelLower);
        }

        public override void Draw()
        {
            if (alive)
            {
                base.Draw();
            }
        }

        //shoot
        void Shoot(GameTime gameTime)
        {
            if (bulletCooldownTime < 0.0f)
            {
                Vector2 shootPosition;
                if (direction.X > 0)
                    shootPosition.X = position.X + base.texture.Width - game.bulletTexture.Width / 2;
                else
                    shootPosition.X = position.X - game.bulletTexture.Width / 2;

                shootPosition.Y = position.Y + (base.texture.Height / 2) - (game.bulletTexture.Height / 2);

                Vector2 shootDirection = direction * Vector2.UnitX;

                if (shootDirection == Vector2.Zero)
                    shootDirection = Vector2.UnitX;

                game.Shoot(shootPosition, shootDirection, bulletSpeed, bulletDamage, this, color);

                bulletGroup++;
                bulletCooldownTime = bulletTime / 4;

                if (bulletGroup >= bulletGroupMax)
                {
                    bulletCooldownTime = bulletTime;
                    bulletGroup = 0;
                }
            }
        
        }

        void CheckReverseTimer(GameTime gameTime)
        {
            reverseCooldownTime -= 0.1f * (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (reverseCooldownTime < 0.0f)
            {
                speed = Math.Abs(speed);
            }
        }

        void CheckJumpTimer(GameTime gameTime)
        {
            jumpCooldownTime -= 0.1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (jumpCooldownTime < 0.0f)
            {
                jumpCountMax = startingJumpCountMax;
            }
        }


        public void OrbEffect(int colorNo)
        {
            switch (colorNo)
            {
                case 1:
                    health += 20;
                    break;
                case 2:
                    reverseCooldownTime = reverseTime;
                    speed *= -1;
                    break;
                case 3:
                    jumpCooldownTime = jumpTime;
                    jumpCountMax = 30;
                    jumpCount = 30;
                    break;
            }
        }

        void CheckHealth()
        {
            if (health <= 0)
                alive = false;
        }

        public bool Hit(int damage)
        {
            health -= damage;

            if (health <= 0)
            {
                health = 0;
                respawnCooldownTime = respawnTime;
                return true;
            }

            return false;
        }

        public void AddPoints(int amount)
        {
            score += amount;
        }

        public void ResetPlayer(Vector2 pos)
        {
            position = pos;
            alive = true;
            score = 0;
            health = startingHealth;
            jumpCount = jumpCountMax;
        }
    }
}
