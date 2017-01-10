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
       // Player player;
        
       
        int score = 0;
        int monsterscore = 0;
        int totalcoins;
        int totalmonsters;
        int level;
        int mylives;
        SpriteFont font;
        bool pause;
        bool gotkey = false;
        Camera camera;
        KeyboardState lastState;
        SpriteManager spriteManager;



        List<gameObjects> gameobs = new List<gameObjects>();
      

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

        private void NewLevel()
        {
            level++;
            score = 0;
            monsterscore = 0;
            if (File.Exists(appPath + "\\" + level + ".tmx"))
            {
                LoadGame(appPath + "\\" + level + ".tmx");
                spriteManager.LoadSprites(gameobs);
            }

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
            spriteManager = new SpriteManager(Content);
            NewGame();

        }

    
        private void NewGame()
        {
            spriteManager.LoadSprites(gameobs);
        }

        private void RestartGame()
        {
            score = 0;
            monsterscore = 0;
            Map.ID = 0;
            mylives = 3;
            LoadGame(appPath + "\\" + level + ".tmx");
            NewGame();
        }

        private void LoadGame(string TMXfile)
        {

            Map.Content = Content;
            Map.Loadfile(TMXfile);
            gameobs = Map.GetObjects();
            Background = Content.Load<Texture2D>(Map.BackGroundImage);
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
            if (mylives < 0)
            {
                RestartGame();
            }
            if (!pause)
            {
                spriteManager.Update(gameTime);
                camera.Update(spriteManager.PlayerPosition, Map.Width, Map.Height);
               if (spriteManager.PlayerDead)
                {
                    mylives--;
                    NewGame();
                }
                if (spriteManager.AtExit)
                {

                    NewLevel();
                }
                base.Update(gameTime);
            } // end pause
        }

              
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                                null, null, null, null,
                                camera.Transform);
            spriteBatch.Draw(Background, Vector2.Zero);

            Map.Draw(spriteBatch, gameTime);
            spriteManager.Draw(spriteBatch);
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