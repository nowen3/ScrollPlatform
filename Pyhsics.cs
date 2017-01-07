using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scrollPlatform
{
    static class Pyhsics
    {

        public static Vector2 GetTrajectoryPos(float horizontalVelocity,float initvelocity,float startposX, float totaltime, Vector2 oldposition)
        {
            Vector2 Position;
            Position.X = startposX + horizontalVelocity * totaltime;
            Position.Y = oldposition.Y - ((-(9.81f / 2) * totaltime * totaltime) + (initvelocity * totaltime));

            return Position;

        }
    }
}
