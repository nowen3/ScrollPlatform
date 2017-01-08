using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace scrollPlatform
{
    static class Map
    {

        public static string BackGroundImage { get; set; }
        public static int ID { get; set; }
        static int[,] mapcords;
        static int[,] mapanimatecords;
        static int LayerHeightTiles;
        static int LayerWidthTiles;
        static int tilewidth;
        static int tileheight;
        static string[] TileType;
        static string[] TileTypeAnimate;
        static List<gameObjects> gameobjects;
        static List<Tile> mytiles = new List<Tile>();
        static List<AnimateTile> myanimatetiles = new List<AnimateTile>();
        static List<AnimateTile> antiles = new List<AnimateTile>();
        private static int screenwidth, screenheight;

        public static int Width
        {
            get { return screenwidth; }

        }

        public static int Height
        {
            get { return screenheight; }
        }

        private static ContentManager content;
        public static ContentManager Content
        {
            get { return content; }
            set { content = value; }
        }


        public static void Loadfile(string fname)
        {
            ID = Convert.ToInt32(Path.GetFileNameWithoutExtension(fname));
            gameobjects = new List<gameObjects>();
            //load TMX file
            var tmxmap = new TMXload(fname);
            tmxmap.LoadXMLfile();
            // get game objects
            gameobjects = tmxmap.GameObjects;
            // get tile types ie solid ladder etc
            TileType = tmxmap.TileType;
            // get tile properties ie solid animate
           
            TileTypeAnimate = tmxmap.TileTypeAnimate;
            // get backdround image
            BackGroundImage = tmxmap.BackGroundImage;
            // get map height and width in tiles ie 36*24
            LayerHeightTiles = tmxmap.LayerHeightTiles;
            LayerWidthTiles = tmxmap.LayerWidthTiles;
            // get tile images size
            tilewidth = tmxmap.tilewidth;
            tileheight = tmxmap.tileheight;
            //screenwidth
            screenwidth = tmxmap.ScreenWidth;
            screenheight = tmxmap.ScreenHeight;
            // load tile number into mapcords ie 0=notile
            mapcords = new int[LayerHeightTiles, LayerWidthTiles];
            mapanimatecords = new int[LayerHeightTiles, LayerWidthTiles];
            mapcords = tmxmap.Map;
            var  mapimage = Content.Load<Texture2D>(Path.GetFileNameWithoutExtension(tmxmap.SpriteSheetSource));
            Texture2D[] imagetile;
            imagetile =SpriteUtils.Split(mapimage, tilewidth, tileheight);
            var antiles = tmxmap.anTiles;
            foreach (var at in antiles)
            {
                // create animated tiles for map
                for (int row = 0; row < LayerHeightTiles; row++)
                    for (int col = 0; col < LayerWidthTiles; col++)
                    {
                        if (mapcords[row, col] == at.id + 1)
                        {
                            Texture2D[] tilearray = new Texture2D[at.tilenumbers.Count()];
                            for (int x = 0; x <= at.tilenumbers.Count() - 1; x++)
                            {
                                tilearray[x] = imagetile[at.tilenumbers[x]];
                               
                            }
                            mytiles.Add(new AnimateTile(tilearray, col * tilewidth, tileheight * row, TileType[at.id]));
                            


                        }
                    }
            }


            // create static tiles for map
            for (int row = 0; row < LayerHeightTiles; row++)
                for (int col = 0; col < LayerWidthTiles; col++)
                {
                    if (mapcords[row, col] != 0)
                    {
                        if (TileTypeAnimate[mapcords[row, col] - 1] == "Static")
                        {

                            int imageno = mapcords[row, col];
                            mytiles.Add(new Tile(imagetile[imageno - 1], col * tilewidth, tileheight * row, TileType[imageno - 1]));
                        }
                    }
                }

        }

       
        public static List<gameObjects> GetObjects()
        {
            return gameobjects;
        }


        public static string GetTile(float x, float y)
        {
            string result = "Nothing";
            if (x < 0 || x > Constants.ScreenWidth)
                return "Nothing";
            if (y < 0 || y >= Constants.ScreenHeight)
                return "Nothing";
            double ytile = Math.Ceiling(y / tileheight) - 1;
            double xtile = Math.Ceiling(x / tilewidth) - 1;
            if (ytile < 0)
                ytile = 0;
            if (xtile < 0)
                xtile = 0;

            if (ytile < LayerWidthTiles || xtile < LayerHeightTiles)
            {
                var item = mytiles.FirstOrDefault(o => o.TileAcross == xtile && o.TileDown == ytile);
                if (item != null)
                    result = item.Type;
                return result;
            }
            else
                return result;
        }

        public static Tile GetTileB(float x, float y)
        {
            Tile result = null;
            if (y > screenheight) y =screenheight;
            if (x > screenwidth) x = screenwidth;
            double ytile = Math.Ceiling(y / tileheight) - 1;
            double xtile = Math.Ceiling(x / tilewidth) - 1;
            if (ytile < 0) ytile = 0;
            if (xtile < 0) xtile = 0;
            result = mytiles.FirstOrDefault(o => o.TileAcross == xtile && o.TileDown == ytile);
                return result;
        }

        public static string GetTileBelow(Vector2 position, Rectangle rect)
        {

            return GetTile(position.X + (rect.Width / 2), position.Y + rect.Height);

        }



        public static string GetTileAbove(Vector2 position, Rectangle rect)
        {
            return GetTile(position.X + (rect.Width / 2), position.Y);
        }


        public static string GetTileRight(Vector2 postion, Rectangle rect)
        {
            return GetTile(postion.X + (rect.Width), postion.Y + (rect.Height / 2));
        }

        public static string GetTileLeft(Vector2 postion, Rectangle rect)
        {
            return GetTile(postion.X, postion.Y + (rect.Height / 2));
        }

        public static void Draw(SpriteBatch spritebatch, GameTime gametime)
        {

            foreach (Tile mytile in mytiles)
            {
                if (mytile is AnimateTile)
                {
                    mytile.Update(gametime);

                }

                mytile.Draw(spritebatch);

            }


        }
    }
}
