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

namespace Moxies5.Utilities
{
    public class AnimationPlayer
    {
        #region Fields
        private int framecount;
        private Texture2D myTexture;
        private float TimePerFrame;
        private int Frame;
        private float TotalElapsed;
        private bool Paused;
        public bool Flipped;
        private SpriteEffects effect;
        public Color Color;
        public float Rotation, Scale, Depth;
        public Vector2 Origin;
        #endregion

        #region Getters and Setters
        public int Height()
        {
            return myTexture.Height;
        }

        public int Width()
        {
            return myTexture.Width;
        }

        public int FrameCount()
        {
            return framecount;
        }

        public void FramesPerSecond(int FramesPerSec)
        {
            TimePerFrame = (float)1 / FramesPerSec;
        }

        public bool IsPaused
        {
            get { return Paused; }
        }

        public int GetCurrentFrame()
        {
            return Frame;
        }

        public Texture2D GetTexture()
        {
            return myTexture;
        }

        #endregion

        public AnimationPlayer(Vector2 Origin, float Rotation, float Scale, float Depth)
        {
            this.Origin = Origin;
            this.Rotation = Rotation;
            this.Scale = Scale;
            this.Depth = Depth;
            this.Color = Color.White;
        }

        public void Load(GraphicsDevice device, ContentManager content, string asset, int FrameCount, int FramesPerSec)
        {
            framecount = FrameCount;
            myTexture = content.Load<Texture2D>(asset);
            TimePerFrame = (float)1 / FramesPerSec;
            Frame = 0;
            TotalElapsed = 0;
            Paused = false;
        }

        // class AnimatedTexture
        public void UpdateFrame(float elapsed)
        {
            if (Paused)
                return;
            TotalElapsed += elapsed;
            if (TotalElapsed > TimePerFrame)
            {
                Frame++;
                // Keep the Frame between 0 and the total frames, minus one.
                Frame = Frame % framecount;
                TotalElapsed -= TimePerFrame;
            }
        }

        // class AnimatedTexture
        public void DrawFrame(SpriteBatch Batch, Vector2 screenpos)
        {
            DrawFrame(Batch, Frame, screenpos);
        }

        public void DrawFrame(SpriteBatch Batch, int Frame, Vector2 screenpos)
        {
            if (Flipped)
                effect = SpriteEffects.FlipHorizontally;
            else
                effect = SpriteEffects.None;
            int FrameWidth = myTexture.Width / framecount;
            Rectangle sourcerect = new Rectangle(FrameWidth * Frame, 0,
                FrameWidth, myTexture.Height);
            Batch.Draw(myTexture, screenpos, sourcerect, Color,
                Rotation, Origin, Scale, effect, Depth);
        }



        public void Reset()
        {
            Frame = 0;
            TotalElapsed = 0f;
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Play()
        {
            Paused = false;
        }

        public void Pause()
        {
            Paused = true;
        }



        public static Texture2D Crop(Texture2D source, Rectangle area)
        {
            if (source == null)
                return null;

            Texture2D cropped = new Texture2D(source.GraphicsDevice, area.Width, area.Height);
            Color[] data = new Color[source.Width * source.Height];
            Color[] cropData = new Color[cropped.Width * cropped.Height];

            source.GetData<Color>(data);

            int index = 0;
            for (int y = area.Y; y < area.Y + area.Height; y++)
            {
                for (int x = area.X; x < area.X + area.Width; x++)
                {
                    cropData[index] = data[x + (y * source.Width)];
                    index++;
                }
            }

            cropped.SetData<Color>(cropData);

            return cropped;
        }



        public static Texture2D Flip(Texture2D source, bool vertical, bool horizontal)
        {
            Texture2D flipped = new Texture2D(source.GraphicsDevice, source.Width, source.Height);
            Color[] data = new Color[source.Width * source.Height];
            Color[] flippedData = new Color[data.Length];

            source.GetData<Color>(data);

            for (int x = 0; x < source.Width; x++)
                for (int y = 0; y < source.Height; y++)
                {
                    int idx = (horizontal ? source.Width - 1 - x : x) + ((vertical ? source.Height - 1 - y : y) * source.Width);
                    flippedData[x + y * source.Width] = data[idx];
                }

            flipped.SetData<Color>(flippedData);

            return flipped;
        }

        public Texture2D Flip(bool vertical, bool horizontal)
        {
            Texture2D flipped = new Texture2D(myTexture.GraphicsDevice, myTexture.Width, myTexture.Height);
            Color[] data = new Color[myTexture.Width * myTexture.Height];
            Color[] flippedData = new Color[data.Length];

            myTexture.GetData<Color>(data);

            for (int x = 0; x < myTexture.Width; x++)
                for (int y = 0; y < myTexture.Height; y++)
                {
                    int idx = (horizontal ? myTexture.Width - 1 - x : x) + ((vertical ? myTexture.Height - 1 - y : y) * myTexture.Width);
                    flippedData[x + y * myTexture.Width] = data[idx];
                }

            flipped.SetData<Color>(flippedData);

            return flipped;
        }

    }
}