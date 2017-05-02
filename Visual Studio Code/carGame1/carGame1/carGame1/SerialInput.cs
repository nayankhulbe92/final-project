using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace carGame1
{
    public class SerialInput
    {
        SerialPort serialPort1 = new SerialPort();
        String comPort = "COM1", Start = "Start"; int num = 1;
        MouseState mouseState; bool started;
        Color fontColor = Color.White;
        MouseState oldMouseState; bool busyState = false;
        int x, y, state = 0;
        float posX;
        public string f = "";
        public string s = "";
        public void Initialize()
        {

        }
        public float PosX
        {
            get { return posX; }
        }
        public int MoveSpeedX
        {
            get { return x; }
        }
        public int MoveSpeedY
        {
            get { return y; }
        }
        public int SerialState
        {
            get { return state; }
            set { state = value; }
        }
        public bool IsBusy
        {
            get { return busyState; }
            set { busyState = value; }
        }
        public void Update(SpriteFont spriteFont)
        {
            mouseState = Mouse.GetState();
            if (mouseState.X > 900 && mouseState.X < 900 + (int)spriteFont.MeasureString(comPort).X && (mouseState.Y > 700 && mouseState.Y < 700 + (int)spriteFont.MeasureString(comPort).Y) && started == false)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                { num++; comPort = "COM" + num; }
                if (mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released && num > 1)
                { num--; comPort = "COM" + num; }
            }
            if (mouseState.X > 900 && mouseState.X < 900 + (int)spriteFont.MeasureString("Start").X && (mouseState.Y > 675 && mouseState.Y < 675 + (int)spriteFont.MeasureString("Start").Y) && started == false)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    num++;
                    started = true;
                    Start = "";
                    fontColor = Color.Red;
                    try
                    {
                        serialPort1.PortName = comPort;
                        serialPort1.BaudRate = 9600;
                        serialPort1.Open();
                        serialPort1.DataReceived += new SerialDataReceivedEventHandler(dataReceived);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not open COM port");
                    }
                }
            }
            oldMouseState = mouseState;
        }

        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                s = s + serialPort1.ReadExisting();
                // WriteData(s);

                if (s.Contains("$") && s.Contains("!"))
                {
                    if (s.IndexOf('$') < s.IndexOf('!'))
                    {
                        f = s.Substring(s.IndexOf('$') + 1, s.IndexOf('!') - s.IndexOf('$') - 1);
                        String[] parameters = f.Split(',');
                        if (parameters.Length == 3 && busyState == false)
                        {
                            state = 1;
                            x = Int32.Parse(parameters[1]);
                            y = Int32.Parse(parameters[2]);
                        }
                        else if (parameters.Length == 2)
                        {
                            state = 2;
                            if (float.Parse(parameters[1]) > 250)
                            posX = float.Parse(parameters[1]);
                        }
                        else
                        {
                            state = 0;
                        }
                    }
                    s = s.Substring(s.IndexOf('!') + 1);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.DrawString(spriteFont, comPort, new Vector2(900, 700), fontColor);
            spriteBatch.DrawString(spriteFont, Start, new Vector2(900, 675), fontColor);
        }

    }
}
