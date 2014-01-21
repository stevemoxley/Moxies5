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
using Moxies5.Entities;
using Moxies5.Utilities;
using Moxies5.Serialization;
using Moxies5.Controllers;

namespace Moxies5.Components.ItemComponents
{
    public class TileBlockComponent: Component, ISerialize
    {
        
        #region Fields
        private Vector2 _gridLocation;
        private int _tilesWide;
        private int _tilesHigh;
        #endregion

        #region Properties
        public Vector2 GridLocation
        {
            get
            {
                return _gridLocation;
            }
        }

        public int TilesWide
        {
            get
            {
                return _tilesWide;
            }
        }
        public int TilesHigh
        {
            get
            {
                return _tilesHigh;
            }
        }

        /// <summary>
        /// This will allow the tile to be used as a target, but will still block tile
        /// </summary>
        public bool CanBeUsedAsTarget
        {
            get;
            set;
        }
        #endregion

        #region Getters and Setters
        #endregion

        public TileBlockComponent(Entity parentEntity, SpatialComponent sc, int tilesWide, int tilesHigh, bool canBeUsedAsTarget)
            : this(parentEntity, sc.Position, tilesWide, tilesHigh, canBeUsedAsTarget)
        {
            
        }

        /// <summary>
        /// This component will block the path of moxies.
        /// </summary>
        /// <param name="parentEntity"></param>
        /// <param name="location">This is the tile location on the grid of the upper left hand tile </param>
        /// <param name="tilesWide"></param>
        /// <param name="tilesHigh"></param>
        public TileBlockComponent(Entity parentEntity, Vector2 position, int tilesWide, int tilesHigh, bool canBeUsedAsTarget)
            : base(parentEntity)
        {

            this._gridLocation = ConvertPixelsToTileLocation(position);
            this._tilesWide = tilesWide;
            this._tilesHigh = tilesHigh;
            this.CanBeUsedAsTarget = canBeUsedAsTarget;

            UpdateOrder = 1;
            Name = "TileBlockComponent";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public static Vector2 ConvertPixelsToTileLocation(Vector2 pixelLocation)
        {
            Vector2 gridLocation = new Vector2((int)(pixelLocation.X / Tile.tileWidth), (int)(pixelLocation.Y / Tile.tileHeight));
            return gridLocation;
        }

        /// <summary>
        /// Sets whether or not the tiles that this component blocks are actually blocked
        /// </summary>
        /// <param name="value"></param>
        public void SetBlocked(bool value)
        {
            for (int x = 0; x < _tilesWide; x++)
            {
                for (int y = 0; y < _tilesHigh; y++)
                {
                    if (_gridLocation.X + x >= 0 && _gridLocation.X + x <= PathfindingController.TilesWide && _gridLocation.Y + y >= 0 && _gridLocation.Y + y <= PathfindingController.TilesHigh)
                    {
                        PathfindingController.Tiles[(int)_gridLocation.X + x, (int)_gridLocation.Y + y].Blocked = value;
                    }
                }
            }
        }

        public void SetCanBeUsedAsTarget()
        {
            for (int x = 0; x < _tilesWide; x++)
            {
                for (int y = 0; y < _tilesHigh; y++)
                {
                    if (_gridLocation.X + x >= 0 && _gridLocation.X + x <= PathfindingController.TilesWide && _gridLocation.Y + y >= 0 && _gridLocation.Y + y <= PathfindingController.TilesHigh)
                    {
                        PathfindingController.Tiles[(int)_gridLocation.X + x, (int)_gridLocation.Y + y].CanBeUsedAsTarget = CanBeUsedAsTarget;
                    }
                }
            }
        }

        public SaveObject Serialize(int ID)
        {
            TileBlockComponentSave save = new TileBlockComponentSave();
            save.Serialize(this, ID);
            return save;
        }

    }

    public class TileBlockComponentSave : SaveObject
    {
        TileBlockComponent _tileBlockComponent = null;
        public int GridLocationX;
        public int GridLocationY;
        public int TilesWide;
        public int TilesHigh;
        public bool CanBeUsedAsTarget;

        public override void Serialize(object _tileBlockComponent, int ID)
        {
            TileBlockComponent tileBlockComponent = (TileBlockComponent)_tileBlockComponent;
            this.GridLocationX = (int)tileBlockComponent.GridLocation.X;
            this.GridLocationY = (int)tileBlockComponent.GridLocation.Y;
            this.TilesWide = tileBlockComponent.TilesWide;
            this.TilesHigh = tileBlockComponent.TilesHigh;
            this.CanBeUsedAsTarget = tileBlockComponent.CanBeUsedAsTarget;
            base.Serialize(_tileBlockComponent, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            TileBlockComponentSave fSave = (TileBlockComponentSave)save;
            _tileBlockComponent = new TileBlockComponent(null, new Vector2(fSave.GridLocationX, fSave.GridLocationY), fSave.TilesWide, fSave.TilesHigh, fSave.CanBeUsedAsTarget);
            return _tileBlockComponent;
        }

    }


}
