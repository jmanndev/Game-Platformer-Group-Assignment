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
    class Tile : Sprite
    {
        public bool isBlocked;

        //public Rectangle leftEdge;
        //public Rectangle rightEdge;
        //public Rectangle topEdge;
        //public Rectangle bottomEdge;

        public Tile(Texture2D tileTexture, Vector2 tilePosition, SpriteBatch spriteBatch, bool isBlocked) 
            : base(tileTexture, tilePosition, Color.White, spriteBatch)
        {
            this.isBlocked = isBlocked;
            //CreateEdgeRectangles();
        }

        public override void Draw()
        {
            if (isBlocked)
            {
                base.Draw();

                //base.spriteBatch.Draw(base.texture, leftEdge, Color.Blue);
                //base.spriteBatch.Draw(base.texture, rightEdge, Color.Red);
                //base.spriteBatch.Draw(base.texture, topEdge, Color.Green);
                //base.spriteBatch.Draw(base.texture, bottomEdge, Color.Yellow);

            }
        }

        //void CreateEdgeRectangles()
        //{
        //    Rectangle rec = base.rectangle;
        //    leftEdge = new Rectangle(rec.Left, rec.Top, rec.Width / 3, rec.Height);
        //    rightEdge = new Rectangle(rec.Right - rec.Width / 3, rec.Top, rec.Width / 3, rec.Height);
        //    topEdge = new Rectangle(rec.Left, rec.Top, rec.Width, rec.Height / 3);
        //    bottomEdge = new Rectangle(rec.Left, rec.Bottom - rec.Height / 3, rec.Width, rec.Height / 3);

        //}


    }
}
