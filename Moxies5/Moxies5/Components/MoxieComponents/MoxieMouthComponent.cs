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
    public class MoxieMouthComponent: DrawableComponent,ISerialize
    {
        public enum MouthStates
        {
            Open,
            Smile,
            Dead,
        }

        private MouthStates _mouthState = MouthStates.Smile;

        public void SetMouthState(MouthStates state)
        {
            this._mouthState = state;

            switch (state)
            {
                case MouthStates.Dead:
                    {
                        SetTexturePath("Moxie/moxieMouthDead");
                        break;
                    }
                case MouthStates.Smile:
                    {
                        SetTexturePath("Moxie/moxieMouthSmile");
                        break;
                    }
                case MouthStates.Open:
                    {
                        break;
                    }
            }
        }

        public MouthStates MouthState
        {
            get
            {
                return _mouthState;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public MoxieMouthComponent(Entity parentEntity, Cameras cameraType)
            : base(parentEntity, "Moxie/moxieMouthSmile", cameraType)
        {
            Name = "MoxieMouthComponent";
            UpdateOrder = 4;
            SetScale(0.1f);
            SetLayerDepth(Layers.Moxie_Mouth);
        }

        public new SaveObject Serialize(int ID)
        {
            MoxieMouthComponentSave save = new MoxieMouthComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class MoxieMouthComponentSave : DrawableComponentSave
    {
        public MoxieMouthComponent.MouthStates MouthState;

        public override void Serialize(object _toSerialize, int ID)
        {
            MoxieMouthComponent toSerialize = (MoxieMouthComponent)_toSerialize;
            this.MouthState = toSerialize.MouthState;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject _toDeserialize)
        {
            MoxieMouthComponentSave toDeserialize = (MoxieMouthComponentSave)_toDeserialize;
            MoxieMouthComponent component = new MoxieMouthComponent(null, Cameras.Dynamic);
            component.SetMouthState(toDeserialize.MouthState);
            return component;
        }
    }
}
