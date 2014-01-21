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
using Moxies5.Controllers;

namespace Moxies5
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public static class Debugger
    {
        private static List<string> debugStrings = new List<string>();
        private static SpriteFont debugFont;
        private static Vector2 position = Vector2.Zero; //Top left position of the debug box


        static Debugger()
        {
            
        }

        public static void LoadContent(Game game)
        {
            debugFont = game.Content.Load<SpriteFont>("font");
        }

        public static void Initialize(Vector2 _position)
        {
            position = _position;
        }

        //Draws the static debugger stuff
        public static void DrawStatic()
        {
            // TODO: Add your update code here
            float startY = position.Y;
            foreach (string debugStr in debugStrings)
            {
                MainController.StaticSpriteBatch.DrawString(debugFont, debugStr, new Vector2(0, startY * 20), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                startY++;
            }
            debugStrings.Clear();

        }

        public static void AddDebugString(string debugString)
        {
            debugStrings.Add(debugString);
        }
    }
}
