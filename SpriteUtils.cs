using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scrollPlatform
{
    static class  SpriteUtils
    {

        public static Texture2D[] Split(Texture2D original, int partWidth, int partHeight)
        {
            int xCount = original.Width / partWidth;//The number of textures in each horizontal row
            int yCount = original.Height / partHeight;//The number of textures in each vertical column
            Texture2D[] r = new Texture2D[xCount * yCount];//Number of parts = (area of original) / (area of each part).
            int index = 0;
            for (int y = 0; y < yCount; y++)
                for (int x = 0; x < xCount; x++)
                {
                    Rectangle sourceRectangle = new Rectangle(x * partWidth, y * partHeight, partWidth, partHeight);
                    Texture2D cropTexture = new Texture2D(original.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
                    Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
                    original.GetData(0, sourceRectangle, data, 0, data.Length);
                    cropTexture.SetData(data);
                    r[index] = cropTexture;
                    index++;


                }
            return r;
        }

        public static Texture2D SplitSingle(Texture2D original, int partWidth, int partHeight)
        {
           
            Texture2D r;
            
            
                    Rectangle sourceRectangle = new Rectangle(0, 0, partWidth, partHeight);
                    Texture2D cropTexture = new Texture2D(original.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
                    Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
                    original.GetData(0, sourceRectangle, data, 0, data.Length);
                    cropTexture.SetData(data);
                    r = cropTexture;
                    


               
            return r;
        }
    }
}
