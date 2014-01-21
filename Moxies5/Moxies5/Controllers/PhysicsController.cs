using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Moxies5.Components;
using Moxies5.Entities;
using Moxies5.Utilities;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.DebugViews;


namespace Moxies5.Controllers
{
    /// <summary>
    /// This is the master controller for all of the systems
    /// The Main Game class calls the methods of this class to run 
    /// all of the game systems
    /// </summary>
    public static class PhysicsController
    {
        #region Fields
        private static bool _debugEnabled = false;
        private static DebugViewXNA _debugViewXNA;
        private static List<Body> _bodyRemoveList = new List<Body>();

        #endregion

        #region Properties
        public readonly static World World = new World(Vector2.Zero);

        #endregion

        #region Methods

        /// <summary>
        /// Sets up the system controller
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="game"></param>
        public static void Setup()
        {
            _debugViewXNA = new DebugViewXNA(World);
            _debugViewXNA.LoadContent(MainController.GraphicsDevice, MainController.Content);
            _debugViewXNA.AppendFlags(DebugViewFlags.Shape);
            _debugViewXNA.AppendFlags(DebugViewFlags.PolygonPoints);
            _debugViewXNA.AppendFlags(DebugViewFlags.Joint);
        }


        /// <summary>
        /// Updates all of the updateable components
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
            //World step goes here

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds * 0.001f; //Might change the stepping time

            World.Step(elapsed); //Step the physics simulation

            for (int i = 0; i < _bodyRemoveList.Count; i++)
            {
                _bodyRemoveList[i].Dispose();
            }
            _bodyRemoveList.Clear();

            if(InputHandler.KeyPressed(Keys.F1))
            {
                if(_debugEnabled)
                    _debugEnabled = false;
                else
                    _debugEnabled = true;
            }
        }

        public static void Draw(GameTime gameTime)
        {
            if (_debugEnabled)
            {
                Matrix proj = MainController.Camera.ToFarseerProjection(MainController.DefaultScreenWidth, MainController.DefaultScreenHeight);
                Matrix view = MainController.Camera.ToFarseerView();
                _debugViewXNA.RenderDebugData(ref proj, ref view);
            }
        }

        public static void Reset()
        {
            World.Clear();
        }

        public static void RemoveBody(Body rBody)
        {
            _bodyRemoveList.Add(rBody);
        }

        public static bool BodyRemoveListContains(Body rBody)
        {
            if (_bodyRemoveList.Contains(rBody))
                return true;
            return false;
        }

        #endregion

    }
}
