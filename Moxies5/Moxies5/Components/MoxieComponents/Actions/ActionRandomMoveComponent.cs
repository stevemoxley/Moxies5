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
    public class ActionRandomMoveComponent : AbstractActionComponent, ISerialize
    {
        
        #region Fields
        Timer timeToMoveAround;
        private int randomX;
        private int randomY;
        #endregion

        #region Properties

        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionRandomMoveComponent(Entity parentEntity)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "ActionRandomMoveAction";

            timeToMoveAround = new Timer(MainController.Random.Next(0, 11)); //Pick a random amount of time

            SetMovement();

        }

        private void SetMovement()
        {
            if (Moxie != null)
            {
                if (Moxie.HasComponent(typeof(MovementComponent)))
                {
                    MovementComponent mc = (MovementComponent)Moxie.GetComponent(typeof(MovementComponent));
                    randomX = MainController.Random.Next(-1, 2); //Get random x movement
                    randomY = MainController.Random.Next(-1, 2); //Get random y movement
                    mc.SetMovementVector(new Vector2(randomX, randomY)); //Set the movement Vector of the Moxie
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Finish the action and set the moxies movement vector to zero
            timeToMoveAround.Update(gameTime);

            if (Moxie != null)
            {
                if (timeToMoveAround.Done)
                {
                    Finish();
                    if (Moxie.HasComponent(typeof(MovementComponent)))
                    {
                        MovementComponent mc = (MovementComponent)Moxie.GetComponent(typeof(MovementComponent));
                        mc.SetMovementVector(new Vector2(0, 0)); //Set the movement Vector of the Moxie
                    }
                }
                else
                {
                    MovementComponent mc = (MovementComponent)Moxie.GetComponent(typeof(MovementComponent));
                    mc.SetMovementVector(new Vector2(randomX, randomY)); //Set the movement Vector of the Moxie
                }
            }

            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            ActionRandomMoveActionSave save = new ActionRandomMoveActionSave();
            save.Serialize(this, ID);
            return save;
        }

    }

    public class ActionRandomMoveActionSave : SaveObject
    {
        ActionRandomMoveComponent component = new ActionRandomMoveComponent(null);

        public override void Serialize(object _toSerialize, int ID)
        {
            ActionRandomMoveComponent component = (ActionRandomMoveComponent)_toSerialize;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            ActionRandomMoveActionSave sSave = (ActionRandomMoveActionSave)save;
            return component;
        }

    }


}
