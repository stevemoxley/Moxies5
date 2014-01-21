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
    

    public class MoxieEyeComponent : DrawableComponent, ISerialize
    {

        public enum EyeStates
        {
            Open,
            Closed,
            Dead
        }

        public EyeStates EyeState { get; set; }

        private Timer _blinkTimer; //Blinky! No need to serialize this one

        public void SetEyeState(EyeStates state)
        {
            EyeState = state;

            switch (state)
            {
                case EyeStates.Closed:
                    {
                        SetTexturePath("Moxie/moxieEyesClosed");
                        break;
                    }
                case EyeStates.Open:
                    {
                        SetTexturePath("Moxie/moxieEyesOpen");
                        break;
                    }
                case EyeStates.Dead:
                    {
                        SetTexturePath("Moxie/moxieEyesDead");
                        break;
                    }
            }
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public MoxieEyeComponent(Entity parentEntity, Cameras cameraType)
            : base(parentEntity, "Moxie/moxieEyesClosed", cameraType)
        {
            Name = "MoxieEyeComponent";
            UpdateOrder = 4;
            SetScale(0.1f);
            SetLayerDepth(Layers.Moxie_Eyes);
            _blinkTimer = new Timer(1);
        }

        public override void Update(GameTime gameTime)
        {
            #region Blink
            _blinkTimer.Update(gameTime);
            if (_blinkTimer.Done)
            {
                float openMaxTime = 3;
                float openMinTime = 0.5f;

                float closedMaxTime = 0.2f;
                float closedMinTime = 0.1f;
                //Switch between open and closed eyes
                if (EyeState == MoxieEyeComponent.EyeStates.Open)
                {
                    SetEyeState(MoxieEyeComponent.EyeStates.Closed);
                    float randomTime = (float)MainController.Random.Next((int)(openMinTime * 100), (int)(openMaxTime * 100) + 1) / 100;
                    _blinkTimer.TimeToCount = randomTime;
                }
                else if (EyeState == MoxieEyeComponent.EyeStates.Closed)
                {
                    SetEyeState(MoxieEyeComponent.EyeStates.Open);
                    float randomTime = (float)MainController.Random.Next((int)(closedMinTime * 100), (int)(closedMaxTime * 100) + 1) / 100;
                    _blinkTimer.TimeToCount = randomTime;
                }
                //Set the timer to some randomness
            }

            #endregion

            base.Update(gameTime);
        }

        public new SaveObject Serialize(int ID)
        {
            MoxieEyeComponentSave save = new MoxieEyeComponentSave();
            save.Serialize(this, ID);
            return save;
        }

    }

    public class MoxieEyeComponentSave : DrawableComponentSave
    {
        public MoxieEyeComponent.EyeStates EyeState;

        public override void Serialize(object _toSerialize, int ID)
        {
            MoxieEyeComponent toSerialize = (MoxieEyeComponent)_toSerialize;
            this.EyeState = toSerialize.EyeState;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject _toDeserialize)
        {
            MoxieEyeComponentSave toDeserialize = (MoxieEyeComponentSave)_toDeserialize;
            MoxieEyeComponent component = new MoxieEyeComponent(null, Cameras.Dynamic);
            component.SetEyeState(toDeserialize.EyeState);
            return component;
        }
    }

}
