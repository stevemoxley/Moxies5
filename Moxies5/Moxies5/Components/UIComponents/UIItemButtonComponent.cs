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
using Moxies5.Controllers;
using Moxies5.Utilities;
using Moxies5.Serialization;
using Moxies5.Components.ItemComponents;

namespace Moxies5.Components
{
    public class UIItemButtonComponent: DrawableComponent, ISerialize
    {
        #region Fields
        public Item Item { get; set; }
        private Texture2D _itemTexture;
        private int _index;
        private bool _subbedToClickEvent = false;
        #endregion

        #region Properties
        public int Index
        {
            get
            {
                return _index;
            }
        }
        #endregion

        /// <summary>
        /// This is a UI item button. Pressing the button allows placement of an item
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        /// <param name="index">This is the index in the item screen that the component sits at. 0 index</param>
        public UIItemButtonComponent(Entity parentEntity, Item item, int index)
            : base(parentEntity, "UI/itemFrame", Cameras.Static)
        {
            this.Item = item;
            this._index = index;
            Name = "UIItemButtonComponent";
            UpdateOrder = 4;
            SetLayerDepth(Layers.UI_Foreground);
        }

        public override void Update(GameTime gameTime)
        {
            if (!_subbedToClickEvent)
            {
                if(Parent.HasComponent(typeof(ClickableComponent)))
                {
                    ClickableComponent cc = (ClickableComponent)Parent.GetComponent(typeof(ClickableComponent));
                    cc.OnClick += new ClickableComponent.ClickHandler(cc_OnClick);
                    _subbedToClickEvent = true;
                }
                else
                {
                    throw new Exception("Parent needs a clickable component");
                }
            }
            //Add an item placement component to the parent
            base.Update(gameTime);
        }

        //This is called when the clickable component of the parent is clicked
        void cc_OnClick(EventArgs e)
        {
            //Make sure no other entity has an item placement component first
            for (int i = 0; i < EntityManager.EntityCount; i++)
            {
                if (EntityManager.EntityMasterList[i].HasComponent(typeof(ItemPlacementComponent)))
                {
                    return;
                }
            }

            Entity testPlacementEntity = new Entity();

            SpatialComponent sc = new SpatialComponent(testPlacementEntity);
            testPlacementEntity.AddInitialComponent(sc);

            SensorComponent sensor = new SensorComponent(testPlacementEntity);
            sensor.CreateSensorRectangleBody(ConvertUnits.ToSimUnits(Tile.tileWidth), ConvertUnits.ToSimUnits(Tile.tileHeight), 1);
            testPlacementEntity.AddInitialComponent(sensor);

            ItemPlacementComponent ipc = new ItemPlacementComponent(testPlacementEntity, Item);
            testPlacementEntity.AddInitialComponent(ipc);

            EntityManager.AddEntity(testPlacementEntity);
        }

        public override void Draw(GameTime gameTime)
        {
            if (UIController.UIState == UIStates.Items)
            {
                if (Visible)
                {
                    if (Parent.HasComponent(typeof(SpatialComponent)))
                    {
                        if (_itemTexture == null)
                        {
                            _itemTexture = GetTextureFromItem(Item);
                        }
                        SpatialComponent spatialComponent = (SpatialComponent)Parent.GetComponent(typeof(SpatialComponent));
                        Vector2 _itemTexturePosition = new Vector2(spatialComponent.Position.X + 4, spatialComponent.Position.Y + 4);

                        //This will be the texture of the item
                        SpriteBatch.Draw(_itemTexture, _itemTexturePosition, new Rectangle(0,0,_itemTexture.Width, _itemTexture.Height), Color * Transparency, spatialComponent.Rotation, Origin, Scale, SpriteEffects.None, LayerDepth);
                        //Make sure to add in the background texture here
                        SpriteBatch.Draw(Texture, spatialComponent.Position, SourceRectangle, Color * Transparency, spatialComponent.Rotation, Origin, Scale, SpriteEffects.None, (float)Layers.UI_Midground/1000);
                        //Also add some text
                    }
                    else
                    {
                        throw new Exception("Entities with a drawable component also need a spatial component");
                    }
                }
            }
        }

        private Texture2D GetTextureFromItem(Item item)
        {
            Texture2D _texture = null;
            string path = "UI/ItemFrame/";
            switch (item)
            {
                case Item.FOOD_DISPENSER_BASIC:
                    {
                        _texture = MainController.Game.Content.Load<Texture2D>(path + "foodItem_ItemFrame");
                        break;
                    }
                case Item.MONEY_MAKING_BASIC:
                    {
                        _texture = MainController.Game.Content.Load<Texture2D>(path + "moneyItem_ItemFrame");
                        break;
                    }
                default:
                    {
                        throw new Exception("No item texture found");
                    }
            }


            return _texture;
        }



        SaveObject ISerialize.Serialize(int ID)
        {
            UIItemButtonComponentSave save = new UIItemButtonComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class UIItemButtonComponentSave : SaveObject
    {
        UIItemButtonComponent _toDeserialize = new UIItemButtonComponent(null, Item.DECO_BUSH_LARGE, -1);
        public Item Item;
        public int Index;

        public override void Serialize(object _toSerialize, int ID)
        {
            UIItemButtonComponent toSerialize = (UIItemButtonComponent)_toSerialize;
            this.Item = toSerialize.Item;
            this.Index = toSerialize.Index;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            UIItemButtonComponentSave _save = (UIItemButtonComponentSave)save;
            _toDeserialize = new UIItemButtonComponent(null, _save.Item, _save.Index);
            return _toDeserialize;
        }

    }
}
