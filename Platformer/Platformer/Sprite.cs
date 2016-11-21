

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
    public class Sprite
    {
        public Texture2D texture;
        public Vector2 position;
        public SpriteBatch spriteBatch;
        public Color color;
        public Rectangle rectangle;

        public Sprite(Texture2D texture, Vector2 position, Color color, SpriteBatch spriteBatch)
        {
            this.texture = texture;
            this.position = position;
            this.spriteBatch = spriteBatch;
            this.color = color;
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public virtual void Update()
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public virtual void Draw()
        {
            spriteBatch.Draw(texture, position, color);
        }
    }
}
