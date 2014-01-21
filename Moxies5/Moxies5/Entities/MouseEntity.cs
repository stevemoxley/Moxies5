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
using Moxies5.Components;
using Moxies5.Serialization;
using Moxies5.Utilities;
using Moxies5.Controllers;
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;

namespace Moxies5.Entities
{
    public class MouseEntity : Entity
    {
        PhysicsComponent pc;

        public Body SensorBody
        {
            get
            {
                return pc.Body;
            }
        }

        public MouseEntity()
        {
            pc = new PhysicsComponent(this);
            pc.CreateCircleBody(1, 1);
            pc.Body.IsSensor = true;
            pc.Body.OnCollision += new FarseerPhysics.Dynamics.OnCollisionEventHandler(Body_OnCollision);
       }

        bool Body_OnCollision(FarseerPhysics.Dynamics.Fixture fixtureA, FarseerPhysics.Dynamics.Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureB.Body.UserData != null)
            {
                if (fixtureB.Body.UserData.GetType() == typeof(MoxieEntity))
                {
                    if (InputHandler.LeftMouseClick())
                    {
                        MoxieEntity moxieEntity = (MoxieEntity)fixtureB.Body.UserData;
                        UIController.SetTargetMoxie(moxieEntity);
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 cameraPos = MainController.Camera.get_mouse_pos(MainController.GraphicsDevice);
            pc.Body.Position = new Vector2(ConvertUnits.ToSimUnits(cameraPos.X), ConvertUnits.ToSimUnits(cameraPos.Y));

            base.Update(gameTime);
        }

    }


}
