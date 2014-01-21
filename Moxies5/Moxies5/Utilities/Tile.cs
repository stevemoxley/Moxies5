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


namespace Moxies5.Utilities
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Tile : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields
        private Vector2 _location; //This is the location in X,Y on the grid NOT IN PIXELS
        public const int tileWidth = 45;
        public const int tileHeight = 45;
        private bool _blocked = false;
        private bool _highlighted = false;
        private Texture2D gridTexture;
        private Texture2D highlightTexture;
        public Rectangle Rect { get; set; }

        //Pathfinding variables. Do not save
        public float cost = 0;
        public float heuristic = 0;
        public float total = 0;
        public bool diagonal = false;
        public Tile parent = null;

        #endregion

        #region Properties
        public bool Blocked
        {
            get
            {
                return _blocked;
            }
            set
            {
                _blocked = value;
            }

        }

        /// <summary>
        /// This is the alt tile a moxie can travel to if this tile is blocked
        /// </summary>
        public bool CanBeUsedAsTarget
        { get; set; }

        /// <summary>
        /// The location of the tile on the grid. NOT PIXEL
        /// </summary>
        public Vector2 Location
        {
            get
            {
                return _location;
            }
        }
        #endregion


        public Tile(Game game, int locationX, int locationY)
            : base(game)
        {
            this._location = new Vector2(locationX, locationY);
            Rect = new Rectangle(locationX * tileWidth, locationY*tileHeight, tileWidth, tileHeight);
            // TODO: Construct any child components here
            gridTexture = MainController.Game.Content.Load<Texture2D>("grid");
            highlightTexture = MainController.Game.Content.Load<Texture2D>("barpixel");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #region Drawing
        public override void Draw(GameTime gameTime)
        {
            MainController.CameraSpriteBatch.Draw(gridTexture, new Vector2(_location.X * tileWidth, _location.Y * tileHeight), new Rectangle(0, 0, gridTexture.Width, gridTexture.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            if (_highlighted)
            {
                MainController.CameraSpriteBatch.Draw(highlightTexture, new Vector2(_location.X * tileWidth, _location.Y * tileHeight), new Rectangle(0, 0, tileWidth, tileHeight), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            if (_blocked)
            {
                MainController.CameraSpriteBatch.Draw(highlightTexture, new Vector2(_location.X * tileWidth, _location.Y * tileHeight), new Rectangle(0, 0, tileWidth, tileHeight), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            base.Draw(gameTime);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnNUll">If true this will return null if no tile found. If false the tile wont be added to the list</param>
        /// <returns></returns>
        public List<Tile> GetAdjacentTiles(bool returnNull)
        {
            List<Tile> returnTiles = new List<Tile>();
            //Adjacent Tiles are defined as the tiles above, below, to the right and to the left
            //Of this tile. Diagonals are not counted with this code
            //You might need more code that does take account diagonal tiles
            //Who knows...

            Tile topTile = null;
            Tile bottomTile = null;
            Tile leftTile = null;
            Tile rightTile = null;

            //Get Top Tile
            try
            {
                if(PathfindingController.Tiles[(int)_location.X, (int)_location.Y - 1] != null)
                    topTile = PathfindingController.Tiles[(int)_location.X, (int)_location.Y - 1];
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //Get Bottom
            try
            {
                if (PathfindingController.Tiles[(int)_location.X, (int)_location.Y + 1] != null)
                    bottomTile = PathfindingController.Tiles[(int)_location.X, (int)_location.Y + 1];
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //Get Left
            try
            {
                if (PathfindingController.Tiles[(int)_location.X - 1, (int)_location.Y] != null)
                        leftTile = PathfindingController.Tiles[(int)_location.X - 1, (int)_location.Y];
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //Get Right
            try
            {
                if (PathfindingController.Tiles[(int)_location.X + 1, (int)_location.Y] != null)
                {
                    rightTile = PathfindingController.Tiles[(int)_location.X + 1, (int)_location.Y];
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //This region adds the tiles to the tile list
            //This is where return null is used
            #region ReturnNull
            if (returnNull)
            {
                returnTiles.Add(topTile);
                returnTiles.Add(bottomTile);
                returnTiles.Add(leftTile);
                returnTiles.Add(rightTile);
            }
            else
            {
                if(topTile != null)
                    returnTiles.Add(topTile);
                if (bottomTile != null)
                    returnTiles.Add(bottomTile);
                if (leftTile != null)
                    returnTiles.Add(leftTile);
                if (rightTile != null)
                    returnTiles.Add(rightTile);
            }
            #endregion

            return returnTiles;
        }

        public void SetHighLight(bool value)
        {
            _highlighted = value;
        }

        /// <summary>
        /// Returns the tile on the grid given the location in pixels on the screen
        /// </summary>
        /// <param name="location">Location in pixels on the screen</param>
        /// <returns></returns>
        public static Tile GetTileFromPixelLocation(Vector2 location)
        {
            return PathfindingController.Tiles[(int)(location.X/tileWidth), (int)(location.Y/tileHeight)];
        }

        public static Tile GetTileFromGridLocation(Vector2 gridLocation)
        {
            try
            {
                return PathfindingController.Tiles[(int)gridLocation.X, (int)gridLocation.Y];
            }
            catch
            {
                return null;
            }
        }

        //Pathfinding stuff
        #region Pathfinding
        public void Reset()
        {
            cost = 0;
            total = 0;
            heuristic = 0;
            parent = null;
            diagonal = false;
        }
        #endregion
    }

}
