using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace scrollPlatform
{
    internal class TMXload
    {
        private readonly string myPath = string.Empty;
        int rows;
        int cols;
       
        
       
     
       
        readonly List<animate> antilelist = new List<animate>();
        readonly List<gameObjects> gameobjects = new List<gameObjects>();
        
        public TMXload(string fname)
        {
            myPath = fname;
           
        }
        //file name of tile sheet
        public String SpriteSheetSource { get; set; }
        // background image for the map
        public string BackGroundImage { get; set; }
        // tile type by index ie solid death etc
        public string[] TileType { get; set; }
        public string[] TileTypeAnimate { get; set; }
        // individual tile width pixels
        public int tilewidth { get; set; }
        //individual tile height pixels
        public int tileheight { get; set; }
        // tile sheet width pixels
        public int SpriteSheetWidth { get; set; }
        // tile sheet height pixels
        public int SpriteSheetHeight { get; set; }
        // number of tiles across
        public int LayerWidthTiles
        { get { return rows; }  }
        // number of tiles down
        public int LayerHeightTiles
        { get { return cols; } }

        public int ScreenWidth
        {
            get { return LayerWidthTiles * tilewidth; }
        }

        public int ScreenHeight
        {
            get { return LayerHeightTiles * tileheight; }
        }

        public int[,] Map { get; set; }

        public List<animate> anTiles
        {
            get { return antilelist; }
           
        }

        public List<gameObjects> GameObjects
        {
            get { return gameobjects; }
        }

        private void GetRootProperties(XDocument xdoc)
        {

            rows = Convert.ToInt32(xdoc.Root.Attribute("width").Value);
            cols = Convert.ToInt32(xdoc.Root.Attribute("height").Value);

            if (xdoc.Root.Element("properties") != null)
            {
                if (xdoc.Root.Element("properties").Element("property").Attribute("name").Value == "Background")
                {
                    BackGroundImage = xdoc.Root.Element("properties").Element("property").Attribute("value").Value;

                }
            }

        }

        private string RemoveLineEndings(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
        }

        private void GetLayerMap(XDocument xdoc, string name)
        {
            int arow, acol;
            arow = acol = 0;
            var layerList = from el in xdoc.Descendants("layer") select el;
            foreach (var layer in layerList)
            {
                if (layer.Attribute("name").Value == name)
                {
                    int lrows = Convert.ToInt32(layer.Attribute("width").Value);
                    int lcols = Convert.ToInt32(layer.Attribute("height").Value);
                    Map = new int[LayerHeightTiles, LayerWidthTiles];
                    var tempmap = new string[lcols * lrows];
                    string tempstring = RemoveLineEndings(layer.Element("data").Value);
                    tempmap = tempstring.Split(',');
                    foreach (string num in tempmap)
                    {
                        Map[arow, acol] = Int32.Parse(num);

                        acol++;
                        if (acol > LayerWidthTiles - 1)
                        {
                            acol = 0;
                            arow++;

                        }
                    }
                }
            }

        }

        private void GetObjects(XDocument xdoc)
        {
            var objectList = from el in xdoc.Descendants("objectgroup").Descendants("object") select el;

            for (var i = objectList.GetEnumerator(); i.MoveNext();)
            {
                var el = i.Current;
                var go = new gameObjects();
                go.ID = el.Attribute("id").Value;
                go.name = el.Attribute("name").Value;
                go.type = (string)el.Attribute("type");
                go.xpos = (float)el.Attribute("x");
                go.ypos = (float)el.Attribute("y");
                go.width = (int)el.Attribute("width");
                go.height = (int)el.Attribute("height");
                

                var tempcontent = el.Attribute("rotation");

                if (tempcontent != null)
                    go.rotation = (float)el.Attribute("rotation");

                var ob = el.Descendants("properties").Descendants("property");

                if (ob != null)
                {
                    foreach (XElement node1 in ob)
                    {
                        if (node1.Attribute("name").Value == "Content")
                        {
                            go.content = node1.Attribute("value").Value;
                        }
                        if (node1.Attribute("name").Value == "Health")
                        {
                            go.health = (int)node1.Attribute("value");
                        }
                        if (node1.Attribute("name").Value == "Sound")
                        {
                            go.Sound = node1.Attribute("value").Value;
                        }
                        if (node1.Attribute("name").Value == "MoveBy")
                        {
                            go.moveby = (int)node1.Attribute("value");
                        }
                        if (node1.Attribute("name").Value == "movetype")
                        {
                            go.movetype = node1.Attribute("value").Value;
                        }

                    }
                }
                gameobjects.Add(go);
            }
        }

        private void GetTileSet(XDocument xdoc)
        {
            var childList = from tset in xdoc.Descendants("tileset") select tset;

            foreach (var tileXML in childList)
            {
                tilewidth = (int)tileXML.Attribute("tilewidth");
                tileheight = (int)tileXML.Attribute("tileheight");
                int tilecount = (int)tileXML.Attribute("tilecount");
                SpriteSheetSource = (string)tileXML.Element("image").Attribute("source");
                SpriteSheetWidth = (int)tileXML.Element("image").Attribute("width");
                SpriteSheetHeight = (int)tileXML.Element("image").Attribute("height");

                var tileList = from el1 in xdoc.Descendants("tileset").Descendants("tile") select el1;
                TileType = new string[tilecount];
                TileTypeAnimate = new string[tilecount];
                foreach (XElement el1 in tileList)
                {

                    int id = Convert.ToInt32(el1.Attribute("id").Value);
                    // create a List of animate objects for animated tiles
                    if ((string)el1.Element("animation") != null)
                    {
                        animate an = new animate();
                        an.id = id;
                        an.tilenumbers = new int[el1.Elements("animation").Elements("frame").Count()];
                        int count = 0;
                        foreach (XElement prop in el1.Elements("animation").Elements("frame"))
                        {
                            an.tilenumbers[count] = (int)prop.Attribute("tileid");
                            count++;
                        }
                        antilelist.Add(an);
                    }
                    // create string array of tiletypes ie static animate and a string array of types ie solid nothing etc
                    foreach (XElement prop in el1.Elements("properties").Elements("property"))
                    {
                        if ((string)prop.Attribute("name") == "TileType")
                            TileTypeAnimate[id] = (string)prop.Attribute("value").Value;

                        if ((string)prop.Attribute("name") == "Type")
                            TileType[id] = (string)prop.Attribute("value").Value;
                    }
                }
            }

        }
        public void LoadMapBtName(string name)
        {
           // GetLayerMap(xdoc, name);
        }

        public void LoadXMLfile()
        {
            XDocument xdoc = new XDocument();
            xdoc = XDocument.Load(myPath);
            GetRootProperties(xdoc);
            GetTileSet(xdoc);
            GetObjects(xdoc);
            GetLayerMap(xdoc, "Solid"); 
        }
    }
}