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
using Moxies5.Components.MoxieComponents.Actions;

namespace Moxies5.Controllers
{
    /// <summary>
    /// This is the master controller for all of the systems
    /// The Main Game class calls the methods of this class to run 
    /// all of the game systems
    /// </summary>
    public static class PathfindingController
    {
        #region Fields
        private static Tile[,] _tiles;
        private static int _tilesWide = 0;
        private static int _tilesHigh = 0;
        #endregion

        #region Properties
        public static Tile[,] Tiles
        {
            get
            {
                return _tiles;
            }
        }

        public static int TilesWide
        {
            get
            {
                return _tilesWide;
            }
        }

        public static int TilesHigh
        {
            get
            {
                return _tilesHigh;
            }
        }

        public static bool UsePathHighlights
        {
            get;
            set;
        }
        #endregion

        #region Getters and Setters

        #endregion

        #region Methods

        /// <summary>
        /// Sets up the system controller
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="game"></param>
        public static void Setup(int tilesWide, int tilesHigh)
        {
            UsePathHighlights = false;
            _tilesWide = tilesWide;
            _tilesHigh = tilesHigh;

            _tiles = new Tile[tilesWide, tilesHigh]; //Set up the tile array
            

            //Add tiles to the array
            for (int x = 0; x < tilesWide; x++)
            {
                for (int y = 0; y < tilesHigh; y++)
                {
                    Tile newTile = new Tile(MainController.Game, x, y);
                    _tiles[x, y] = newTile;
                }
            }
        }

        /// <summary>
        /// Updates all of the updateable components
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
        }

        public static void Draw(GameTime gameTime)
        {
            foreach (Tile tile in Tiles)
            {
                tile.Draw(gameTime);
            }
        }

        #endregion

    }
}
