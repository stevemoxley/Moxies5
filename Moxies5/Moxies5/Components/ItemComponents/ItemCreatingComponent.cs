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
    public class ItemCreatingComponent: Component, ISerialize
    {
        
        #region Fields
        private Item _itemToCreate;
        private Timer _timer;
        private Vector2 _createPosition;
        #endregion

        #region Properties
        public Item Item
        {
            get
            {
                return _itemToCreate;
            }
        }

        public Timer Timer
        {
            get
            {
                return _timer;
            }
        }

        public Vector2 CreatePosition
        {
            get
            {
                return _createPosition;
            }
        }
        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// This will create a new item every time the timer ticks
        /// </summary>
        /// <param name="parentEntity"></param>
        public ItemCreatingComponent(Entity parentEntity, Item itemToCreate, float timeBetweenCreations)
            : base(parentEntity)
        {
            _timer = new Timer(timeBetweenCreations);
            this._itemToCreate = itemToCreate;
            
            UpdateOrder = 1;
            Name = "ItemCreatingComponent";
        }

        /// <summary>
        /// This will create a new item once and then remove itself from the Parent
        /// </summary>
        /// <param name="parentEntity"></param>
        public ItemCreatingComponent(Entity parentEntity, Item itemToCreate)
            : base(parentEntity)
        {
            this._itemToCreate = itemToCreate; 
            
            UpdateOrder = 1;
            Name = "ItemCreatingComponent";
        }

        public override void Update(GameTime gameTime)
        {

            if (Parent.HasComponent(typeof(SpatialComponent)))
            {
                SpatialComponent sc = (SpatialComponent)Parent.GetComponent(typeof(SpatialComponent));
                this._createPosition = sc.Position;
            }


            if (_timer != null)
            {
                _timer.Update(gameTime);
                if (_timer.Done)
                {
                    Entity orange = EntityFactory.CreateItem(_itemToCreate, _createPosition);
                    EntityManager.AddEntity(orange);
                }
            }
            else
            {
                EntityManager.AddEntity(EntityFactory.CreateItem(_itemToCreate, _createPosition));
                Parent.RemoveComponent(Name);
            }
            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            ItemCreatingComponentSave save = new ItemCreatingComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class ItemCreatingComponentSave : SaveObject
    {
        ItemCreatingComponent icc = null;
        public Item Item;
        public float TimerTimeBetween = -1;
        public float TimerTimeRemaining = -1;
        public float CreatePositionX;
        public float CreatePositionY;

        public override void Serialize(object _itemCreatingComponent, int ID)
        {
            ItemCreatingComponent itemCreatingComponent = (ItemCreatingComponent)_itemCreatingComponent;
            this.Item = itemCreatingComponent.Item;
            this.CreatePositionX = itemCreatingComponent.CreatePosition.X;
            this.CreatePositionY = itemCreatingComponent.CreatePosition.Y;
            if (itemCreatingComponent.Timer != null)
            {
                Timer timer = itemCreatingComponent.Timer;
                this.TimerTimeBetween = timer.TimeToCount;
                this.TimerTimeRemaining = timer.ElapsedTime;
            }
            base.Serialize(_itemCreatingComponent, ID);
        }

        public override object Deserialize(SaveObject toDeserialize)
        {
            ItemCreatingComponentSave iccSave = (ItemCreatingComponentSave)toDeserialize;
            if (iccSave.TimerTimeBetween != -1)
            {
                icc = new ItemCreatingComponent(null, iccSave.Item, iccSave.TimerTimeBetween);
                icc.Timer.ElapsedTime = iccSave.TimerTimeRemaining;
            }
            else
            {
                icc = new ItemCreatingComponent(null, Item);
            }
            return icc;
        }
    }
}
