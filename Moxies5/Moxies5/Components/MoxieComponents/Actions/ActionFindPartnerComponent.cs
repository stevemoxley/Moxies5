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
    public class ActionFindPartnerComponent: AbstractActionComponent, ISerialize
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
        public ActionFindPartnerComponent(Entity parentEntity)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "ActionFindPartnerComponent";

        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            ActionSleepComponentSave save = new ActionSleepComponentSave();
            save.Serialize(this, ID);
            return save;
        }

    }

    public class ActionFindPartnerComponentSave : SaveObject
    {
        ActionFindPartnerComponent component = new ActionFindPartnerComponent(null);

        public override void Serialize(object _toSerialize, int ID)
        {
            ActionFindPartnerComponent component = (ActionFindPartnerComponent)_toSerialize;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            ActionFindPartnerComponentSave sSave = (ActionFindPartnerComponentSave)save;
            return component;
        }

    }


}
