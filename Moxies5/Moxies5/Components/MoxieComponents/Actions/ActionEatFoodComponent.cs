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
using Moxies5.Components.ItemComponents;

namespace Moxies5.Components.MoxieComponents.Actions
{
    public class ActionEatFoodComponent: AbstractActionComponent
    {
        
        #region Fields
        private FoodComponent _foodToEat;
        #endregion

        #region Properties

        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionEatFoodComponent(Entity parentEntity, FoodComponent foodToEat)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "ActionEatFoodComponent";

            this._foodToEat = foodToEat;
        }

        public override void Update(GameTime gameTime)
        {
            if (_foodToEat != null)
            {
                Entity foodEntity = _foodToEat.Parent;
                Moxie.SetHunger(Moxie.Hunger + _foodToEat.HungerRestore);

                if (!_foodToEat.Permanent)
                {
                    foodEntity.Remove();
                    _foodToEat = null;
                }

                Finish();
            }

            base.Update(gameTime);
        }
    }
}
