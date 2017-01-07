using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace scrollPlatform
{
    class Missile
    {

        private bool hit, rotatemissile;
        private float animationinterval = 20f;
        public Vector2 Position;
        private float horizontalVelocity;
        float startposX;
        float initvelocity;
        Random r;

        Rectangle imageRectange;
        readonly Texture2D myimage;
        float timer, totaltime;
        SoundEffect myeffect;
        Direction direction;
        Owner parent;

        public Missile(Texture2D content, Vector2 startpos, Direction Dir, bool randomspeed , float hvelocity = 300, Owner myparent = Owner.FOE)
        {
            Position = startpos;
          //  Position.X = Position.X - 25;
            r = new Random();
            myimage = content;
           // myeffect = content.Load<SoundEffect>("MissileWav");
            imageRectange = new Rectangle(0, 0, myimage.Width, myimage.Height);
            timer = 0f;
            totaltime = 0f;
            if (randomspeed) { horizontalVelocity = r.Next(200, 400); }
            else horizontalVelocity = hvelocity;
            //horizontalVelocity = randomspeed ? r.Next(200, 400) : 300;
            if (Dir == Direction.RIGHT )
            {
                Position.Y = Position.Y - 36;
            }
            if (Dir == Direction.LEFT)
            {
                horizontalVelocity = -horizontalVelocity;
            }
            hit = false;
            rotatemissile = false;
            direction = Dir;
            initvelocity = 7;
            startposX = Position.X;
            parent = myparent;
           
        }

        public bool Hit
        {
            get { return hit; }
            set { hit = value; }
        }

        public Owner Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public bool RotateMissile
        {
           set { rotatemissile = value; }
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, imageRectange.Width, imageRectange.Height);
            }
        }

        public void Update(GameTime gameTime)
        {

            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            totaltime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > animationinterval)
            {
                if (direction == Direction.DOWN)
                {
                    Position.Y += 4;
                    timer = 0f;
                }
                else
                {
                    Position = Pyhsics.GetTrajectoryPos(horizontalVelocity, initvelocity, startposX, totaltime, Position);
                    timer = 0f;
                    if ((Position.X < 1) | Position.X > Constants.ScreenWidth) { hit = true; }
                }

            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (!hit)
            {
                if(rotatemissile)
                {
                    spritebatch.Draw(myimage, Position, null, Color.White, (float)Math.PI, new Vector2(imageRectange.Width, imageRectange.Height), 1, SpriteEffects.None, 0);
                }
                else
                spritebatch.Draw(myimage, Position, imageRectange, Color.White);
            }
        }

    }

    class Explosion
    {

        public float animationinterval;
        public Vector2 Position;
        Rectangle imageRectange;
        readonly Texture2D myimage;
        float timer;
        SoundEffect myeffect;
        int currentFrame;
        int tilenumbers;
        protected Rectangle[] foerect;
        public bool hit;
        




        public Explosion(ContentManager content, Vector2 startpos)
        {
            currentFrame = 0;
            animationinterval = 50f;
            myimage = content.Load<Texture2D>("explosion");
            myeffect = content.Load<SoundEffect>("bomb-sound");
            tilenumbers = myimage.Width / myimage.Height;
            foerect = new Rectangle[tilenumbers];
            for (int i = 0; i <= tilenumbers - 1; i++)
            {
                foerect[i] = new Rectangle(myimage.Height * i, 0, myimage.Height, myimage.Height);
            }
            timer = 0f;
            imageRectange = foerect[0];
            Position = startpos;
            hit = false;
            Position.Y = Position.Y - 25;
            

        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, imageRectange.Width, imageRectange.Height);
            }
        }

        public bool Hit
        {
            get { return hit; }

            set
            {
                hit = value;
               
            }
        }

        public void Update(GameTime gameTime)
        {

            if (currentFrame == 1) { myeffect.Play(); }
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > animationinterval)
            {
                currentFrame++;
                if (currentFrame > tilenumbers - 1)
                {
                    
                    hit = true;
                    currentFrame = 0;
                   

                }

                timer = 0f;
                imageRectange = foerect[currentFrame];

            }

        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(myimage, Position, imageRectange, Color.White);
        }



    }

}
