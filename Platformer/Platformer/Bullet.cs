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

   
    class Bullet : Sprite
    {
        float speed;
        public int damage;
        Vector2 direction;
        public Player owner;
        
        
        public Bullet(Texture2D bulletTexture, Vector2 bulletPosition, SpriteBatch spriteBatch, Color bulletColor, float speed, int damage, Vector2 direction, Player owner)
            : base(bulletTexture, bulletPosition, bulletColor, spriteBatch)
        {
            this.damage = damage;
            this.speed = speed;
            this.direction = direction;
            this.owner = owner;
        }

        public override void Update()
        {
            base.position += direction * speed;
            base.Update();
        }



    }
}
