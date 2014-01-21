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
    public class ActionSleepComponent: AbstractActionComponent, ISerialize
    {
        
        #region Fields
        Timer _sleepingTimer;
        Timer _healthRestoreTimer;
        #endregion

        #region Properties

        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionSleepComponent(Entity parentEntity)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "ActionDeathComponent";
            ThoughtBubbleTexture = "sleeping";
            _sleepingTimer = new Timer(1);
            _healthRestoreTimer = new Timer(3);
        }

        public override void Update(GameTime gameTime)
        {
            _healthRestoreTimer.Update(gameTime);
            _sleepingTimer.Update(gameTime);
            MoxieEyeComponent eyeComponent = (MoxieEyeComponent)Moxie.GetComponent(typeof(MoxieEyeComponent));
            if (Moxie.Sleep == 100)
            {
                eyeComponent.SetEyeState(MoxieEyeComponent.EyeStates.Open);
                Finish();
            }
            else
            {
                eyeComponent.SetEyeState(MoxieEyeComponent.EyeStates.Closed);
                if (_sleepingTimer.Done)
                    Moxie.SetSleep(Moxie.Sleep + 1);
                if (_healthRestoreTimer.Done)
                    Moxie.SetHealth(Moxie.Health + 1);
            }

            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            ActionSleepComponentSave save = new ActionSleepComponentSave();
            save.Serialize(this, ID);
            return save;
        }

    }

    public class ActionSleepComponentSave : SaveObject
    {
        ActionSleepComponent component = new ActionSleepComponent(null);

        public override void Serialize(object _toSerialize, int ID)
        {
            ActionSleepComponent component = (ActionSleepComponent)_toSerialize;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            ActionSleepComponentSave sSave = (ActionSleepComponentSave)save;
            return component;
        }

    }


}
