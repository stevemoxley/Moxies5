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

namespace Moxies5.Components.ItemComponents
{
    public class FoodComponent: Component, ISerialize
    {
        
        #region Fields
        private int _hungerRestore = 0;
        private bool _feeder = false;
        private bool _permanent;
        #endregion

        #region Properties
        public int HungerRestore
        {
            get
            {
                return _hungerRestore;
            }
        }

        public bool Feeder
        {
            get
            {
                return _feeder;
            }
        }

        public bool Permanent
        {
            get
            {
                return _permanent;
            }
            set
            {
                _permanent = value;
            }
        }
        #endregion

        #region Getters and Setters
        public int GetHungerRestore()
        {
            return _hungerRestore;
        }

        public void SetHungerRestore(int amount)
        {
            this._hungerRestore = amount;
        }

        public bool GetPermanent()
        {
            return _permanent;
        }

        public void SetPermanent(bool value)
        {
            this._permanent = value;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity"></param>
        /// <param name="hungerRestore">How much hunger this food restores</param>
        public FoodComponent(Entity parentEntity, int hungerRestore, bool permanent)
            : base(parentEntity)
        {
            UpdateOrder = 1;
            Name = "FoodComponent";
            this._hungerRestore = hungerRestore;
            this._permanent = permanent;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            FoodComponentSave save = new FoodComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class FoodComponentSave : SaveObject
    {
        FoodComponent foodComponent = new FoodComponent(null, 0, false);
        public int HungerRestore;
        public bool Feeder;
        public bool Permanent;

        public override void Serialize(object _foodComponent, int ID)
        {
            FoodComponent foodComponent = (FoodComponent)_foodComponent;
            this.HungerRestore = foodComponent.HungerRestore;
            this.Feeder = foodComponent.Feeder;
            this.Permanent = foodComponent.Permanent;
            base.Serialize(_foodComponent, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            FoodComponentSave fSave = (FoodComponentSave)save;
            foodComponent.SetHungerRestore(fSave.HungerRestore);
            foodComponent.Permanent = fSave.Permanent;
            return foodComponent;
        }

    }


}
