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
using Moxies5.Controllers;

namespace Moxies5.Components.MoxieComponents.Actions
{
    public class ActionUseMoneyMakingComponent: AbstractActionComponent
    {
        
        #region Fields
        private MoneyMakingComponent _moneyMaker;
        #endregion

        #region Properties

        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionUseMoneyMakingComponent(Entity parentEntity, MoneyMakingComponent moneyMaker)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "ActionUseMoneyMakingComponent";

            this._moneyMaker = moneyMaker;
        }

        public override void Update(GameTime gameTime)
        {
            if (_moneyMaker != null)
            {
                Entity moneyMaker = _moneyMaker.Parent;

                //Get moxie productivity
                float multiplier = -1;
                switch (Moxie.Genetics.DProductivityTrait)
                {
                    case ProductivityTraits.Slowest:
                        {
                            multiplier = 0.25f;
                            break;
                        }
                    case ProductivityTraits.Slower:
                        {
                            multiplier = 0.5f;
                            break;
                        }
                    case ProductivityTraits.Slow:
                        {
                            multiplier = 0.75f;
                            break;
                        }
                    case ProductivityTraits.Normal:
                        {
                            multiplier = 1;
                            break;
                        }
                    case ProductivityTraits.Fast:
                        {
                            multiplier = 1.25f;
                            break;
                        }
                    case ProductivityTraits.Faster:
                        {
                            multiplier = 1.5f;
                            break;
                        }
                    case ProductivityTraits.Fastest:
                        {
                            multiplier = 2f;
                            break;
                        }
                    default:
                        {
                            throw new Exception("You forgot something");
                        }
                }

                //Add the money
                UIController.UISaveComponent.ChangeMoney((int)(_moneyMaker.MoneyAmount * multiplier));

                Finish();
            }
            base.Update(gameTime);
        }
    }
}
