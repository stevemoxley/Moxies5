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
    public class ActionFindMoneyMakerComponent: AbstractActionComponent
    {
        
        #region Fields
        #endregion

        #region Properties

        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionFindMoneyMakerComponent(Entity parentEntity)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "ActionFindMoneyMakerComponent";
        }

        public override void Update(GameTime gameTime)
        {
            Entity closestMoneyMaker = null;
            float closestDistance = float.MaxValue;

            //Find a money maker
            for (int i = 0; i < EntityManager.EntityCount; i++)
            {
                Entity testEntity = EntityManager.EntityMasterList[i];
                if (testEntity.HasComponent(typeof(MoneyMakingComponent)))
                {
                    SpatialComponent sc = (SpatialComponent)testEntity.GetComponent(typeof(SpatialComponent));
                    SpatialComponent moxieSC = (SpatialComponent)Moxie.GetComponent(typeof(SpatialComponent));
                    //Get the distance to the money maker
                    float distance = Vector2.Distance(moxieSC.Position, sc.Position);
                    if (!Moxie.EntityIgnoreList.Contains(testEntity))
                    {
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestMoneyMaker = testEntity;
                        }
                    }
                }
            }

            if (closestMoneyMaker != null)
            {
                Finish();
                SpatialComponent closestSC = (SpatialComponent)closestMoneyMaker.GetComponent(typeof(SpatialComponent));
                //move to it
                ActionMoveToTargetComponent moveAction = new ActionMoveToTargetComponent(Parent, closestSC, new ActionUseMoneyMakingComponent(Moxie, (MoneyMakingComponent)closestMoneyMaker.GetComponent(typeof(MoneyMakingComponent)))); 
                ThoughtProcess.SetAction(moveAction);
            }
            else
            {
                Finish();
            }

            base.Update(gameTime);
        }


    }

}
