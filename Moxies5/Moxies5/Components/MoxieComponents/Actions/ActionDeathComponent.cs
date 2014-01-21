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
    public class ActionDeathComponent: AbstractActionComponent, ISerialize
    {
        
        #region Fields
        Timer _decayTimer;
        #endregion

        #region Properties

        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionDeathComponent(Entity parentEntity)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "ActionDeathComponent";

            _decayTimer = new Timer(15);

            MoxieMouthComponent mouth = (MoxieMouthComponent)Moxie.GetComponent(typeof(MoxieMouthComponent));
            MoxieEyeComponent eye = (MoxieEyeComponent)Moxie.GetComponent(typeof(MoxieEyeComponent));
            mouth.SetMouthState(MoxieMouthComponent.MouthStates.Dead);
            eye.SetEyeState(MoxieEyeComponent.EyeStates.Dead);
        }

        public override void Update(GameTime gameTime)
        {
            _decayTimer.Update(gameTime);

            if (_decayTimer.Done)
            {
                UIController.ChangeMoxieCount(-1);
                Moxie.Remove();
                Finish();
            }

            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            ActionDeathComponentSave save = new ActionDeathComponentSave();
            save.Serialize(this, ID);
            return save;
        }

    }

    public class ActionDeathComponentSave : SaveObject
    {
        ActionDeathComponent component = new ActionDeathComponent(null);

        public override void Serialize(object _toSerialize, int ID)
        {
            ActionDeathComponent component = (ActionDeathComponent)_toSerialize;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            ActionDeathComponentSave sSave = (ActionDeathComponentSave)save;
            return component;
        }

    }


}
