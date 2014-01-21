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
using Moxies5.Serialization;
using Moxies5.Entities;
using Moxies5.Utilities;
using Moxies5.Components.ItemComponents;
using Moxies5.Components.MoxieComponents.Actions;
using Moxies5.Components.MoxieComponents;
using Moxies5.Controllers;

namespace Moxies5.Entities
{
    public class ThoughtProcess
    {
        #region Fields
        private MoxieEntity _moxie;
        private AbstractActionComponent _action = null;
        #endregion

        #region Properties
        public AbstractActionComponent Action
        {
            get
            {
                return _action;
            }
        }

        public MoxieEntity Moxie
        {
            get
            {
                return _moxie;
            }
        }
        #endregion

        #region Getters and Setters
        public void SetAction(AbstractActionComponent action)
        {
            this._action = action;

            if (action != null)
            {
                if (!_moxie.HasComponent(action.GetType()))
                {
                    _moxie.AddInitialComponent(action);
                }
            }
        }
        #endregion

        #region Constructor
        public ThoughtProcess(MoxieEntity moxie)
        {
            this._moxie = moxie;
        }
        #endregion

        #region Methods
        /// <summary>
        /// This is the logic that will determine which action the Moxie should perform
        /// This is a very important method
        /// </summary>
        public void FindAction()
        {

            //Make sure to call return after an action has been set

            #region Interrupting Actions

            #region Death

            #region Zero Health Death
            if (Moxie.Health == 0)
            {
                if (_action == null)
                {
                     SetAction(new ActionDeathComponent(_moxie));
                }
                else
                {
                    if (_action.GetType() != typeof(ActionDeathComponent))
                        SetAction(new ActionDeathComponent(_moxie));
                }
                return;
            }
            #endregion

            #region Old Age Death
            if (Moxie.Age >= Moxie.MaxAge)
            {
                if (_action == null)
                {
                    SetAction(new ActionDeathComponent(_moxie));
                }
                else
                {
                    if (_action.GetType() != typeof(ActionDeathComponent))
                        SetAction(new ActionDeathComponent(_moxie));
                }
                return;
            }
            #endregion

            #endregion

            #endregion

            if (_action == null)
            {
                #region Reproduction
                float reproductionLevel = (Moxie.Health + Moxie.Hunger + Moxie.Fun + Moxie.Sleep) / 4;
                if ((reproductionLevel / 100) >= Constants.REPRODUCTION_AVERAGE)
                {
                    if (!_moxie.HasComponent(typeof(RecentlyReproducedComponent)))
                    {
                        if (_moxie.Genetics.Gender == GenderTrait.Male)
                        {
                            SetAction(new ActionFindMateComponent(_moxie, ActionFindMateComponent.MatingStages.LookForMate));
                            return;
                        }
                        if(_moxie.Genetics.Gender == GenderTrait.Female)
                        {
                            SetAction(new ActionWaitForMateComponent(_moxie));
                            return;
                        }
                    }
                }
                #endregion

                //Take care of basic needs
                #region Eat Food
                if (Moxie.Hunger < 80)
                {
                    if (FoodAvailable())
                    {
                        //Find food
                        SetAction(new ActionFindFoodComponent(_moxie));
                        return;
                    }
                }
                #endregion

                #region Sleep
                if (Moxie.Sleep < 20)
                {
                    //Sleep
                    SetAction(new ActionSleepComponent(_moxie));
                    return;

                }
                #endregion

                //Produce something if all needs are met
                #region Production
                //Random chance to decide to produce based on Productivity Trait
                if (MoneyMakerAvailable())
                {
                    int productionRand = MainController.Random.Next(1, 101);
                    switch (_moxie.Genetics.DProductivityTrait)
                    {
                        case ProductivityTraits.Slowest:
                            {
                                if (productionRand <= 30)
                                {
                                    SetAction(new ActionFindMoneyMakerComponent(_moxie));
                                    return;
                                }
                                break;
                            }
                        case ProductivityTraits.Slower:
                            {
                                if (productionRand <= 35)
                                {
                                    SetAction(new ActionFindMoneyMakerComponent(_moxie));
                                    return;
                                }
                                break;
                            }
                        case ProductivityTraits.Slow:
                            {
                                if (productionRand <= 40)
                                {
                                    SetAction(new ActionFindMoneyMakerComponent(_moxie));
                                    return;
                                }
                                break;
                            }
                        case ProductivityTraits.Normal:
                            {
                                if (productionRand <= 45)
                                {
                                    SetAction(new ActionFindMoneyMakerComponent(_moxie));
                                    return;
                                }
                                break;
                            }
                        case ProductivityTraits.Fast:
                            {
                                if (productionRand <= 50)
                                {
                                    SetAction(new ActionFindMoneyMakerComponent(_moxie));
                                    return;
                                }
                                break;
                            }
                        case ProductivityTraits.Faster:
                            {
                                if (productionRand <= 55)
                                {
                                    SetAction(new ActionFindMoneyMakerComponent(_moxie));
                                    return;
                                }
                                break;
                            }
                        case ProductivityTraits.Fastest:
                            {
                                if (productionRand <= 60)
                                {
                                    SetAction(new ActionFindMoneyMakerComponent(_moxie));
                                    return;
                                }
                                break;
                            }
                        default:
                            {
                                throw new Exception("No case set for this trait");
                            }
                    }
                }
                #endregion

                //Do something random
                #region Random Movement
                SetAction(new ActionRandomMoveComponent(_moxie));
                return;
                #endregion
            }

            //Error Handling:
            #region No action Found after thought Process
            throw new Exception("Went through action throught process and didn't determine an action");
            #endregion 
        }

        private bool FoodAvailable()
        {
            for (int i = 0; i < EntityManager.EntityCount; i++)
            {
                if (EntityManager.EntityMasterList[i].HasComponent(typeof(FoodComponent)) && !Moxie.EntityIgnoreList.Contains(EntityManager.EntityMasterList[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private bool MoneyMakerAvailable()
        {
            for (int i = 0; i < EntityManager.EntityCount; i++)
            {
                if (EntityManager.EntityMasterList[i].HasComponent(typeof(MoneyMakingComponent)) && !Moxie.EntityIgnoreList.Contains(EntityManager.EntityMasterList[i]))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        

    }
}
