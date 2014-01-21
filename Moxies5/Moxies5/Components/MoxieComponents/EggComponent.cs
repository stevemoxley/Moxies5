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
using Moxies5.Controllers;
using Moxies5.Utilities;
using Moxies5.Serialization;


namespace Moxies5.Components.MoxieComponents
{
    public class EggComponent : Component, ISerialize
    {

        public Timer HatchTimer = new Timer(10);
        private MoxieGeneticsComponent _genetics;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public EggComponent(Entity parentEntity, MoxieGeneticsComponent genetics)
            : base(parentEntity)
        {
            Name = "EggComponent";
            UpdateOrder = 1;
            this._genetics = genetics;
        }

        public override void Update(GameTime gameTime)
        {
            HatchTimer.Update(gameTime);
            if (HatchTimer.Done)
            {
                //Hatch the Moxie
                MoxieEntity newMoxie = new MoxieEntity();
                newMoxie.Initialize();
                EntityManager.AddEntity(newMoxie);
                SpatialComponent moxieSpatialComponent = (SpatialComponent)newMoxie.GetComponent(typeof(SpatialComponent));
                SpatialComponent eggSpatialComponent = (SpatialComponent)Parent.GetComponent(typeof(SpatialComponent));
                moxieSpatialComponent.SetPosition(eggSpatialComponent.Position);
                _genetics.SetParent(newMoxie);
                newMoxie.AddInitialComponent(_genetics);
                newMoxie.Genetics.SetBodyColor();
                newMoxie.SetMaxAge();

                //Remove the parent component
                Parent.Remove();
            }

            if (_genetics == null)
            {
                if (Parent.HasComponent(typeof(MoxieGeneticsComponent)))
                {
                    _genetics = (MoxieGeneticsComponent)Parent.GetComponent(typeof(MoxieGeneticsComponent));
                }
                else
                {
                    throw new Exception("Parent must have a moxie genetics component");
                }
            }

            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            EggComponentSave save = new EggComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class EggComponentSave : SaveObject
    {
        EggComponent component = new EggComponent(null, null);
        public float TimeLeftOnTimer = -1;

        public override void Serialize(object _toSerialize, int ID)
        {
            EggComponent component = (EggComponent)_toSerialize;
            this.TimeLeftOnTimer = component.HatchTimer.ElapsedTime;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            EggComponentSave sSave = (EggComponentSave)save;
            component.HatchTimer.ElapsedTime = sSave.TimeLeftOnTimer;
            return component;
        }

    }
}
