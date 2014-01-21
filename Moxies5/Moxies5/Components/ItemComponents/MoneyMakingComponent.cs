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
    public class MoneyMakingComponent: Component, ISerialize
    {
        
        #region Fields
        private int _moneyAmount;
        #endregion

        #region Properties
        public int MoneyAmount
        {
            get
            {
                return _moneyAmount;
            }
        }
        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity"></param>
        /// <param name="moneyAmount">The amount of money given for each use of this component</param>
        public MoneyMakingComponent(Entity parentEntity, int moneyAmount)
            : base(parentEntity)
        {
            UpdateOrder = 1;
            Name = "MoneyMakingComponent";
            this._moneyAmount = moneyAmount;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            MoneyMakingComponentSave save = new MoneyMakingComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class MoneyMakingComponentSave : SaveObject
    {
        public int MoneyAmount;


        public override void Serialize(object _toSerialize, int ID)
        {
            MoneyMakingComponent toSerialize = (MoneyMakingComponent)_toSerialize;
            this.MoneyAmount = toSerialize.MoneyAmount;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            MoneyMakingComponentSave fSave = (MoneyMakingComponentSave)save;
            MoneyMakingComponent component = new MoneyMakingComponent(null, fSave.MoneyAmount);
            return component;
        }

    }


}
