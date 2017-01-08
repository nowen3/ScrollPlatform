using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;


namespace scrollPlatform
{
    abstract class Sprite
    {
        protected Texture2D myimage;
        protected Vector2 position;
        protected int currentFrame;
        protected int tilenumbers;
        protected string type;
        protected SoundEffect sound;
        protected Rectangle[] foerect;
        protected Rectangle imageRectange;
        protected bool hit;
        protected float timer;
        protected float animationinterval;
        protected bool rotate;
        protected int health;

        public Sprite(ContentManager content, gameObjects go)
        {
            currentFrame = 0;
            myimage = content.Load<Texture2D>(go.content);
            if (!string.IsNullOrEmpty(go.Sound))
            {
                sound = content.Load<SoundEffect>(go.Sound);
            }
            if (!string.IsNullOrEmpty(go.type))
            {
                type = go.type;
            }
            tilenumbers = (myimage.Width / go.width);
            foerect = new Rectangle[tilenumbers];
            for (int i = 0; i <= tilenumbers - 1; i++)
            {
                foerect[i] = new Rectangle(go.width * i, 0, go.width, go.height);
            }
            imageRectange = foerect[0];
            position = new Vector2(go.xpos, go.ypos);
            animationinterval = 100f;
            rotate = false;
            if (go.rotation == 180) { rotate = true; }
            hit = false;
        }

        public bool Hit
        {
            get { return hit; }

            set
            {
                hit = value;
                if (sound != null)
                    sound.Play();
            }
        }

        public virtual int Health
        {
            get { return health; }

            set { health = health - value; }
    
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Texture2D GetImage
        {
            get { return myimage; }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)position.X, (int)position.Y, imageRectange.Width, imageRectange.Height); }
        }

        public virtual void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > animationinterval)
            {
                currentFrame++;
                if (currentFrame > tilenumbers - 1)
                {
                    currentFrame = 0;
                }
                timer = 0f;
                imageRectange = foerect[currentFrame];
            }
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {

            if (rotate)
            {
                Vector2 pos = new Vector2(imageRectange.Width / 2, imageRectange.Height);
                spritebatch.Draw(myimage, position, imageRectange, Color.White, 0, pos, 1, SpriteEffects.FlipHorizontally, 0);
            }
            else
                spritebatch.Draw(myimage, position, imageRectange, Color.White);
        }

    }
//-----------------------------------------------------------------------------------------------------------------------------------
    class SmallBot: Sprite
    {
        protected int animoveby;
        protected bool direction;
               
        public SmallBot(ContentManager content, gameObjects go) : base(content, go)

        {
            animoveby = 4;
            timer = 0f;
            direction = false;
            animationinterval = 200f;
            health = go.health;
        }

        
        public override int Health
        {
            get { return health; }

            set
            {
                health = health -  value;
                if (health <= 0)
                    hit = true;
            }
        }

      
        public override void Update(GameTime gameTime)
        {
            int frame = 0;
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            string below = Map.GetTileBelow(position, imageRectange);
            string frontr = Map.GetTileRight(position, imageRectange);
            string frontl = Map.GetTileLeft(position, imageRectange);

            if (timer > animationinterval)
            {
                currentFrame++;
                if (currentFrame > (tilenumbers/2) - 1)
                {
                    currentFrame = 0;

                }

                timer = 0f;
                if (direction)
                {
                    if (below == "Nothing" | frontl == "Solid")
                    {
                        direction = !direction;
                        position.X += animoveby;
                    }
                    else
                        position.X -= animoveby;
                }
                else
                {
                    if (below == "Nothing" | frontr == "Solid")
                    {
                        direction = !direction;
                        position.X -= animoveby;
                    }
                    else
                        position.X += animoveby;

                }
                if (!direction) { frame = currentFrame; }
                else frame = currentFrame + 3;

                imageRectange = foerect[frame];

            }
        }


    }
    //-----------------------------------------------------------------------------------------------------------------------------------
    class Drone : Sprite
    {

        int inc;
        protected int animoveby;
        float firetime, lighttimer;
        bool fire;

        public Drone(ContentManager content, gameObjects go) : base(content, go)
        {
            animationinterval = 50;
            inc = 0;
            fire = false;
            firetime = 0;
            lighttimer = 0;
            animoveby = 4;
            health = go.health;
           
        }

        public bool Fire
        {
          get { return fire; }
          set { fire = value; }
        }

        public override int Health
        {
            get { return health; }

            set
            {
                health = health - value;
                if (health <= 0)
                    hit = true;
            }
        }

        public override void Update(GameTime gameTime)
        {

            fire = false;
            string below = Map.GetTileBelow(position, imageRectange);
            string right = Map.GetTileRight(position, imageRectange);
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            firetime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            lighttimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (firetime > 2000)
            {
                firetime = 0;
                fire = true;
            }

            if (timer > animationinterval)
            {
                if (below == "Nothing")
                {

                    position.X += animoveby;
                }
                timer = 0f;
            }
            if (lighttimer > 500)
            {
                lighttimer = 0;
                currentFrame = currentFrame + inc;
                if (currentFrame > tilenumbers - 1)
                {
                    inc = -1;
                    currentFrame = tilenumbers - 2;
                   
                }
                if (currentFrame < 1)
                {
                    inc = 1;
                    currentFrame = 0;
                }
               
                imageRectange = foerect[currentFrame];
            }
            if (right == "Solid")
            {
                position.X = position.X - animoveby;
                animoveby = -animoveby;
            }

           
            if (position.X + imageRectange.Width >= Map.Width )
            {
                position.X = Map.Width - imageRectange.Width;
                animoveby = -animoveby;
            }
            if (position.Y < 0) position.Y = 0;
            if (position.X < 0)
            {
                position.X = 0;
                animoveby = Math.Abs(animoveby);
            }
        }

    }
    //-----------------------------------------------------------------------------------------------------------------------------------
    class BigGun : Sprite
    {

        int inc;
        Direction firedirection;

        public BigGun(ContentManager content, gameObjects go) : base(content, go)
        {
            animationinterval = 300f;
            inc = 0;
            Fire = false;
            if (rotate)
                firedirection = Direction.LEFT;
            else firedirection = Direction.RIGHT;
            health = go.health;
        }

        public bool Fire { get; set; }
        public Direction FireDirection
        {
            get { return firedirection; }
            set { firedirection = value; }
        }

        public override int Health
        {
            get { return health; }

            set
            {
                health = health - value;
                if (health <= 0)
                    hit = true;
            }
        }

        public override void Update(GameTime gameTime)
        {

            Fire = false;

            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer > animationinterval)
            {
                currentFrame = currentFrame + inc;
                if (currentFrame > tilenumbers - 1)
                {
                    inc = -1;
                    currentFrame = tilenumbers - 2;
                    Fire = true;
                }
                if (currentFrame < 1)
                {
                    inc = 1;
                    currentFrame = 0;
                }
                timer = 0f;
                imageRectange = foerect[currentFrame];
            }
        }





    }
}
