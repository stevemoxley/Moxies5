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
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Controllers;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Moxies5.Components.MoxieComponents.Actions
{
    public class ActionWaitForMateComponent: AbstractActionComponent, ISerialize
    {
        
        #region Fields
        private Timer _waitForMateTimer;
        private Timer _checkForMalesTimer = new Timer(2);
        private Timer _checkForContactTimer = new Timer(1);
        public bool FoundMate = false;
        private MoxieEntity _mate = null;
        
        #endregion

        #region Properties
        public Timer WaitForMateTimer
        {
            get
            {
                return _waitForMateTimer;
            }
        }
        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionWaitForMateComponent(Entity parentEntity)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            float timeToWait = 120; //This could change

            _waitForMateTimer = new Timer(timeToWait);
            Name = "ActionWaitForMateComponent";

            if (Parent != null)
            {
                if (Parent.HasComponent(typeof(MoxieGeneticsComponent)))
                {
                    MoxieGeneticsComponent genes = (MoxieGeneticsComponent)Parent.GetComponent(typeof(MoxieGeneticsComponent));

                    if (genes.Gender != GenderTrait.Female)
                    {
                        throw new Exception("Only females can have this component");
                    }
                }
            }

            ThoughtBubbleTexture = "reproducing";


        }



        public override void Update(GameTime gameTime)
        {
            #region UpdateTimers
            _checkForContactTimer.Update(gameTime);
            _checkForMalesTimer.Update(gameTime);
            _waitForMateTimer.Update(gameTime);
            #endregion

            #region Check if a possible mate is touching the moxie
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
                            if (moxie.Genetics.Gender == GenderTrait.Male)
                            {
                                if (Moxie.Genetics.TestIfMoxieIsPossibleMate(moxie) && !moxie.HasComponent(typeof(RecentlyReproducedComponent))) 
                                {
                                    FoundMate = true;
                                    _mate = moxie;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Mate Found
            if (FoundMate)
            {
                Finish();

                //Lay the egg
                ThoughtProcess.SetAction(new ActionLayEggComponent(Parent, _mate));

                //Add recently reproduced component
                RecentlyReproducedComponent rrC = new RecentlyReproducedComponent(Parent);
                Parent.AddComponent(rrC);
            }
            #endregion

            #region Wait For the mate timer

            if (_checkForMalesTimer.Done)
            {
                bool maleFound = true;
                for (int i = 0; i < EntityManager.EntityCount; i++)
                {
                    if (EntityManager.EntityMasterList[i].GetType() == typeof(MoxieEntity))
                    {
                        MoxieEntity possibleMate = (MoxieEntity)EntityManager.EntityMasterList[i];
                        if (possibleMate.Genetics.Gender == GenderTrait.Male && possibleMate.HasComponent(typeof(ActionFindMateComponent)))
                        {
                            maleFound = true;
                            break;
                        }
                    }
                }
                if (!maleFound)
                {
                    Finish();
                }
                if (_waitForMateTimer.Done)
                {
                    Finish();
                }
            }
            #endregion
            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            ActionWaitForMateComponentSave save = new ActionWaitForMateComponentSave();
            save.Serialize(this, ID);
            return save;
        }

    }

    public class ActionWaitForMateComponentSave : SaveObject
    {
        ActionWaitForMateComponent component = new ActionWaitForMateComponent(null);

        public override void Serialize(object _toSerialize, int ID)
        {
            ActionWaitForMateComponent component = (ActionWaitForMateComponent)_toSerialize;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            ActionWaitForMateComponentSave sSave = (ActionWaitForMateComponentSave)save;
            return component;
        }

    }


}
