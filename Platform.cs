using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scrollPlatform
{
    class MovingPlatform : Sprite

    {
        float starty;
        int moveby = 100;
        int moved;
        int animoveby = 4;
        string movetype = "Vertical";
        private bool direction;


        public MovingPlatform(ContentManager content, gameObjects go) : base(content, go)
        {
            //animationinterval = 50f;
            starty = Position.Y;
            moved = 0;
            if (go.rotation == 1f)
                direction = true;
            else direction = false;
            moveby = go.moveby;
            movetype = go.movetype;
        }

        public bool Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public string MoveType
        {
            get { return movetype; }
        }

        public override void Update(GameTime gameTime)
        {
            //true = down flase = up

            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            moved++;
            if (moved > moveby)
            {
                direction = !direction;
                moved = 0;
            }
          //  string below = Map.GetTileBelow(Position, imageRectange);
            if (timer > animationinterval)
            {
                if (direction)
                {
                    if (movetype == "Vertical")
                    {
                        position.Y += animoveby;
                    }
                    else position.X += animoveby;
                }
                else
                {
                    if (movetype == "Vertical")
                    {
                        position.Y -= animoveby;
                    }
                    else position.X -= animoveby;

                }
                timer = 0f;
            }


        }

    } // end platform cl
}
