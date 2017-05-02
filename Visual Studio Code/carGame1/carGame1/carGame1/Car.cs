using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
namespace carGame1
{
    public class Car
    {
        Texture2D carImage;
        Vector2 carPosition,Origin;
        Matrix trans;
        Rectangle sourceRect;
        float scale = 0.25f;
        Color[] frame;
        Color[,] colors2D;
        public Vector2 Position
        {
            get { return carPosition; }
            set { carPosition = value; }
        }
        public int Width
        {
            get { return carImage.Width; }
        }
        public int Height
        {
            get { return carImage.Height; }
        }
        public Color[] Frame
        {
            get { return frame; }
            set { frame = value; }
        }
        public Matrix Transform
        {
            get { return trans; }
        }
        public Color[,] Tex
        {
            get { return colors2D; }
        }
        public void UpdateTransform()
        {
            trans = Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(new Vector3(carPosition, 0.0f));
        }
        public void Initialize(Vector2 position)
        {
            carPosition = position;
        }
        public void LoadContent(ContentManager Content)
        {
            carImage = Content.Load<Texture2D>("Sprites\\car1");
            Color[] data = new Color[carImage.Width * carImage.Height];
            carImage.GetData<Color>(data);
            Frame = data;
            colors2D = new Color[carImage.Width, carImage.Height];
            for (int x = 0; x < carImage.Width; x++)
                for (int y = 0; y < carImage.Height; y++)
                    colors2D[x, y] = data[x + y * carImage.Width];
        }
        public void Update(GameTime gameTime,float speed)
        {
            UpdateTransform();
            carPosition.Y += speed;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(0,0,carImage.Width,carImage.Height);
            spriteBatch.Draw(carImage, carPosition,sourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        public void Draw(SpriteBatch spriteBatch,Color color)
        {
            Rectangle sourceRect = new Rectangle(0, 0, carImage.Width, carImage.Height);
            spriteBatch.Draw(carImage, carPosition, sourceRect, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        public Rectangle getBoundary()
        {
            Rectangle a = new Rectangle((int)carPosition.X, (int)carPosition.Y, (int)(carImage.Width * scale), (int)(carImage.Height * scale));
            return a;
        }
    }
}
