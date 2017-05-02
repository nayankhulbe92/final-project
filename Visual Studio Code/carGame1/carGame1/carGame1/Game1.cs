using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace carGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background,staticBackground;
        private ScrollingBackground myBackground,slowBackground;
        Car car1 = new Car();
        SerialInput serial = new SerialInput();
        List<Car> obsatcles = new List<Car>();
        Random rnd = new Random();
        Physics phy = new Physics();
        int scrollSpeed = 0;
        float obstacleSpeed = 0;
        String GameOver = "";
        int score=0;
        SpriteFont spriteFont;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferMultiSampling = false;
            graphics.IsFullScreen = true;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            car1.Initialize(new Vector2(350,600));
            for (int i = 1; i <= 2; i++)
            {
                Car obs = new Car();
                int x = rnd.Next(320, 680);
                int y = rnd.Next(-350 * i, (-400 * (i-1)));
                obs.Initialize(new Vector2(x, y));
                obsatcles.Add(obs);
            }
            base.Initialize();
        }
        protected void Reset()
        {
            score = 0;
            car1.Initialize(new Vector2(350, 600));
            obsatcles.Clear();
            for (int i = 1; i <= 2; i++)
            {
                Car obs = new Car();
                int x = rnd.Next(320, 680);
                int y = rnd.Next(-350 * i, (-400 * (i - 1)));
                obs.Initialize(new Vector2(x, y));
                obsatcles.Add(obs);
            }
            obstacleSpeed = 0;
            scrollSpeed = 0;
            for (int i = 0; i < 2; i++)
            {
                obsatcles[i].LoadContent(Content);
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            myBackground = new ScrollingBackground();
            slowBackground = new ScrollingBackground();
            background = Content.Load<Texture2D>("Sprites\\road1");
            spriteFont = Content.Load<SpriteFont>("Fonts\\SpriteFont1");
            myBackground.Load(GraphicsDevice, background);
            staticBackground = Content.Load<Texture2D>("Sprites\\road copy");
            slowBackground.Load(GraphicsDevice, staticBackground);
            car1.LoadContent(Content);
            for (int i = 0; i < 2; i++)
            {
                obsatcles[i].LoadContent(Content);
            }
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            serial.Update(spriteFont);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyState=Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Right)&& car1.Position.X<650)
                car1.Position = new Vector2(car1.Position.X + 10, car1.Position.Y);
            if (keyState.IsKeyDown(Keys.Left) && car1.Position.X > 300)
                car1.Position = new Vector2(car1.Position.X - 10, car1.Position.Y);
            if(serial.SerialState==2)
            {
                car1.Position = new Vector2(serial.PosX, car1.Position.Y);
            }
            car1.Update(gameTime, 0);
            if(keyState.IsKeyDown(Keys.Up))
            {
                scrollSpeed += 10;
                if (obstacleSpeed < 0)
                    obstacleSpeed = 0;
                if (scrollSpeed > 1600)
                    scrollSpeed = 1600;
                else
                obstacleSpeed += 0.1f;
                GameOver = "";
            }
            if(keyState.IsKeyDown(Keys.Down))
            {
                scrollSpeed -= 10;

                if (scrollSpeed < 0)
                {
                    scrollSpeed = 0;
                    obstacleSpeed = -1f;
                }
                else
                    obstacleSpeed -= 0.1f;
            }
            // TODO: Add your game logic here.
            myBackground.Update(elapsed * scrollSpeed);
            // TODO: Add your update logic here
            slowBackground.Update(elapsed * scrollSpeed);
            for(int i=0;i<obsatcles.Count;i++)
            {
                obsatcles[i].Update(gameTime,obstacleSpeed);
                if(obsatcles[i].Position.Y>768)
                {
                    int x = rnd.Next(300, 660);
                    int y = rnd.Next(-700, -(int)(car1.Height*0.25f)-10);
                    obsatcles[i].Position = new Vector2(x, y);
                    score += 1;
                }
                if(obsatcles[i].Position.Y<-700)
                {
                    int x = rnd.Next(300, 660);
                    int y = rnd.Next((int)(car1.Height * 0.25f) + 10, 700);
                    obsatcles[i].Position = new Vector2(x, y);
                }
                /*if(phy.TexturesCollide(car1.Tex,car1.Transform,obsatcles[i].Tex,obsatcles[i].Transform))
                {
                    Console.WriteLine("Collision");
                }*/
                Rectangle a = car1.getBoundary();
                Rectangle b = obsatcles[i].getBoundary();
                if(a.Intersects(b))
                { 
                    //Vector2 point=phy.pixelCollisionPoint(car1.Frame, obsatcles[i].Frame, a, b);
                    if(phy.TexturesCollide(car1.Tex,car1.Transform,obsatcles[i].Tex,obsatcles[i].Transform))
                    {
                        GameOver = "Game Over";
                        Reset();
                    }
                    //Console.WriteLine(point.X + "  " + point.Y);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            slowBackground.Draw(spriteBatch);
            
            myBackground.Draw(spriteBatch);

            car1.Draw(spriteBatch);
            for (int i = 0; i < 2; i++)
            {
                obsatcles[i].Draw(spriteBatch,Color.CadetBlue);
            }
            spriteBatch.DrawString(spriteFont, GameOver, new Vector2(300, 300), Color.White);
            spriteBatch.DrawString(spriteFont, "Score : " + score, new Vector2(0, 0), Color.White);
            serial.Draw(spriteBatch, spriteFont);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
