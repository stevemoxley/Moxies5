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
    public class ActionFindFoodComponent: AbstractActionComponent
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
        public ActionFindFoodComponent(Entity parentEntity)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "ActionFindFoodComponent";

            ThoughtBubbleTexture = "hungry";
        }

        public override void Update(GameTime gameTime)
        {
            Entity closestFoodEntity = null;
            float closestDistance = float.MaxValue;

            //Find something to eat
            for (int i = 0; i < EntityManager.EntityCount; i++)
            {
                Entity testEntity = EntityManager.EntityMasterList[i];
                if (testEntity.HasComponent(typeof(FoodComponent)))
                {
                    FoodComponent food = (FoodComponent)testEntity.GetComponent(typeof(FoodComponent));
                    SpatialComponent sc = (SpatialComponent)testEntity.GetComponent(typeof(SpatialComponent));
                    SpatialComponent moxieSC = (SpatialComponent)Moxie.GetComponent(typeof(SpatialComponent));
                    //Get the distance to the food
                    float distance = Vector2.Distance(moxieSC.Position, sc.Position);
                    if (!Moxie.EntityIgnoreList.Contains(testEntity))
                    {
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestFoodEntity = testEntity;
                        }
                    }
                }
            }

            if (closestFoodEntity != null)
            {
                Finish();
                SpatialComponent closestSC = (SpatialComponent)closestFoodEntity.GetComponent(typeof(SpatialComponent));
                //move to it
                ActionMoveToTargetComponent moveAction = new ActionMoveToTargetComponent(Parent, closestSC, new ActionEatFoodComponent(Parent, (FoodComponent)closestFoodEntity.GetComponent(typeof(FoodComponent)))); //Set your after move action to eat it
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
