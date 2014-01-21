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
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;
using Moxies5.Controllers;

namespace Moxies5.Components.ItemComponents
{
    public class ItemPlacementComponent: DrawableComponent
    {
        
        #region Fields
        private int _tilesWide = -1;
        private int _tilesHigh = -1;
        private bool _placementAllowed = true;
        private SensorComponent _sensorComponent;
        private Entity _pictureEntity; //This is the picture of the item you will be placing
        private Item _itemToPlace;

        #endregion

        #region Properties

        #endregion

        #region Getters and Setters
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ItemPlacementComponent(Entity parentEntity, Item toPlace)
            : base(parentEntity , "barpixel", Cameras.Dynamic)
        {
            _itemToPlace = toPlace;
            int tilesWide = -1;
            int tilesHigh = -1;

            Vector2 mousePos = MainController.Camera.get_mouse_pos(MainController.GraphicsDevice);

            Entity entityToPlace = EntityFactory.CreateItem(toPlace, mousePos);
            if (entityToPlace.HasComponent(typeof(TileBlockComponent)))
            {
                TileBlockComponent tbc = (TileBlockComponent)entityToPlace.GetComponent(typeof(TileBlockComponent));
                tilesWide = tbc.TilesWide;
                tilesHigh = tbc.TilesHigh;
                tbc.SetBlocked(false);
            }
            else
            {
                throw new Exception("Parent must have a tile block component...maybe");
            }

            _pictureEntity = new Entity();
            if (entityToPlace.HasComponent(typeof(DrawableComponent)))
            {
                DrawableComponent _pictureDC = (DrawableComponent)entityToPlace.GetComponent(typeof(DrawableComponent)).Copy(_pictureEntity);
                _pictureEntity.AddInitialComponent(_pictureDC);
                _pictureDC.SetLayerDepth(Layers.Grid_TempPicture);
            }
            else
            {
                throw new Exception("Item must have a drawable component");
            }
            //Lock to grid
            int xTile = (int)(mousePos.X / Tile.tileWidth);
            int yTile = (int)(mousePos.Y / Tile.tileHeight);
            Vector2 lockPosition = new Vector2(xTile * Tile.tileWidth, yTile*Tile.tileHeight);
            Vector2 gridLocation = new Vector2((int)(lockPosition.X / Tile.tileWidth), (int)(lockPosition.Y / Tile.tileHeight));
            Vector2 offset = new Vector2((_tilesWide * Tile.tileWidth) / 2, (_tilesHigh * Tile.tileHeight) / 2);
            lockPosition += offset;

            if (entityToPlace.HasComponent(typeof(SpatialComponent)))
            {
                SpatialComponent _pictureSC = (SpatialComponent)entityToPlace.GetComponent(typeof(SpatialComponent)).Copy(_pictureEntity);
                SpatialComponent _entityToPlaceSC = (SpatialComponent)entityToPlace.GetComponent(typeof(SpatialComponent));
                DrawableComponent _entityToPlaceDC = (DrawableComponent)entityToPlace.GetComponent(typeof(DrawableComponent));
                DrawableComponent _pictureDC = (DrawableComponent)entityToPlace.GetComponent(typeof(DrawableComponent)).Copy(_pictureEntity);
                
                _entityToPlaceSC.SetPosition(lockPosition);
                _pictureSC.SetPosition(lockPosition); //Set the position of the picture Entity

                _pictureDC.Update(null);
                _entityToPlaceDC.Update(null);
                _pictureEntity.AddInitialComponent(_pictureSC);
            }
            else
            {
                throw new Exception("Item must have a spatial component");
            }

  
            EntityManager.AddEntity(_pictureEntity);
            entityToPlace.Remove();


            this._tilesWide = tilesWide;
            this._tilesHigh = tilesHigh;
            UpdateOrder = 1;
            Name = "ItemPlacementComponent";
            SetSourceRectangle(new Rectangle(0, 0, Tile.tileWidth * tilesWide, Tile.tileHeight * tilesHigh));
            SetOrigin(new Vector2((Tile.tileWidth * tilesWide) / 2, (Tile.tileHeight * tilesHigh) / 2));
            SetTransparency(0.5f);
            SetColor(Color.Green);
            SetLayerDepth(Layers.Grid_Blocked_Indicator);
            if (Parent.HasComponent(typeof(SpatialComponent)))
            {
                SpatialComponent _sc = (SpatialComponent)parentEntity.GetComponent(typeof(SpatialComponent));
                _sc.SetPosition(lockPosition);
            }
            else
            {
                throw new Exception("Parent needs a spatial component");
            }
           
            if (Parent.HasComponent(typeof(SensorComponent)))
            {
                SensorComponent sensor = (SensorComponent)Parent.GetComponent(typeof(SensorComponent));
                _sensorComponent = sensor;
                sensor.Body.OnCollision += new FarseerPhysics.Dynamics.OnCollisionEventHandler(Body_OnCollision);
                sensor.Body.IgnoreCollisionWith(MainController.MouseEntity.SensorBody);
            }
            else
            {
                throw new Exception("Must have a sensor component or the sensor component must be added first");
            }
        }

        bool Body_OnCollision(FarseerPhysics.Dynamics.Fixture fixtureA, FarseerPhysics.Dynamics.Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            //If there is a collision with any physical Entity. Do not allow placement. Make sure you ignore the mouse pointer
            if (fixtureB.Body != fixtureA.Body)
            {
                if (fixtureB.Body.UserData != null)
                {
                   //Disallow collisions with items. 
                    Entity blockingEntity = (Entity)fixtureB.Body.UserData;
                    if(blockingEntity.HasComponent(typeof(TileBlockComponent)))
                    {
                        //Dont allow collisions with tileblockcomponents we will test later if the tile is blocked
                        _placementAllowed = true;
                        SetColor(Color.Green);
                        return false;
                    }
                    _placementAllowed = false;
                    SetColor(Color.Red);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            //Update the picture
            SpatialComponent _pictureSC = (SpatialComponent)_pictureEntity.GetComponent(typeof(SpatialComponent));
            MainController.SavingEnabled = false;

            //Lock to grid
            Vector2 mousePos = MainController.Camera.get_mouse_pos(MainController.GraphicsDevice);
            int xTile = (int)(mousePos.X / Tile.tileWidth);
            int yTile = (int)(mousePos.Y / Tile.tileHeight);
            Vector2 lockPosition = new Vector2(xTile * Tile.tileWidth, yTile*Tile.tileHeight);
            Vector2 gridLocation = new Vector2((int)(lockPosition.X / Tile.tileWidth), (int)(lockPosition.Y / Tile.tileHeight));
            Vector2 offset = new Vector2((_tilesWide * Tile.tileWidth)/2, (_tilesHigh * Tile.tileHeight)/2);
            lockPosition += offset;
            if (Parent.HasComponent(typeof(SpatialComponent)))
            {
                SpatialComponent sc = (SpatialComponent)Parent.GetComponent(typeof(SpatialComponent));
                sc.SetPosition(lockPosition);
            }
            else
            {
                throw new Exception("Parent must have a spatial component");
            }

            _pictureSC.SetPosition(lockPosition); //Set the position of the picture Entity
            DrawableComponent _pictureDC = (DrawableComponent)_pictureEntity.GetComponent(typeof(DrawableComponent));

            //Sense if there was a collision with another entity
            //If there was turn it red and disallow placement
            //If there wasn't turn green and allow placement
            if (_sensorComponent.Body.ContactList == null)
            {
                //Check to make sure the tiles are empty before allowing placement

                _placementAllowed = true;
                SetColor(Color.Green);
            }

            for (int x = 0; x < _tilesWide; x++)
            {
                for (int y = 0; y < _tilesHigh; y++)
                {
                    if (gridLocation.X + x >= 0 && gridLocation.X + x < PathfindingController.TilesWide && gridLocation.Y + y >= 0 && gridLocation.Y + y < PathfindingController.TilesHigh)
                    {
                        Tile tile = PathfindingController.Tiles[(int)gridLocation.X + x, (int)gridLocation.Y + y];
                        if (tile.Blocked)
                        {
                            _placementAllowed = false;
                            SetColor(Color.Red);
                        }
                    }
                }
            }

            if (_placementAllowed)
            {
                if (InputHandler.LeftMouseClick())
                {
                    //Place the item
                    Entity entityToPlace = EntityFactory.CreateItem(_itemToPlace, lockPosition);
                    TileBlockComponent tbc = (TileBlockComponent)entityToPlace.GetComponent(typeof(TileBlockComponent));
                    tbc.SetBlocked(true); //Block the tile
                    tbc.SetCanBeUsedAsTarget(); 
                    EntityManager.AddEntity(entityToPlace);
                    MainController.SavingEnabled = true;
                    Parent.RemoveComponent(Name);
                }
            }


            base.Update(gameTime);
        }

    }
}
