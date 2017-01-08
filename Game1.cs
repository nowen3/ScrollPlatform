using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace scrollPlatform
{
    public struct animate
    {
        public int id;
        public int[] tilenumbers;
    }

    enum Direction {LEFT , RIGHT, UP, DOWN };
    enum Owner { FOE, PLAYER}; 

    public struct gameObjects
    {

        public string name;
        public string ID;
        public float xpos;
        public float ypos;
        public string content;
        public string Sound;
        public string type;
        public int width;
        public int height;
        public int health;
        public int moveby;
        public float rotation;
        public string movetype;
    }
    public class Game1 : Game
    {
        private string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Content";
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D Background;
        Texture2D lives;
        Map mymap;
        Player player;
        
       
        int score = 0;
        int monsterscore = 0;
        int totalcoins;
        int totalmonsters;
        int level;
        int mylives;
        int firecount;
        double missilletimer;
        SpriteFont font;
        bool pause;
        bool gotkey = false;
        Camera camera;
        KeyboardState lastState;



        List<gameObjects> gameobs = new List<gameObjects>();
        List<Sprite> foes = new List<Sprite>();
        List<Missile> missile = new List<Missile>();
        List<Explosion> myexplsion = new List<Explosion>();
        List<Dead> mydead = new List<Dead>();
        List<HealthAnimate> myhealth = new List<HealthAnimate>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 600;   // set this value to the desired height of your window
            graphics.ApplyChanges();
           
            base.Initialize();
        }

        protected override void LoadContent()
        {
            level = 32;
            pause = false;
            mylives = 3;
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new Camera(GraphicsDevice.Viewport);
            lives = Content.Load<Texture2D>("lives");
            font = Content.Load<SpriteFont>("numbers");
            LoadGame(appPath + "\\"+ level +".tmx");
            LoadOjects(gameobs);
           
            NewGame();

        }

    
        private void NewGame()
        {
           LoadOjects(gameobs);
            firecount = 0;
           
        }

        private void RestartGame()
        {
            score = 0;
            monsterscore = 0;
            mymap.ID = 0;
            mylives = 3;
            LoadGame(appPath + "\\" + level + ".tmx");
            NewGame();
        }

        private void LoadGame(string TMXfile)
        {

            var mapimage = Content.Load<Texture2D>("TileSet32 x 32");
            mymap = new Map();
            mymap.Content = Content;
            mymap.Loadfile(TMXfile);
            gameobs = mymap.GetObjects();
            
 
        }

        private void LoadOjects(List<gameObjects> gameobs)
        {

            foes.Clear();
            foreach (gameObjects go in gameobs)
            {
                if (go.name == "StartPoint")
                {
                    player = new Player(Content,go);
                    player.IsDead = false;
                    player.Hit = false;
                    player.Position = new Vector2(go.xpos, go.ypos);
                    player.MyMap = mymap;
                }
                if (go.name == "Eye")
                {
                    foes.Add(new enemie(Content, go));
                    totalmonsters++;
                }
                if (go.name == "Biggun")
                {
                    foes.Add(new BigGun(Content, go));
                    totalmonsters++;
                }
                if (go.name == "Drone")
                {
                    foes.Add(new Drone(Content, go));
                    totalmonsters++;
                }
            }
        }



        protected override void UnloadContent()
        {
            Content.Unload();
        }

        private void GetInput()
        {
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.Escape))
                Exit();
            if (newState.IsKeyDown(Keys.P) && !lastState.IsKeyDown(Keys.P)) { pause = !pause; }
            if (newState.IsKeyDown(Keys.Enter) && !lastState.IsKeyDown(Keys.Enter))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            lastState = newState;
        }

       
        protected override void Update(GameTime gameTime)
        {
             GetInput();
            missilletimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (mylives < 0)
            {
                RestartGame();
            }
            if (!pause)
            {
                
                foreach (Sprite foe in foes)
                {
                    if (foe is BigGun)
                    {
                        if ((foe as BigGun).Fire & missile.Count < 10)
                        {
                            if ((foe as BigGun).FireDirection == Direction.LEFT)
                                missile.Add(new Missile(Content.Load<Texture2D>("Missile"), foe.Position, Direction.RIGHT, true)); // back to front
                            else
                                missile.Add(new Missile(Content.Load<Texture2D>("Missile"), foe.Position, Direction.LEFT, true));
                        }
                    }
                    if (foe is Drone)
                    {
                        
                        if ((foe as Drone).Fire & missile.Count < 10)
                        {
                            var bombpos = new Vector2(foe.Position.X + (foe.BoundingBox.Width/2), foe.Position.Y + foe.BoundingBox.Height);
                            missile.Add(new Missile(Content.Load<Texture2D>("bomb"), bombpos , Direction.DOWN, false));
                            
                        }
                    }
                    foe.Update(gameTime, mymap);
                }
                if (!player.fire) { firecount = 0; }
                if (player.fire && missilletimer > 1000)
                {
                    
                    firecount++;
                    //Debug.WriteLine(firecount);
                    if (player.PlayerDirection == Direction.LEFT)
                    {
                        var playerpos = new Vector2(player.Position.X - 30, player.Position.Y );
                        missile.Add(new Missile(Content.Load<Texture2D>("Missile1"), playerpos, Direction.LEFT, false, 100 + (firecount * 20), Owner.PLAYER));
                    }
                    if (player.PlayerDirection == Direction.RIGHT)
                    {
                        var playerpos = new Vector2(player.Position.X + 30, player.Position.Y + 40);
                        var miss = new Missile(Content.Load<Texture2D>("Missile1"), playerpos, Direction.RIGHT, false, 100 + (firecount * 20), Owner.PLAYER);
                        miss.RotateMissile = true;
                        missile.Add(miss);
                    }
                    missilletimer = 0;
                }
                
                foreach (Dead dead in mydead)
                {
                    dead.Update(gameTime);
                }
                foreach(HealthAnimate ha in myhealth)
                {
                    ha.Update(gameTime);
                }
                foreach (Missile miss in missile)
                {
                    miss.Update(gameTime);
                    if (mymap.GetTileBelow(miss.Position, miss.BoundingBox) == "Solid")
                    {
                        miss.Hit = true;
                        myexplsion.Add(new Explosion(Content, miss.Position));
                    }
                }
                foreach (Explosion exp in myexplsion)
                {
                    exp.Update(gameTime);
                }
                CheckCollision();
                player.Update(gameTime);
                player.Input( );
                
                camera.Update(player.Position, mymap.Width, mymap.Height);
                foes.RemoveAll(x => x.Hit == true);
                myexplsion.RemoveAll(x => x.Hit == true);
                myhealth.RemoveAll(x => x.Hit == true);
                mydead.RemoveAll(x => x.Hit == true);
                missile.RemoveAll(x => x.Hit == true);
                if (player.IsDead)
                {
                    mylives--;
                    NewGame();
                }
                base.Update(gameTime);
            } // end pause
        }

        private void CheckCollision()
        {
            foreach (Sprite foe in foes)
            {
                if (foe.BoundingBox.Intersects(player.BoundingBox) && foe.Type == "Foe")
                {

                    if (player.BoundingBox.Bottom + (foe.BoundingBox.Height / 2) <= foe.BoundingBox.Bottom)
                    {
                        foe.Hit = true;
                        monsterscore++;
                        mydead.Add(new Dead(foe.GetImage, foe.Position));
                    }
                    else
                    {
                        if (!player.Hit)
                        {
                            player.Hit = true;

                        }
                    }
                }
            }

            foreach (Missile miss in missile)
            {
                if (miss.BoundingBox.Intersects(player.BoundingBox) && miss.Parent == Owner.FOE)
                {
                    if (!player.Hit)
                    {
                        player.Hit = true;

                    }
                }
               // Debug.WriteLine(miss.Parent);
                if (miss.Parent == Owner.PLAYER)
                {
                    foreach (Sprite foe in foes)
                    {
                        if (miss.BoundingBox.Intersects(foe.BoundingBox))
                        {
                            foe.Health = 100;
                            miss.Hit = true;
                            myexplsion.Add(new Explosion(Content, miss.Position));
                            myhealth.Add(new HealthAnimate(Content, miss.Position, foe.Health.ToString()));
                            Debug.WriteLine(foe.Health);
                        }

                    }
                }
            }
            foreach (Explosion exp in myexplsion)
            {
                if (exp.BoundingBox.Intersects(player.BoundingBox))
                {
                    if (!player.Hit)
                    {
                        player.Hit = true;

                    }
                }
            }

        }

       
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                                null, null, null, null,
                                camera.Transform);

            mymap.Draw(spriteBatch, gameTime);
            player.Draw(spriteBatch);
            foreach (Sprite foe in foes)
                foe.Draw(spriteBatch);
            foreach (Dead dead in mydead)
                dead.Draw(spriteBatch);
            foreach (Missile miss in missile)
                miss.Draw(spriteBatch);
            foreach (Explosion exp in myexplsion)
                exp.Draw(spriteBatch);
            foreach (HealthAnimate ha in myhealth)
                ha.Draw(spriteBatch);


            spriteBatch.End();

            spriteBatch.Begin();
            for (int x = 1; x <= mylives; x++)
            {
                spriteBatch.Draw(lives, new Rectangle((x * lives.Width) + 55, 5, lives.Width, lives.Height), Color.White);
            }
            spriteBatch.DrawString(font, "Lives ", new Vector2(5, 10), Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}