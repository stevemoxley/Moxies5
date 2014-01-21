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
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;
using Moxies5.Components.ItemComponents;

namespace Moxies5
{
    public static class EntityFactory
    {
        /// <summary>
        /// Call this to create a new entity of the specified item
        /// </summary>
        /// <param name="item">The item to create</param>
        public static Entity CreateItem(Item item, Vector2 position)
        {
            switch (item)
            {
                #region FOOD_ORANGE
                case Item.FOOD_ORANGE:
                    {
                        Entity orange = new Entity();
                        SpatialComponent sc = new SpatialComponent(orange);
                        DrawableComponent dc = new DrawableComponent(orange, "orange", Cameras.Dynamic);
                        dc.SetScale(0.05f);
                        PhysicsComponent pc = new PhysicsComponent(orange);
                        pc.CreateCircleBody(10, 1);
                        FoodComponent fc = new FoodComponent(orange, 10, false);
                        SensorComponent sensorComponent = new SensorComponent(orange);
                        sensorComponent.CreateSensorCircleBody(13, 1);
                        orange.AddInitialComponent(sensorComponent);
                        orange.AddInitialComponent(fc);
                        orange.AddInitialComponent(sc);
                        orange.AddInitialComponent(dc);
                        orange.AddInitialComponent(pc);
                        sc.SetPosition(position);
                        sc.SetPhysicsBodyLocation();
                        return orange;
                    }
                #endregion

                #region FOOD_DISPENSER_BASIC
                case Item.FOOD_DISPENSER_BASIC:
                    {
                        Entity dispenser = new Entity();

                        SpatialComponent sc = new SpatialComponent(dispenser);
                        dispenser.AddInitialComponent(sc);

                        DrawableComponent dc = new DrawableComponent(dispenser, "Items/foodItem", Cameras.Dynamic);
                        dc.SetSourceRectangle(new Rectangle(0, 0, Tile.tileWidth, Tile.tileHeight));
                        dc.SetOrigin(new Vector2(Tile.tileWidth / 2, Tile.tileHeight / 2));
                        dispenser.AddInitialComponent(dc);

                        PhysicsComponent pc = new PhysicsComponent(dispenser);
                        pc.CreateRectangleBody(ConvertUnits.ToSimUnits(Tile.tileWidth), ConvertUnits.ToSimUnits(Tile.tileHeight), 1);
                        pc.Body.BodyType = BodyType.Static;
                        dispenser.AddInitialComponent(pc);
                        sc.SetPhysicsBodyLocation();
                        sc.SetPosition(position);

                        TileBlockComponent tbc = new TileBlockComponent(dispenser, position, 1, 1, true);
                        dispenser.AddInitialComponent(tbc);

                        FoodComponent foodComponent = new FoodComponent(dispenser, 20, true);
                        dispenser.AddInitialComponent(foodComponent);

                        SensorComponent sensor = new SensorComponent(dispenser);
                        sensor.CreateSensorRectangleBody(ConvertUnits.ToSimUnits(50), ConvertUnits.ToSimUnits(50), 1);
                        dispenser.AddInitialComponent(sensor);

                        return dispenser;
                    }
                #endregion

                #region MONEY_MAKING_BASIC
                case Item.MONEY_MAKING_BASIC:
                    {
                        Entity moneyMaker = new Entity();

                        SpatialComponent sc = new SpatialComponent(moneyMaker);
                        moneyMaker.AddInitialComponent(sc);

                        DrawableComponent dc = new DrawableComponent(moneyMaker, "Items/moneyItem", Cameras.Dynamic);
                        dc.SetSourceRectangle(new Rectangle(0, 0, Tile.tileWidth, Tile.tileHeight));
                        dc.SetOrigin(new Vector2(Tile.tileWidth / 2, Tile.tileHeight / 2));
                        moneyMaker.AddInitialComponent(dc);

                        PhysicsComponent pc = new PhysicsComponent(moneyMaker);
                        pc.CreateRectangleBody(ConvertUnits.ToSimUnits(Tile.tileWidth), ConvertUnits.ToSimUnits(Tile.tileHeight), 1);
                        pc.Body.BodyType = BodyType.Static;
                        moneyMaker.AddInitialComponent(pc);
                        sc.SetPhysicsBodyLocation();
                        sc.SetPosition(position);

                        TileBlockComponent tbc = new TileBlockComponent(moneyMaker, position, 1, 1, true);
                        moneyMaker.AddInitialComponent(tbc);

                        MoneyMakingComponent mmc = new MoneyMakingComponent(moneyMaker, 10);
                        moneyMaker.AddInitialComponent(mmc);


                        SensorComponent sensor = new SensorComponent(moneyMaker);
                        sensor.CreateSensorRectangleBody(ConvertUnits.ToSimUnits(50), ConvertUnits.ToSimUnits(50), 1);
                        moneyMaker.AddInitialComponent(sensor);
                        return moneyMaker;
                    }
                #endregion

                #region Default Case
                default:
                    {
                        throw new Exception("No creation method found for this item");
                    }
                #endregion
            }
        }
    }


}
