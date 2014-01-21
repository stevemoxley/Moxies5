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
using Moxies5.Utilities;
using Moxies5.Entities;
using Moxies5.Controllers;

namespace Moxies5.Components
{
    public class ControllerComponent: Component
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        ///
        public ControllerComponent(Entity parentEntity)
            : base(parentEntity)
        {
            Name = "ControllerComponent";
            UpdateOrder = 1;
        }

        public override void Update(GameTime gameTime)
        {
            if(Parent.HasComponent(typeof(MovementComponent)))
            {
                MovementComponent mc = (MovementComponent)Parent.GetComponent(typeof(MovementComponent));
                Vector2 newVector = Vector2.Zero;
                if (InputHandler.KeyDown(Keys.W))
                {
                    newVector.Y = -1;
                }
                if (InputHandler.KeyDown(Keys.S))
                {
                    newVector.Y = 1;
                }
                if (InputHandler.KeyDown(Keys.A))
                {
                    newVector.X = -1;
                }
                if (InputHandler.KeyDown(Keys.D))
                {
                    newVector.X = 1;
                }

                if(newVector != Vector2.Zero)
                    mc.SetMovementVector(newVector);
            }
        }
        
    }
}
