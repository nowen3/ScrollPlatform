using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace scrollPlatform
{
    class Dead
    {

        private bool hit;
        private float animationinterval = 20;
        private Vector2[] Position;
        private Rectangle[] deadrect;
        readonly Texture2D myimage;
        float timer;
        private int animoveby, counter, tilenumbers;

        private float horizontalVelocity;
        
        public Dead(Texture2D image, Vector2 startpos)
        {
            tilenumbers = (image.Width / image.Height);
            myimage = SpriteUtils.SplitSingle(image, image.Width / tilenumbers, image.Height);

            Position = new Vector2[4];
            deadrect = new Rectangle[4];
           
            deadrect[0] = new Rectangle(0, 0, myimage.Width / 2, myimage.Height / 2);
            deadrect[1] = new Rectangle(myimage.Width / 2, 0, myimage.Width / 2, myimage.Height / 2);
            deadrect[2] = new Rectangle(0, myimage.Height / 2, myimage.Width / 2, myimage.Height / 2);
            deadrect[3] = new Rectangle(myimage.Width / 2, myimage.Height / 2, myimage.Width / 2, myimage.Height / 2);
            Position[0] = new Vector2(startpos.X, startpos.Y);
            Position[1] = new Vector2(startpos.X, startpos.Y);
            Position[2] = new Vector2(startpos.X, startpos.Y);
            Position[3] = new Vector2(startpos.X, startpos.Y);
            animoveby = 4;
            counter = 0;
            hit = false;

         

        }

        public bool Hit
        {
            get { return hit; }
            set { hit = value; }
        }

       
        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            if (timer > animationinterval)
            {
                timer = 0f;
                counter++;
                Position[0].X = Position[0].X - animoveby;
                Position[0].Y = Position[0].Y + animoveby;
               
                Position[1].X = Position[1].X + animoveby;
                Position[1].Y = Position[1].Y + animoveby;
               
                Position[2].X = Position[2].X - animoveby;
                Position[3].X = Position[3].X + animoveby;
             
                if (counter > 7) hit = true;
            }
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {

           spritebatch.Draw(myimage, Position[0], deadrect[0], Color.White);
            spritebatch.Draw(myimage, Position[1], deadrect[1], Color.White);
            spritebatch.Draw(myimage, Position[2], deadrect[2], Color.White);
            spritebatch.Draw(myimage, Position[3], deadrect[3], Color.White);
        }


    }
}
