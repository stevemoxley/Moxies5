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
using Moxies5.Components.ItemComponents;
using Moxies5.Entities;
using Moxies5.Utilities;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.DebugViews;
using Moxies5.Components.MoxieComponents;


namespace Moxies5.Controllers
{
    public enum UIStates
    {
        None,
        MoxieInfo,
        Items,
    }

    public static class UIController
    {
        #region Fields

        private static Entity _UISaveEntity = new Entity();
        private static UIStates _UIState = UIStates.MoxieInfo;
        private static Texture2D _UITexture;
        private static Timer _resetTwoSelectedTimer = new Timer(1);

        public static bool ThoughtBubblesVisible = false;


        #region Item Screen Fields
        private static List<Entity> _itemScreenButtonEntities = new List<Entity>();

        private static int _itemScreenPage = 1;
        #endregion

        #region Top Bar Entities
        private static Entity _moneyTextEntity;
        private static Entity _moxieCountTextEntity;
        #endregion

        #region Moxie Info Entities
        private static MoxieEntity _targetMoxie;
        private static Entity healthText;
        private static Entity hungerText;
        private static Entity sleepText;
        private static Entity funText;
        private static Entity ageText;
        #endregion

        #region Item Screen Entities

        #endregion

        #region Items
        /// <summary>
        /// This is a list of the items that have been unlocked for use
        /// </summary>
        private static List<Item> _unlockedItems = new List<Item>();
        #endregion

        #endregion

        #region Properties

        #region Moxie Info
        public static MoxieEntity TargetMoxie
        {
            get
            {
                return _targetMoxie;
            }
        }
        public static GameSaveComponent UISaveComponent
        {
            get
            {
                return (GameSaveComponent)_UISaveEntity.GetComponent(typeof(GameSaveComponent));
            }
        }
        #endregion

        public static UIStates UIState
        {
            get
            {
                return _UIState;
            }
        }

        #region Item Screen
        /// <summary>
        /// Gets the current page of the item screen
        /// </summary>
        public static int ItemScreenPage
        {
            get
            {
                return _itemScreenPage; 
            }
        }
        #endregion

        #endregion

        #region Getters and Setters

        public static void SetGameSaveComponent(GameSaveComponent component)
        {
            if(_UISaveEntity.HasComponent(typeof(GameSaveComponent)))
            {
                _UISaveEntity.RemoveComponent(typeof(GameSaveComponent));
                _UISaveEntity.RunThoughWaitLists();
                _UISaveEntity.AddInitialComponent(component);
            }
        }

        #region Moxie Info
        public static void SetTargetMoxie(MoxieEntity target)
        {
            if (_targetMoxie != null)
            {
                _targetMoxie.RemoveComponent(typeof(MoxieOutlineComponent));
                _targetMoxie.RunThoughWaitLists();
            }

            _targetMoxie = target;
            //You will have to remove the component that shows the outline around the moxie here
            if (_targetMoxie != null)
            {
                if (!_targetMoxie.HasComponent(typeof(MoxieOutlineComponent)))
                {
                    MoxieOutlineComponent moc = new MoxieOutlineComponent(target, Cameras.Dynamic);
                    switch(_targetMoxie.BodySize)
                    {
                        case BodySize.Normal:
                            {
                                break;
                            }
                        case BodySize.Small:
                            {
                                moc.SetScale(0.75f);
                                break;
                            }
                        case BodySize.Baby:
                            {
                                moc.SetScale(0.05f);
                                break;
                            }
                    }
                    
                    _targetMoxie.AddComponent(moc);
                }
            }

        }
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Sets up the system controller
        /// </summary>
        public static void Setup()
        {
            #region GameSave Entity
                EntityManager.AddEntity(_UISaveEntity);
                GameSaveComponent UIS = new GameSaveComponent(_UISaveEntity);
                _UISaveEntity.AddInitialComponent(UIS);
                #endregion

            #region UI Setup
            _UITexture = MainController.Game.Content.Load<Texture2D>("UI/moxiesUI");

            #region Top Bar Entities

            #region Money Text
            _moneyTextEntity = new Entity();
            EntityManager.AddEntity(_moneyTextEntity);
            TextComponent _moneyTextComponent = new TextComponent(_moneyTextEntity, "$" + UISaveComponent.Money, Cameras.Static);
            _moneyTextComponent.Alignment = Alignment.Right;
            _moneyTextEntity.AddInitialComponent(_moneyTextComponent);

            SpatialComponent _moneyTextSC = new SpatialComponent(_moneyTextEntity);
            _moneyTextSC.SetPosition(new Vector2(MainController.DefaultScreenWidth, 0));
            _moneyTextEntity.AddInitialComponent(_moneyTextSC);
            _moneyTextComponent.SetLayerDepth(Layers.UI_Foreground);
            #endregion

            #region Moxie Count Text
            _moxieCountTextEntity = new Entity();
            EntityManager.AddEntity(_moxieCountTextEntity);

            TextComponent _moxieCountTextComponent = new TextComponent(_moxieCountTextEntity, "/", Cameras.Static);
            _moxieCountTextEntity.AddInitialComponent(_moxieCountTextComponent);
            _moxieCountTextComponent.SetLayerDepth(Layers.UI_Foreground);

            SpatialComponent _moxieCountTextSC = new SpatialComponent(_moxieCountTextEntity);
            _moxieCountTextSC.SetPosition(new Vector2(0, 0));
            _moxieCountTextEntity.AddInitialComponent(_moxieCountTextSC);
            
            #endregion
            #endregion

            #region Moxie Info
            #region Create Text Entities
            healthText = new Entity();
            hungerText = new Entity();
            sleepText = new Entity();
            funText = new Entity();
            ageText = new Entity();

            #region Add Text Entities to Manager
            EntityManager.AddEntity(healthText);
            EntityManager.AddEntity(hungerText);
            EntityManager.AddEntity(sleepText);
            EntityManager.AddEntity(funText);
            EntityManager.AddEntity(ageText);
            #endregion
            #endregion

            #region Create Components
            TextComponent healthTextComponent = new TextComponent(healthText, "Health: ", Cameras.Static);
            healthTextComponent.SetLayerDepth(Layers.UI_Foreground);
            TextComponent hungerTextComponent = new TextComponent(hungerText, "Hunger: ", Cameras.Static);
            TextComponent sleepTextComponent = new TextComponent(sleepText, "Sleep: ", Cameras.Static);
            TextComponent funTextComponent = new TextComponent(funText, "Fun: ", Cameras.Static);
            TextComponent ageTextComponent = new TextComponent(ageText, "Age: ", Cameras.Static);

            SpatialComponent healthSpatialComponent = new SpatialComponent(healthText);
            healthSpatialComponent.SetPosition(new Vector2(10, 700));
            SpatialComponent hungerSpatialComponent = new SpatialComponent(hungerText);
            hungerSpatialComponent.SetPosition(new Vector2(10, 750));
            SpatialComponent sleepSpatialComponent = new SpatialComponent(sleepText);
            sleepSpatialComponent.SetPosition(new Vector2(210, 700));
            SpatialComponent funSpatialComponent = new SpatialComponent(funText);
            funSpatialComponent.SetPosition(new Vector2(210, 750));
            SpatialComponent ageSpatialComponent = new SpatialComponent(ageText);
            ageSpatialComponent.SetPosition(new Vector2(410, 700));
            #endregion

            #region Add Components to Entities

            healthText.AddInitialComponent(healthTextComponent);
            healthText.AddInitialComponent(healthSpatialComponent);

            hungerText.AddInitialComponent(hungerTextComponent);
            hungerText.AddInitialComponent(hungerSpatialComponent);

            sleepText.AddInitialComponent(sleepSpatialComponent);
            sleepText.AddInitialComponent(sleepTextComponent);

            funText.AddInitialComponent(funSpatialComponent);
            funText.AddInitialComponent(funTextComponent);

            ageText.AddInitialComponent(ageSpatialComponent);
            ageText.AddInitialComponent(ageTextComponent);

            #endregion
            #endregion      
            #endregion

            #region Item Screen
            UnlockItem(Item.FOOD_DISPENSER_BASIC);

            #endregion

        }

        /// <summary>
        /// Updates all of the updateable components
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
            #region Moxie Info
            #region Update Moxie Status Texts
            TextComponent healthTextComponent = (TextComponent)healthText.GetComponent(typeof(TextComponent));
            TextComponent hungerTextComponent = (TextComponent)hungerText.GetComponent(typeof(TextComponent));
            TextComponent sleepTextComponent = (TextComponent)sleepText.GetComponent(typeof(TextComponent));
            TextComponent funTextComponent = (TextComponent)funText.GetComponent(typeof(TextComponent));
            TextComponent ageTextComponent = (TextComponent)ageText.GetComponent(typeof(TextComponent));
            if (_targetMoxie != null)
            {
                healthTextComponent.SetText("Health: " + _targetMoxie.Health.ToString());
                hungerTextComponent.SetText("Hunger: " + _targetMoxie.Hunger.ToString());
                sleepTextComponent.SetText("Sleep: " + _targetMoxie.Sleep.ToString());
                funTextComponent.SetText("Fun: " + _targetMoxie.Fun.ToString());
                ageTextComponent.SetText("Age: " + _targetMoxie.Age.ToString());
            }

            if (UIState == UIStates.MoxieInfo)
            {
                healthTextComponent.SetVisible(true);
                hungerTextComponent.SetVisible(true);
                sleepTextComponent.SetVisible(true);
                funTextComponent.SetVisible(true);
                ageTextComponent.SetVisible(true);
            }
            else
            {
                healthTextComponent.SetVisible(false);
                hungerTextComponent.SetVisible(false);
                sleepTextComponent.SetVisible(false);
                funTextComponent.SetVisible(false);
                ageTextComponent.SetVisible(false);
            }
            #endregion
            #endregion

            #region Top Bar 
            #region Money Text
            TextComponent _moneyTextComponent = (TextComponent)_moneyTextEntity.GetComponent(typeof(TextComponent));
            _moneyTextComponent.SetText("$" + UISaveComponent.Money.ToString());
            #endregion

            #region Moxie Count
            TextComponent _moxieCountTextComponent = (TextComponent)_moxieCountTextEntity.GetComponent(typeof(TextComponent));
            _moxieCountTextComponent.SetText(GetMoxieCount().ToString() + "/" + GetMaxMoxies().ToString());
            #endregion
            #endregion

            //This is for testing for now. Might keep it might not.
            //Final version will have clickable tabs
            #region Input
            if(InputHandler.KeyPressed(Keys.D1))
            {
                ChangeUIState(UIStates.MoxieInfo);
            }
            if (InputHandler.KeyPressed(Keys.D2))
            {
                ChangeUIState(UIStates.Items);
            }
            if(InputHandler.KeyPressed(Keys.F4))
            {
                if (ThoughtBubblesVisible)
                    ThoughtBubblesVisible = false;
                else ThoughtBubblesVisible = true;
            }

            #endregion

            #region Two Moxies Selected Handler
            _resetTwoSelectedTimer.Update(gameTime);
            if (_resetTwoSelectedTimer.Done)
            {
                foreach (Entity entity in EntityManager.EntityMasterList)
                {
                    if (entity is MoxieEntity)
                    {
                        MoxieEntity moxie = (MoxieEntity)entity;
                        if (moxie.HasComponent(typeof(MoxieOutlineComponent)) && moxie != _targetMoxie)
                        {
                            moxie.RemoveComponent(typeof(MoxieOutlineComponent));
                        }
                    }
                }
            }
            #endregion
        }

        public static void Draw(GameTime gameTime)
        {
            //Only draw static sprites in here!
            float layer = (float)((float)Layers.UI_Background / 1000);
            //Draw the top bar
            MainController.StaticSpriteBatch.Draw(_UITexture, new Vector2(0, 0), new Rectangle(0, 0, _UITexture.Width, 25), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layer);

            //Draw the bottom bar
            MainController.StaticSpriteBatch.Draw(_UITexture, new Vector2(0, MainController.DefaultScreenHeight - (_UITexture.Height)), new Rectangle(0, 0, _UITexture.Width, _UITexture.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layer);
        }

        /// <summary>
        /// Call this to unlock a new item
        /// </summary>
        /// <param name="item"></param>
        public static void UnlockItem(Item item)
        {
            if(!ItemUnlocked(item))
            {
                GameSaveComponent UIS = (GameSaveComponent)_UISaveEntity.GetComponent(typeof(GameSaveComponent));
                _unlockedItems.Add(item);
                UIS.AddUnlockedItem(item);

                //add the new button entity with the UIiTemButtonComponent
                Entity UIButtonEntity = new Entity();
                int index = _itemScreenButtonEntities.Count;
                UIItemButtonComponent uibc = new UIItemButtonComponent(UIButtonEntity, item, index);
                UIButtonEntity.AddInitialComponent(uibc);

                //Add that entity to the master list as well as _itemScreenButtonEntities
                EntityManager.AddEntity(UIButtonEntity);
                _itemScreenButtonEntities.Add(UIButtonEntity);

                //Add the spatial component
                #region Set Spatial Component Position
            SpatialComponent sc = new SpatialComponent(UIButtonEntity);
            int evenYPosition = MainController.DefaultScreenHeight - 116;
            int oddYPosition = MainController.DefaultScreenHeight - 62;
            int xBasePosition = 4;
            int xSpacing = 4;
            int itemFrameWidth = 54;
            int framesPerRow = 5;
            Vector2 position =Vector2.Zero;
            //Top Row
            if (index < (framesPerRow))
            {
                position.Y = evenYPosition;
                position.X = (index * xSpacing) + (index * itemFrameWidth);
            }
            //Bottom Row
            else
            {
                position.Y = oddYPosition;
                int xPositionValue = index - framesPerRow;
                position.X = (xPositionValue * xSpacing) + (xPositionValue * itemFrameWidth);

            }
            position.X = position.X + xBasePosition + 27;
            sc.SetPosition(position);
            UIButtonEntity.AddInitialComponent(sc);
            #endregion

                ClickableComponent cc = new ClickableComponent(UIButtonEntity, Cameras.Static, new Rectangle((int)sc.Position.X - (int)(uibc.Texture.Width / 2), (int)sc.Position.Y - (int)(uibc.Texture.Height / 2), (int)uibc.Texture.Width, (int)uibc.Texture.Height), ClickTypes.LeftSingle);
                UIButtonEntity.AddInitialComponent(cc);
            }

        }

        /// <summary>
        /// Returns whether or not the item in question is unlocked
        /// </summary>
        /// <param name="item">The item you want to check</param>
        /// <returns>True/false if the item is unlocked or not</returns>
        public static bool ItemUnlocked(Item item)
        {
            if(_unlockedItems.Contains(item))
                return true;
            else return false;
        }

        /// <summary>
        /// Call this to change the UI state. (Moxie info, tank info, or item panel)
        /// </summary>
        /// <param name="newState"></param>
        public static void ChangeUIState(UIStates newState)
        {
            if (_UIState == newState)
                return;

            UIStates _previousState = _UIState;
            _UIState = newState;
        }

        /// <summary>
        /// Increases/Decreases the Moxie Count
        /// </summary>
        /// <param name="amount">amount to increase/decrease by</param>
        public static void ChangeMoxieCount(int amount)
        {
            GameSaveComponent gsc = (GameSaveComponent)_UISaveEntity.GetComponent(typeof(GameSaveComponent));
            gsc.MoxieCount += amount;
        }

        /// <summary>
        /// Returns the count of the moxies
        /// </summary>
        /// <returns></returns>
        public static int GetMoxieCount()
        {
            GameSaveComponent gsc = (GameSaveComponent)_UISaveEntity.GetComponent(typeof(GameSaveComponent));
            return gsc.MoxieCount;
        }

        /// <summary>
        /// Returns the max number of Moxies allowed
        /// </summary>
        /// <returns></returns>
        public static int GetMaxMoxies()
        {
            GameSaveComponent gsc = (GameSaveComponent)_UISaveEntity.GetComponent(typeof(GameSaveComponent));
            return gsc.MaxMoxies;
        }

        /// <summary>
        /// Sets the max number of moxies
        /// </summary>
        /// <param name="max"></param>
        public static void SetMaxMoxies(int max)
        {
            GameSaveComponent gsc = (GameSaveComponent)_UISaveEntity.GetComponent(typeof(GameSaveComponent));
            gsc.MaxMoxies = max;
        }

        #endregion

    }
}
