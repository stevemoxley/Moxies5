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
    public class RecentlyReproducedComponent : Component, ISerialize
    {

        private Timer _timeToBreedAgain;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public RecentlyReproducedComponent(Entity parentEntity)
            : base(parentEntity)
        {
            Name = "RecentlyReproducedComponent";
            float breedTime = -1;
            if (parentEntity != null)
            {
                if(parentEntity.HasComponent(typeof(MoxieGeneticsComponent)))
                {
                    MoxieGeneticsComponent genes = (MoxieGeneticsComponent)parentEntity.GetComponent(typeof(MoxieGeneticsComponent));
                    if (genes.Gender == GenderTrait.Male)
                    {
                        breedTime = 30;
                    }
                    else
                        breedTime = 600;
                }
            }

            _timeToBreedAgain = new Timer(breedTime);
            UpdateOrder = 1;
        }

        public override void Update(GameTime gameTime)
        {
            _timeToBreedAgain.Update(gameTime);
            if (_timeToBreedAgain.Done)
            {
                Parent.RemoveComponent(Name);
            }
            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            RecentlyReproducedComponentSave save = new RecentlyReproducedComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class RecentlyReproducedComponentSave : SaveObject
    {
        RecentlyReproducedComponent component = new RecentlyReproducedComponent(null);

        public override void Serialize(object _toSerialize, int ID)
        {
            RecentlyReproducedComponent component = (RecentlyReproducedComponent)_toSerialize;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            RecentlyReproducedComponentSave sSave = (RecentlyReproducedComponentSave)save;
            return component;
        }

    }
}
