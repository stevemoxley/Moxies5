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
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Controllers;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Moxies5.Components.MoxieComponents.Actions
{
    public class ActionFindMateComponent: AbstractActionComponent, ISerialize
    {
        public enum MatingStages
        {
            LookForMate,
            Mate
        }
        
        #region Fields
        private MatingStages _matingStage;
        private Timer _checkAreaForMatesTimer = new Timer(2);
        #endregion

        #region Properties
        public MatingStages MatingStage
        {
            get
            {
                return _matingStage;
            }
        }

        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionFindMateComponent(Entity parentEntity, MatingStages matingStage)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "ActionFindMateComponent";
            this._matingStage = matingStage;

            if (Parent != null)
            {
                if (Parent.HasComponent(typeof(MoxieGeneticsComponent)))
                {
                    MoxieGeneticsComponent genes = (MoxieGeneticsComponent)Parent.GetComponent(typeof(MoxieGeneticsComponent));
                    if (genes.Gender != GenderTrait.Male)
                    {
                        throw new Exception("Only males can have this component");
                    }
                }
            }



        }

        public override void Update(GameTime gameTime)
        {
            _checkAreaForMatesTimer.Update(gameTime);

            if (MatingStage == MatingStages.LookForMate)
            {
                MoxieEntity bestMateMatch = null;
                //Find a mate
                for (int i = 0; i < EntityManager.EntityCount; i++)
                {
                    Entity testEntity = EntityManager.EntityMasterList[i];
                    if (testEntity.GetType() == typeof(MoxieEntity))
                    {
                        //Test the match rating of each mate
                        MoxieEntity testMoxie = (MoxieEntity)testEntity;
                        //Also can't mate with a Moxie that has recently reproduced
                        if (testMoxie.HasComponent(typeof(ActionWaitForMateComponent)) &&
                            !testMoxie.HasComponent(typeof(RecentlyReproducedComponent)))
                        {
                            if (Moxie.Genetics.TestIfMoxieIsPossibleMate(testMoxie))
                            {
                                bestMateMatch = testMoxie;
                            }
                        }
                    }
                }

                if (bestMateMatch != null)
                {
                    Finish();

                    SpatialComponent closestSC = (SpatialComponent)bestMateMatch.GetComponent(typeof(SpatialComponent));

                    ActionMoveToTargetComponent moveAction = new ActionMoveToTargetComponent(Parent, closestSC, new ActionFindMateComponent(Parent, MatingStages.Mate));

                    ThoughtProcess.SetAction(moveAction);
                }
                else
                {
                    Finish();
                }
            }
            else if (_matingStage == MatingStages.Mate)
            {
                if (_checkAreaForMatesTimer.Done)
                {
                    if (Parent.HasComponent(typeof(PhysicsComponent)))
                    {
                        PhysicsComponent pc = (PhysicsComponent)Parent.GetComponent(typeof(PhysicsComponent));
                        List<Body> bodiesInContact = pc.GetBodiesInContactWithBody();
                        for (int i = 0; i < bodiesInContact.Count; i++)
                        {
                            if (bodiesInContact[i].UserData != null)
                            {
                                if (bodiesInContact[i].UserData.GetType() == typeof(MoxieEntity))
                                {
                                    MoxieEntity moxie = (MoxieEntity)bodiesInContact[i].UserData;
                                    if (moxie.Genetics.Gender == GenderTrait.Female)
                                    {
                                        //Add recently reproduced component
                                        RecentlyReproducedComponent rrC = new RecentlyReproducedComponent(Parent);
                                        Parent.AddComponent(rrC);
                                    }
                                }
                            }
                        }
                    }
                }
                Finish();
            }

            base.Update(gameTime);
        }


        public SaveObject Serialize(int ID)
        {
            ActionFindMateComponentSave save = new ActionFindMateComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class ActionFindMateComponentSave : SaveObject
    {
        public ActionFindMateComponent.MatingStages MatingStage;

        public override void Serialize(object _toSerialize, int ID)
        {
            ActionFindMateComponent component = (ActionFindMateComponent)_toSerialize;
            this.MatingStage = component.MatingStage;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            ActionFindMateComponentSave sSave = (ActionFindMateComponentSave)save;
            ActionFindMateComponent component = new ActionFindMateComponent(null, sSave.MatingStage);
            return component;
        }

    }

}
