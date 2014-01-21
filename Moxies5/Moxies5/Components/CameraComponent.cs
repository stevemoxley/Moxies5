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
using Moxies5.Controllers;

namespace Moxies5.Components
{
    /// <summary>
    /// This will give the camera control to the component's parent entity
    /// This component requires the parent to have a spatial component
    /// </summary>
    public class CameraComponent : Component
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
        ///
        public CameraComponent(Entity parentEntity)
            : base(parentEntity)
        {
            Name = "CameraComponent";
            UpdateOrder = 3;

            //Check to make sure nothing else has the camera's attention
            //Only one this is allowed to be the camera's focus at a time
            for (int i = 0; i < EntityManager.EntityCount; i++)
            {
                if(EntityManager.EntityMasterList[i].HasComponent(typeof(CameraComponent)))
                {
                    throw new Exception("Only one entity can have the camera component at a time");
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Parent.HasComponent(typeof(SpatialComponent)))
            {
                SpatialComponent sc = (SpatialComponent)Parent.GetComponent(typeof(SpatialComponent));
                MainController.Camera.Pos = sc.Position;
            }
            else
            {
                throw new Exception("Parent must have a spatial component");
            }
            base.Update(gameTime);
        }
        
    }
}
