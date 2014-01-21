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


namespace Moxies5.Controllers
{
    /// <summary>
    /// This is the master controller for all of the systems
    /// The Main Game class calls the methods of this class to run 
    /// all of the game systems
    /// </summary>
    public static class MainController
    {
        #region Fields

        private static Main _game = null;
        private static List<Component> _allComponents = new List<Component>();
        private static InputHandler inputHandler;
        private static MouseEntity mouseSensorEntity = new MouseEntity();
        public static Random Random = new Random();

        //Graphics Stuff
        #region Graphics
        private static SpriteBatch _staticSpriteBatch;
        private static SpriteBatch _cameraSpriteBatch;

        private static GraphicsDeviceManager _graphics;
        private static GraphicsDevice _graphicsDevice;
        private static ContentManager _content;


        private static Camera2D _camera;
        #endregion


        #endregion

        #region Properties
        public static ContentManager Content
        {
            get
            {
                return _content;
            }
        }

        public static Main Game
        {
            get
            {
                return _game;
            }
        }

        public static GraphicsDevice GraphicsDevice
        {
            get
            {
                return _graphicsDevice;
            }
        }

        public static SpriteBatch CameraSpriteBatch
        {
            get
            {
                return _cameraSpriteBatch;
            }
        }

        public static SpriteBatch StaticSpriteBatch
        {
            get
            {
                return _staticSpriteBatch;
            }
        }

        public static Camera2D Camera
        {
            get
            {
                return _camera;
            }
        }

        public static MouseEntity MouseEntity
        {
            get
            {
                return mouseSensorEntity;
            }
        }

        public static bool SavingEnabled { get; set; }

        public const int DefaultScreenWidth = 1280;
        public const int DefaultScreenHeight = 800;

        /// <summary>
        /// If this is true the player is allowed to input things
        /// </summary>
        public static bool ControlsEnabled { get; set; }


        #endregion

        #region Methods

        /// <summary>
        /// Sets up the system controller
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="game"></param>
        public static void Setup(ContentManager contentManager, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, Main game, int tilesWide, int tilesHigh)
        {
            _game = game;

            #region Input
            inputHandler = new InputHandler(game);
            ControlsEnabled = true;
            EntityManager.AddEntity(mouseSensorEntity);
            #endregion

            #region Graphics
            _graphicsDevice = graphicsDevice;
            _graphics = graphics;
            _graphics.PreferredBackBufferHeight = DefaultScreenHeight;
            _graphics.PreferredBackBufferWidth = DefaultScreenWidth;
            _graphics.ApplyChanges();
            _content = contentManager;
            
            _staticSpriteBatch = new SpriteBatch(graphicsDevice);
            _cameraSpriteBatch = new SpriteBatch(graphicsDevice);
            _camera = new Camera2D();
            #endregion

            #region Serialization
            Serialization.Serializer.Initalize();
            #endregion

            #region Debugging
            //Debugger.LoadContent(game);
            #endregion

            #region Physics
            PhysicsController.Setup();
            #endregion

            #region UI
            UIController.Setup();
            #endregion

            #region TileGrid
            PathfindingController.Setup(tilesWide, tilesHigh);
            Camera._pos = new Vector2((tilesWide * Tile.tileWidth) / 2, (tilesHigh * Tile.tileHeight) / 2);
            #endregion

            #region Tank Walls
            int wallThickness = 100;

            Body topWall = BodyFactory.CreateRectangle(PhysicsController.World, ConvertUnits.ToSimUnits((PathfindingController.TilesWide * Tile.tileWidth) + (wallThickness * 2)), ConvertUnits.ToSimUnits(wallThickness), 1);
            topWall.BodyType = BodyType.Static;
            topWall.Position = new Vector2(ConvertUnits.ToSimUnits((PathfindingController.TilesWide * Tile.tileWidth) / 2), ConvertUnits.ToSimUnits(-wallThickness/2));

            Body bottomWall = BodyFactory.CreateRectangle(PhysicsController.World, ConvertUnits.ToSimUnits((PathfindingController.TilesWide * Tile.tileWidth) + (wallThickness * 2)), ConvertUnits.ToSimUnits(wallThickness), 1);
            bottomWall.BodyType = BodyType.Static;
            bottomWall.Position = new Vector2(ConvertUnits.ToSimUnits((PathfindingController.TilesWide * Tile.tileWidth) / 2), ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight) + wallThickness / 2));

            Body leftWall = BodyFactory.CreateRectangle(PhysicsController.World, ConvertUnits.ToSimUnits(wallThickness), ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight)), 1);
            leftWall.BodyType = BodyType.Static;
            leftWall.Position = new Vector2(ConvertUnits.ToSimUnits(-wallThickness/2), ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight) /2));

            Body rightWall = BodyFactory.CreateRectangle(PhysicsController.World, ConvertUnits.ToSimUnits(wallThickness), ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight)), 1);
            rightWall.BodyType = BodyType.Static;
            rightWall.Position = new Vector2(ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight) + (wallThickness/2)), ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight) / 2));
            #endregion

            #region Add First Two Moxies

            MoxieEntity boyMoxie = new MoxieEntity();
            EntityManager.AddEntity(boyMoxie);
            boyMoxie.Initialize();
            boyMoxie.GenerateForFirstMoxies(Components.MoxieComponents.GenderTrait.Male);

            MoxieEntity girlMoxie = new MoxieEntity();
            girlMoxie.Initialize();
            EntityManager.AddEntity(girlMoxie);
            girlMoxie.GenerateForFirstMoxies(Components.MoxieComponents.GenderTrait.Female);
            #endregion

        }

        /// <summary>
        /// Call this to resetup the game after its been loaded once
        /// Useful when loading the game
        /// </summary>
        public static void Reset()
        {
            EntityManager.EntityMasterList.Clear();
            PhysicsController.Reset();
            #region Tank Walls
            int wallThickness = 100;

            Body topWall = BodyFactory.CreateRectangle(PhysicsController.World, ConvertUnits.ToSimUnits((PathfindingController.TilesWide * Tile.tileWidth) + (wallThickness * 2)), ConvertUnits.ToSimUnits(wallThickness), 1);
            topWall.BodyType = BodyType.Static;
            topWall.Position = new Vector2(ConvertUnits.ToSimUnits((PathfindingController.TilesWide * Tile.tileWidth) / 2), ConvertUnits.ToSimUnits(-wallThickness / 2));

            Body bottomWall = BodyFactory.CreateRectangle(PhysicsController.World, ConvertUnits.ToSimUnits((PathfindingController.TilesWide * Tile.tileWidth) + (wallThickness * 2)), ConvertUnits.ToSimUnits(wallThickness), 1);
            bottomWall.BodyType = BodyType.Static;
            bottomWall.Position = new Vector2(ConvertUnits.ToSimUnits((PathfindingController.TilesWide * Tile.tileWidth) / 2), ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight) + wallThickness / 2));

            Body leftWall = BodyFactory.CreateRectangle(PhysicsController.World, ConvertUnits.ToSimUnits(wallThickness), ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight)), 1);
            leftWall.BodyType = BodyType.Static;
            leftWall.Position = new Vector2(ConvertUnits.ToSimUnits(-wallThickness / 2), ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight) / 2));

            Body rightWall = BodyFactory.CreateRectangle(PhysicsController.World, ConvertUnits.ToSimUnits(wallThickness), ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight)), 1);
            rightWall.BodyType = BodyType.Static;
            rightWall.Position = new Vector2(ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight) + (wallThickness / 2)), ConvertUnits.ToSimUnits((PathfindingController.TilesHigh * Tile.tileHeight) / 2));
            #endregion
        }

        /// <summary>
        /// Updates all of the updateable components
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
            //1)Update the physics
            #region Physics
            PhysicsController.Update(gameTime);
            #endregion


            //2)
            //Update the input handler
            #region Input
            inputHandler.Update(gameTime);
#endregion

            //3)
            #region Camera Zoom and Movement
            if (InputHandler.KeyPressed(Keys.Add))
            {
                Camera.Zoom += 0.5f;
            }
            if (InputHandler.KeyPressed(Keys.Subtract))
            {
                Camera.Zoom -= 0.5f;
            }
            Vector2 cameraMove = Vector2.Zero;
            if (InputHandler.KeyDown(Keys.Left))
            {
                cameraMove.X = -1;
            }
            if (InputHandler.KeyDown(Keys.Right))
            {
                cameraMove.X = 1;
            }
            if (InputHandler.KeyDown(Keys.Up))
            {
                cameraMove.Y = -1;
            }
            if (InputHandler.KeyDown(Keys.Down))
            {
                cameraMove.Y = 1;
            }
            Camera.Move(cameraMove * 3);
            #endregion

            //4)
            #region Saving
            SavingEnabled = true;
            #endregion

            //5)Update all Entities
            #region Update Entity Master List
            EntityManager.Update(gameTime);
            #endregion

            //6)Update all components
            #region Components
            //Clear the components list
            _allComponents.Clear();

            //Get components from the entities
            for (int x = 0; x < EntityManager.EntityCount; x++)
            {
                //Update the entity
                EntityManager.EntityMasterList[x].Update(gameTime);
                //Add all the components
                _allComponents.AddRange(EntityManager.EntityMasterList[x].Components);
            }

            //Sort the components
            SortUpdateComponents();

            //Update all the components
            for (int i = 0; i < _allComponents.Count; i++)
            {
                _allComponents[i].Update(gameTime);

                if (_allComponents[i].Parent == null)
                {
                    if (_allComponents[i].GetType() == typeof(PhysicsComponent))
                    {
                        PhysicsComponent pc = (PhysicsComponent)_allComponents[i];
                        PhysicsController.World.RemoveBody(pc.Body);
                    }
                }
            }
            #endregion

            #region Serialization Input
            Serialization.Serializer.Update(gameTime);
            if (InputHandler.KeyPressed(Keys.F2))
            {
                if(SavingEnabled)
                    Serialization.Serializer.Save();
            }
            if (InputHandler.KeyPressed(Keys.F3))
            {
                Serialization.Serializer.Load();
            }
            #endregion



            //7) Update the tile grid
            #region Tile Grid
            PathfindingController.Update(gameTime);
            #endregion

            //8 Update the UI
            #region UI
            UIController.Update(gameTime);
            #endregion

            ///Test region
            if (InputHandler.KeyPressed(Keys.F))
            {
                MoxieEntity newMoxie = new MoxieEntity();
                newMoxie.Initialize();
                int GenderRandom = Random.Next(0, 2);
                if(GenderRandom == 0)
                    newMoxie.GenerateForFirstMoxies(Components.MoxieComponents.GenderTrait.Male);
                else
                    newMoxie.GenerateForFirstMoxies(Components.MoxieComponents.GenderTrait.Female);
                SpatialComponent sc = (SpatialComponent)newMoxie.GetComponent(typeof(SpatialComponent));
                sc.SetPosition(MainController.Camera.get_mouse_pos(MainController.GraphicsDevice));
                EntityManager.AddEntity(newMoxie);
            }

        }

        /// <summary>
        /// Draws all of the components
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Draw(GameTime gameTime)
        {
            #region Setup Lists
            List<IDrawableComponent> staticList = new List<IDrawableComponent>();
            List<IDrawableComponent> cameraList = new List<IDrawableComponent>();
            #endregion

            //Add Components to the different lists
            #region Add Drawable Components to the lists
            for (int x = 0; x < EntityManager.EntityCount; x++)
            {
                Entity entity = EntityManager.EntityMasterList[x];
                for (int i = 0; i < entity.Components.Count; i++)
                {
                    if (entity.Components[i] is IDrawableComponent)
                    {
                        IDrawableComponent drawableComponent = (IDrawableComponent)entity.Components[i];
                        if (drawableComponent.Camera == Cameras.Dynamic)
                        {
                            cameraList.Add(drawableComponent);
                        }
                        else if (drawableComponent.Camera == Cameras.Static)
                        {
                            staticList.Add(drawableComponent);
                        }
                        else
                        {
                            throw new Exception("Not using a valid camera");
                        }
                    }
                }

            }
            #endregion

            //Sort the lists
            #region Sort Components
            SortDrawComponents(staticList);
            SortDrawComponents(cameraList);
            #endregion

            //Draw the lists
            #region Draw Camera Sprites

            //This will need to change eventually...maybe

            _cameraSpriteBatch.Begin(SpriteSortMode.FrontToBack,
            BlendState.AlphaBlend,
            null,
            null,
            null,
            null,
            Camera.get_transformation(GraphicsDevice /*Send the variable that has your graphic device here*/));
            
            for (int i = 0; i < cameraList.Count; i++)
            {
                cameraList[i].Draw(gameTime);
            }
            #region Tile Grid
            PathfindingController.Draw(gameTime);
            #endregion
            _cameraSpriteBatch.End();
            #endregion

            #region Draw Static Sprites


            _staticSpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            #region UI
            //Draw the UI
            UIController.Draw(gameTime);
            #endregion
            for (int i = 0; i < staticList.Count; i++)
            {
                staticList[i].Draw(gameTime);
            }
            Debugger.DrawStatic();

 

            _staticSpriteBatch.End();


            #endregion

            #region Physics Debugger Drawing
            PhysicsController.Draw(gameTime);
            #endregion
        }

        /// <summary>
        /// This will sort all the components based on their update order
        /// </summary>
        static void SortUpdateComponents()
        {
            _allComponents.Sort(
                 delegate(Component p1, Component p2)
                 {
                     return p1.UpdateOrder.CompareTo(p2.UpdateOrder);
                 }
             );
        }

        /// <summary>
        /// This will sort all the components based on their update order
        /// </summary>
        static void SortDrawComponents(List<IDrawableComponent> list)
        {
            list.Sort(
                 delegate(IDrawableComponent p1, IDrawableComponent p2)
                 {
                     return p1.DrawOrder.CompareTo(p2.DrawOrder);
                 }
             );
        }

    

        #endregion

    }
}
