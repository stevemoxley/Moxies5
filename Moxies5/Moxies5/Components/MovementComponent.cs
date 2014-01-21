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

namespace Moxies5.Components
{
    public class MovementComponent: Component, ISerialize
    {

        #region Fields
        private Vector2 _movementVector;
        private float _speed = 1;
        private bool _movementVectorJustSet = false;
        private float _normalSpeed = 1;
        #endregion

        #region Properties

        public Vector2 MovementVector
        {
            get
            {
                return _movementVector;
            }
        }

        public float Speed
        {
            get
            {
                return _speed;
            }
        }

        public float NormalSpeed
        {
            get
            {
                return _normalSpeed;
            }
        }

        public bool MovementVectorJustSet
        {
            get
            {
                return _movementVectorJustSet;
            }
            set
            {
                _movementVectorJustSet = value;
            }
        }
        #endregion

        #region Getters and Setters

        public Vector2 GetMovementVector()
        {
            return _movementVector;
        }

        public void SetMovementVector(Vector2 newMovementVector)
        {
            _movementVector = newMovementVector;
            _movementVectorJustSet = true;
        }

        public float GetSpeed()
        {
            return _speed;
        }

        public void SetSpeed(float newSpeed)
        {
            _speed = newSpeed;
        }

        public void SetNormalSpeed(float speed)
        {
            this._normalSpeed = speed;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        ///
        public MovementComponent(Entity parentEntity)
            : base(parentEntity)
        {
            Name = "MovementComponent";
            UpdateOrder = 3;
        }

        public override void Update(GameTime gameTime)
        {
            if (_movementVector != Vector2.Zero)
            {
                if(Parent.HasComponent(typeof(PhysicsComponent)))
                {
                    PhysicsComponent pc = (PhysicsComponent)Parent.GetComponent(typeof(PhysicsComponent));

                    if (_movementVectorJustSet)
                    {
                        _movementVector *= _speed;
                        _movementVector = ConvertUnits.ToDisplayUnits(_movementVector);
                        pc.Body.ApplyLinearImpulse(_movementVector);
                        _movementVectorJustSet = false;
                    }
                }
            }

           base.Update(gameTime);
        }



        public SaveObject Serialize(int ID)
        {
            MovementComponentSave save = new MovementComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class MovementComponentSave : SaveObject
    {

        public Vector2 MovementVector;
        public float Speed;
        public float NormalSpeed;
        public bool MovementVectorJustSet;

        public override void Serialize(object _toSerialize, int ID)
        {
            MovementComponent component = (MovementComponent)_toSerialize;
            this.MovementVector = component.MovementVector;
            this.Speed = component.Speed;
            this.NormalSpeed = component.NormalSpeed;
            this.MovementVectorJustSet = component.MovementVectorJustSet;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            MovementComponentSave sSave = (MovementComponentSave)save;
            MovementComponent component = new MovementComponent(null);
            component.SetSpeed(sSave.Speed);
            component.MovementVectorJustSet = sSave.MovementVectorJustSet;
            if(sSave.MovementVectorJustSet)
                component.SetMovementVector(sSave.MovementVector);
            component.SetNormalSpeed(sSave.NormalSpeed);
            return component;
        }

    }
}
