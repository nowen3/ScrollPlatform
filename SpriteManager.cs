using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace scrollPlatform
{
    class SpriteManager
    {
        private Player player;
        private List<Sprite> foes = new List<Sprite>();
        private List<Missile> missile = new List<Missile>();
        private List<Dead> mydead = new List<Dead>();
        private List<HealthAnimate> myhealth = new List<HealthAnimate>();
        private bool playerdead, atexit;
        private double missilletimer;

        ContentManager content;
        public void LoadSprites(List<gameObjects> gameobs)
        {
            ClearLists();
            ResetVariables();
            foreach (gameObjects go in gameobs)
            {
                if (go.name == "StartPoint")
                {
                    if (player == null)
                    player = new Player(content, go);
                    player.IsDead = false;
                    player.Hit = false;
                    player.Position = new Vector2(go.xpos, go.ypos);
                }
                if (go.name == "Eye") { foes.Add(new SmallBot(content, go)); }
                if (go.name == "Biggun") { foes.Add(new BigGun(content, go)); }
                if (go.name == "Drone") { foes.Add(new Drone(content, go)); }
                if (go.name == "RoofLaser") { foes.Add(new Rooflaser(content, go)); }
            }
        }

      public bool PlayerDead
        {
            get {return playerdead; }
            set { playerdead = value; }
        }

        public bool AtExit
        {
            get { return atexit; }
            set { atexit = value; }
        }
        public void ClearLists()
        {
            foes.Clear();
            missile.Clear();
            mydead.Clear();
            myhealth.Clear();
        }

        private void ResetVariables()
        {
           
            playerdead = false;
        }

        private void RemoveHitSprites()
        {
            foes.RemoveAll(x => x.Hit == true);
            missile.RemoveAll(x => x.Hit == true);
            myhealth.RemoveAll(x => x.Hit == true);
            mydead.RemoveAll(x => x.Hit == true);
        }

        public Vector2 PlayerPosition
            {
            get {return player.Position; }
            }

        public SpriteManager(ContentManager Content)
        {
            content = Content;
            ResetVariables();
            ClearLists();
        }

        public void Update(GameTime gameTime)
        {
            // update foes----------------------------------
            foreach (Sprite foe in foes)
            {
                if (foe is BigGun)
                {
                    if ((foe as BigGun).Fire & missile.Count < 10)
                    {

                        if ((foe as BigGun).FireDirection == Direction.LEFT)
                            missile.Add(new Missile(content.Load<Texture2D>("Missile"), foe.Position, Direction.RIGHT, true)); // back to front
                        else
                            missile.Add(new Missile(content.Load<Texture2D>("Missile"), foe.Position, Direction.LEFT, true));
                    }
                }
                if (foe is Drone)
                {

                    if ((foe as Drone).Fire & missile.Count < 10)
                    {
                        var bombpos = new Vector2(foe.Position.X + (foe.BoundingBox.Width / 2), foe.Position.Y + foe.BoundingBox.Height);
                        missile.Add(new Missile(content.Load<Texture2D>("bomb"), bombpos, Direction.DOWN, false));

                    }
                }
                foe.Update(gameTime);
            }
            // update player--------------------------------------------
            missilletimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            
            if (player.Fire && missilletimer > 1000)
            {

                
                if (player.PlayerDirection == Direction.LEFT)
                {
                    var playerpos = new Vector2(player.Position.X - 30, player.Position.Y);
                    missile.Add(new Missile(content.Load<Texture2D>("Missile1"), playerpos, Direction.LEFT, false, 100 + player.FireStrength, Owner.PLAYER));
                }
                if (player.PlayerDirection == Direction.RIGHT)
                {
                    var playerpos = new Vector2(player.Position.X + 30, player.Position.Y + 40);
                    var miss = new Missile(content.Load<Texture2D>("Missile1"), playerpos, Direction.RIGHT, false, 100 + player.FireStrength , Owner.PLAYER);
                    miss.RotateMissile = true;
                    missile.Add(miss);
                }
                missilletimer = 0;
            }
            //update misc---------------------------------------------
            foreach (Missile miss in missile)
            {
                miss.Update(gameTime);
                if (Map.GetTileBelow(miss.Position, miss.BoundingBox) == "Solid")
                {
                    miss.Hit = true;

                    foes.Add(new Explosion(content, GetExplosion(miss)));
                }
            }

            foreach (Dead dead in mydead)
            {
                dead.Update(gameTime);
            }
            foreach (HealthAnimate ha in myhealth)
            {
                ha.Update(gameTime);
            }
            if (player.IsDead)
            {
                playerdead = true;
            }
            if (player.AtExit)
            {
                atexit = true;
            }
            // end update-----------------------------------------------
            CheckCollision();
            player.Update(gameTime);
            player.Input();
            RemoveHitSprites();
        }

        private void CheckCollision()
        {
            foreach (Sprite foe in foes)
            {
                if (foe.BoundingBox.Intersects(player.BoundingBox) && foe.Type == "Foe")
                {

                    if (player.BoundingBox.Bottom + (foe.BoundingBox.Height / 2) <= foe.BoundingBox.Bottom)
                    {
                        if ((foe is Rooflaser) == false)
                        {
                            foe.Hit = true;
                            mydead.Add(new Dead(foe.GetImage, foe.Position));
                        }
                    }
                    else if (!player.Hit)
                    {
                        if ((foe is Rooflaser) && (foe as Rooflaser).Active == false)
                        {
                            // do nothing
                        }
                        else
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

                if (miss.Parent == Owner.PLAYER)
                {
                    foreach (Sprite foe in foes)
                    {
                        if (miss.BoundingBox.Intersects(foe.BoundingBox) && (foe is Explosion) == false)
                        {
                            foe.Health = 100;
                            miss.Hit = true;
                            foes.Add(new Explosion(content, GetExplosion(miss)));
                            myhealth.Add(new HealthAnimate(content, miss.Position, foe.Health.ToString()));
                            break;

                        }

                    }
                }
            }

        }

        private gameObjects GetExplosion(Missile sprite)
        {
            var go = new gameObjects();
            go.content = "explosion";
            go.xpos = sprite.Position.X;
            go.ypos = sprite.Position.Y;
            go.Sound = "bomb-sound";
            go.height = 50;
            go.width = 50;
            return go;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            player.Draw(spritebatch);
            foreach (Sprite foe in foes)
                foe.Draw(spritebatch);
            foreach (Dead dead in mydead)
                dead.Draw(spritebatch);
            foreach (Missile miss in missile)
                miss.Draw(spritebatch);
            foreach (HealthAnimate ha in myhealth)
                ha.Draw(spritebatch);

        }
    }
}
