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
    class Orb : Sprite
    {
        public int colorNo;

        public Orb(Texture2D orbTexture, Vector2 orbPosition, SpriteBatch spriteBatch, Color color, int colorNo)
            : base(orbTexture, orbPosition, color, spriteBatch)
        {
            this.colorNo = colorNo;
        }


        
    }
}
