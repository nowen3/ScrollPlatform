using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace scrollPlatform
{
    class Player
    {
        KeyboardState currentKBState;
        Rectangle[] mariorect, mariorectfire;
        Rectangle image;
        Vector2 position;
        readonly Texture2D myimage;
       // Map map;
       
        int jumpcount, currentFrame, gravity, animoveby, maxgravity, fallcount;
        int tilewidth, tileheight;
        const float interval = 75;
        float timer;
        Direction mydirection;


        public bool onplatform;
        public bool HasKey, AtExit, fire;
        bool jump, falling, hit;
        string below, left, right, above;
        SoundEffect jumpsound;
        KeyboardState lastState;

        public Player(ContentManager content, gameObjects go)
        {

            myimage = content.Load<Texture2D>(go.content);
            jumpsound = content.Load<SoundEffect>("Jump");
            tilewidth = go.width;
            tileheight = go.height;
            int tileno = myimage.Width / tilewidth;
            mariorectfire = new Rectangle[tileno];
            mariorect = new Rectangle[tileno];
            for (int i = 0; i <= tileno - 1; i++)
            {
                mariorect[i] = new Rectangle(tilewidth * i, 0, tilewidth, tileheight);
            }
            for (int i = 0; i <= tileno - 3; i++)
            {
                mariorectfire[i] = new Rectangle(30 * i, 50, 30, tileheight);
            }
            animoveby = 2;  // pixels moved per mario frame
            currentFrame = 0;
            timer = 0f;
            image = mariorect[0];
            gravity = 2;
            maxgravity = 8; // max falling rate
            fallcount = 0; // how far we have fallen. > 8 increase gravity
            jump = false; // are we jumping
            jumpcount = 0; //height of jump
            IsDead = false;
            onplatform = false;
            HasKey = false;
            AtExit = false;
            falling = false;
           // movedirection = true; // true = right, fasle = left
            mydirection = Direction.RIGHT;

        }



        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        //public Map MyMap
        //{
        //    set { map = value; }
        //}

        public bool Fire
        {
            get { return fire; }
            set { fire = value; }
        }

        public Direction PlayerDirection
        {
            get { return mydirection; }
            set { mydirection = value; }
        }

        public bool Hit
        {
            get { return hit; }
            set
            {
                hit = value;
                jump = true;
                // dead.Play();

            }
        }

        public bool IsDead { get; set; }



        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, image.Width, image.Height);
            }
        }

        public void Update(GameTime gameTime)
        {
            falling = false;
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            currentKBState = Keyboard.GetState();
            if (!hit)
            {
                if (currentKBState.IsKeyDown(Keys.F) && !lastState.IsKeyDown(Keys.F) && !falling)
                {
                    fire = !fire;
                    if (fire)
                    {
                        if (mydirection == Direction.RIGHT)
                            image = mariorectfire[currentFrame];
                        else image = mariorectfire[currentFrame + 8];
                    }
                    else
                        image = mariorect[currentFrame];

                }
                if (!currentKBState.IsKeyDown(Keys.F)) { fire = false; }

                if (currentKBState.IsKeyDown(Keys.Left) & !falling)
                {
                    left = Map.GetTileLeft(position, image);
                    if (left == "Nothing" | left == "Ladder")
                    {
                        AnimateLeft();
                        position.X -= animoveby;
                    }

                }
                if (currentKBState.IsKeyDown(Keys.Right) & !falling)
                {
                    right = Map.GetTileRight(position, image);
                    if (right == "Nothing" | right == "Ladder")
                    {
                        AnimateRight();
                        position.X += animoveby;
                    }

                }
                if (currentKBState.IsKeyDown(Keys.Down))
                {

                    if (below == "Nothing" | below == "Ladder")
                    {
                        Animateup();
                        position.Y += animoveby;
                    }
                }
                if (currentKBState.IsKeyDown(Keys.Up))
                {
                    above = Map.GetTileAbove(position, image);
                    if (above == "Ladder")
                    {
                        Animateup();
                        position.Y -= animoveby;
                    }

                }

                if (currentKBState.IsKeyDown(Keys.Space) & below != "Nothing")
                {
                    jump = true;
                    falling = false;
                    jumpsound.Play();
                }
            }
            lastState = currentKBState;
        }

        public void Input()
        {

            AtExit = false;
            falling = false;

           var tileb = Map.GetTileB(position.X + (image.Width / 2), position.Y + image.Height); // gets the tile below you
            below = Map.GetTileBelow(position, image);
            above = Map.GetTileAbove(position, image);
            if (above == "Exit") { AtExit = true; }
            if (onplatform) { below = "Solid"; }

            //flying through air animation
            if (hit && !jump && below != "Nothing")
            {
                IsDead = true;
            }

            if (jump & jumpcount > 15 | above != "Nothing")
            {
                jump = false;
                jumpcount = 0;
            }
            //jumping and still moving up
            if (jump & above == "Nothing")
            {
                position.Y -= 5;
                jumpcount++;
            }
            //if on the ground reduce gravity to normal resert fallcount
            if (below == "Solid")
            {
                gravity = 2;
                fallcount = 0;
                if (tileb != null)
                    position.Y = tileb.Ypos - image.Height;
            }
            if (below == "Nothing" && onplatform == false)
            {
                fallcount++;
                if (gravity > maxgravity) { gravity = maxgravity; }
                position.Y += gravity;
                falling = true;
                if (fallcount == 8 & jump == false)
                {
                    gravity++;
                    fallcount = 0;
                }
            }
            if (below == "Death")
            {
                Hit = true;
            }
    
            //  mariooffscreen();
            if (position.Y + image.Height >= Map.Height) position.Y = Map.Height;
            if (position.X + image.Width >= Map.Width) position.X = Map.Width - image.Width;
            if (position.Y < 0) position.Y = 0 ;
            if (position.X < 0) position.X =0;
            
        }

        private void AnimateRight()
        {
           
            if (timer > interval)
            {
                mydirection = Direction.RIGHT;
                if (currentFrame > 8)
                {
                    currentFrame = 0;
                }
                timer = 0f;
                if (fire) { image = mariorectfire[currentFrame]; }
                else
                image = mariorect[currentFrame];
                currentFrame++;
            }
            

        }

        private void AnimateLeft()
        {
           
            if (timer > interval)
            {

                mydirection = Direction.LEFT;
                if (currentFrame > 8)
                {
                    currentFrame = 1;
                }
                timer = 0f;
                if (fire) { image = mariorectfire[currentFrame + 8]; }
                else
                    image = mariorect[currentFrame +8];
                currentFrame++;

            }

        }

        private void Animateup()
        {
            if (timer > interval)
            {
                mydirection = Direction.UP;
                if (currentFrame > 1)
                {
                    currentFrame = 0;
                }
                timer = 0f;
                image = mariorect[currentFrame + 18];
                currentFrame++;

            }

        }


        public void Draw(SpriteBatch spritebatch)
        {

            if (!hit)
            {
                spritebatch.Draw(myimage, position, image, Color.White);
            }
            else
            {

                position.X = position.X - 1;
                if (position.X < 0) position.X = 0;
                spritebatch.Draw(myimage, position, image, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipVertically, 0);
            }

        }
    }
}
