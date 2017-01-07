using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;




namespace scrollPlatform
{
    class Tile
    {
        protected int xpos, ypos, width, height;
        protected double tileAcross, tileDown;
        protected string type;
        public Tile(Texture2D image, int x, int y, string Type)
        {

            myimage = image;
            width = image.Width;
            height = image.Height;
            xpos = x;
            ypos = y;
            type = Type;
            tileDown = Math.Ceiling((float)ypos / height);
            tileAcross = Math.Ceiling((float)xpos / width);
            if (TileDown < 0) TileDown = 0;
            if (TileAcross < 0) TileAcross = 0;

        }
        public Texture2D myimage { get; set; }
        public int Xpos
        {
            get { return xpos; }
            set { xpos = value; }
        }
        public int Ypos
        {
            get { return ypos; }
            set { ypos = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public double TileAcross
        {
            get { return tileAcross; }
            set { tileAcross = value; }
        }

        public double TileDown
        {
            get { return tileDown; }
            set { tileDown = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public virtual void Update(GameTime gameTime)
        {
            //dummy
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            var destination = new Rectangle(xpos, ypos, width, height);
            spritebatch.Draw(myimage, destination, Color.White);
        }

    }

    class AnimateTile : Tile
    {
        int currentFrame;
        float timer, animationinterval;


        public float Animationinterval
        {
            get { return animationinterval; }
            set { animationinterval = value; }
        }
        public AnimateTile(Texture2D[] image, int x, int y, string Type) : base(image[0], x, y, Type)
        {
            timer = 0;
            currentFrame = 0;
            myimage = image[0];
            Image = image;
            animationinterval = 100;


        }

        public override void Update(GameTime gameTime)
        {

            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > animationinterval)
            {
                currentFrame++;

                if (currentFrame > Image.Length - 1)
                {
                    currentFrame = 0;

                }
                myimage = Image[currentFrame];

                timer = 0f;
               
            }

        }

        public Texture2D[] Image { get; set; }


        public override void Draw(SpriteBatch spritebatch)
        {
            var destination = new Rectangle(xpos, ypos, width, height);
            spritebatch.Draw(myimage, destination, Color.White);


        }
    }
}
