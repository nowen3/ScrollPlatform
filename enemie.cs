using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;


namespace scrollPlatform
{
    class enemie
    {
        protected bool hit;
        protected float animationinterval;
        public Vector2 position;
        protected int animoveby;
        protected bool direction;
        protected Rectangle[] foerect;
        protected Rectangle imageRectange;
        protected int health;
        public Texture2D myimage;
        protected float timer;
        protected SoundEffect myeffect;
        protected bool rotate;
        int currentFrame;
        int tilenumbers;
        public string Type;



        public enemie(ContentManager content, gameObjects go)

        {
            
            myimage = content.Load<Texture2D>(go.content);
            if (!string.IsNullOrEmpty(go.Sound))
            {
                myeffect = content.Load<SoundEffect>(go.Sound);
            }
            if (!string.IsNullOrEmpty(go.type))
            {
                Type = go.type;
            }
            tilenumbers = (myimage.Width / go.width);
            foerect = new Rectangle[tilenumbers];
            for (int i = 0; i <= tilenumbers - 1; i++)
            {
                foerect[i] = new Rectangle(go.width * i, 0, go.width, go.height);
            }
            animoveby = 4;
            timer = 0f;
            imageRectange = foerect[0];
            position = new Vector2(go.xpos, go.ypos);
            hit = false;
            
            direction = false;
            currentFrame = 0;
            animationinterval = 200f;
            rotate = false;
            if (go.rotation == 180) { rotate = true; }
            health = go.health;
            Debug.WriteLine(health);


        }

        public bool Hit
        {
            get { return hit; }

            set
            {
                   hit = value;
                if (myeffect != null)
                    myeffect.Play();
            }
        }

        public int Health
        {
            get { return health; }

            set
            {
                health = health -  value;
                if (health <= 0)
                    hit = true;
            }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }


        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, imageRectange.Width, imageRectange.Height);
            }
        }

        public virtual void Update(GameTime gameTime, Map map)
        {
            int frame = 0;
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            string below = map.GetTileBelow(position, imageRectange);
            string frontr = map.GetTileRight(position, imageRectange);
            string frontl = map.GetTileLeft(position, imageRectange);

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

    class Drone : enemie
    {

        int currentFrame, inc;
        int tilenumbers;
        float firetime, lighttimer;
        bool fire;

        public Drone(ContentManager content, gameObjects go) : base(content, go)
        {
            animationinterval = 50;
            currentFrame = 0;
            tilenumbers = myimage.Width / go.width;
            inc = 0;
            fire = false;
            firetime = 0;
            lighttimer = 0;
           
        }

        public bool Fire
        {
          get { return fire; }
          set { fire = value; }
        }
        
        public override void Update(GameTime gameTime, Map map)
        {

            fire = false;
            string below = map.GetTileBelow(position, imageRectange);
            string right = map.GetTileRight(position, imageRectange);
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

           
            if (position.X + imageRectange.Width >= map.Width )
            {
                position.X = map.Width - imageRectange.Width;
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

    class BigGun : enemie
    {

        int currentFrame, inc;
        int tilenumbers;
        Direction firedirection;

        public BigGun(ContentManager content, gameObjects go) : base(content, go)
        {
            animationinterval = 300f;
            currentFrame = 0;
            tilenumbers = myimage.Width / go.width;
            inc = 0;
            Fire = false;
            if (rotate)
                firedirection = Direction.LEFT;
            else firedirection = Direction.RIGHT;
        }

        public bool Fire { get; set; }
        public Direction FireDirection
        {
            get { return firedirection; }
            set { firedirection = value; }
        }

        public override void Update(GameTime gameTime, Map map)
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
