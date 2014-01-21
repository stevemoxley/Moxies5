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


namespace Moxies5.Components.MoxieComponents
{
    public class MoxieOutlineComponent: DrawableComponent
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public MoxieOutlineComponent(Entity parentEntity, Cameras cameraType)
            : base(parentEntity, "moxieHighlight", cameraType)
        {
            Name = "MoxieOutlineComponent";
            UpdateOrder = 4;
            SetScale(0.1f);
        }

    }
}
