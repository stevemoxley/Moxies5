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
    public class ActionSpinComponent : AbstractActionComponent
    {
        
        #region Fields
        Timer timeToMoveAround;
        SpatialComponent _spatialComponent;
        float _rotationAmount = -1;
        #endregion

        #region Properties

        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionSpinComponent(Entity parentEntity)
            : base(parentEntity)
        {
            UpdateOrder = 2;

            Name = "ActionSpinComponent";

            timeToMoveAround = new Timer(MainController.Random.Next(0, 3)); //Pick a random amount of time

            if (Moxie.HasComponent(typeof(SpatialComponent)))
            {
                this._spatialComponent = (SpatialComponent)Moxie.GetComponent(typeof(SpatialComponent));
            }
            int rotationRand = MainController.Random.Next(-1000, 1000);
            _rotationAmount = rotationRand / 1000;
        }

        public override void Update(GameTime gameTime)
        {
            //Finish the action and set the moxies movement vector to zero
            timeToMoveAround.Update(gameTime);

            if (timeToMoveAround.Done)
            {
                Finish();
            }
            else
            {
                //Rotate in a circle
                _spatialComponent.SetRotation(_spatialComponent.Rotation + _rotationAmount);
            }

            base.Update(gameTime);
        }

    }

}
