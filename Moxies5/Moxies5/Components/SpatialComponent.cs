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
    public class SpatialComponent: Component, ISerialize
    {
        
        #region Fields
        private Vector2 _position = Vector2.Zero;
        private float _rotation = 0;
        private int _width = 0;
        private int _height = 0;
        #endregion

        #region Properties

        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        public float X
        {
            get
            {
                return _position.X;
            }
            set
            {
                SetPositionX(value);
            }
        }

        public float Y
        {
            get
            {
                return _position.Y;
            }
            set
            {
                SetPositionY(value);
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public float Rotation
        {
            get
            {
                return _rotation;
            }
        }

        #endregion

        #region Getters and Setters

        public Vector2 GetPosition()
        {
            return _position;
        }

        public void SetPosition(Vector2 newPosition)
        {
            _position = newPosition;
            SetPhysicsBodyLocation();
        }

        public void SetPositionX(float positionX)
        {
            _position.X = positionX;
        }

        public void SetPositionY(float positionY)
        {
            _position.Y = positionY;
        }

        public float GetRotation()
        {
            return _rotation;
        }

        public void SetRotation(float newRotation)
        {
            _rotation = newRotation;
        }

        public int GetWidth()
        {
            return _width;
        }

        public void SetWidth(int width)
        {
            this._width = width;
        }

        public void SetHeight(int height)
        {
            this._height = height;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public SpatialComponent(Entity parentEntity) : base(parentEntity)
        {
            UpdateOrder = 1;
            
            Name = "SpatialComponent";
        }

        public override void Update(GameTime gameTime)
        {
            if (Parent.HasComponent(typeof(PhysicsComponent)) && !Parent.HasComponent(typeof(MovementComponent)))
            {
                PhysicsComponent pc = (PhysicsComponent)Parent.GetComponent(typeof(PhysicsComponent));
                this._rotation = pc.Body.Rotation;
            }
            if (Parent.HasComponent(typeof(MovementComponent)))
            {
                MovementComponent mc = (MovementComponent)Parent.GetComponent(typeof(MovementComponent));
                if (mc.MovementVector != Vector2.Zero)
                {
                    SpatialComponent sc = (SpatialComponent)Parent.GetComponent(typeof(SpatialComponent));
                    Vector2 newVector = sc.Position - (sc.Position + mc.MovementVector);
                    float radians = (float)Math.Atan2(-newVector.X, newVector.Y);
                    _rotation = radians;
                }
            }

            base.Update(gameTime);
        }

        public SaveObject Serialize(int ID)
        {
            SpatialComponentSave save = new SpatialComponentSave();
            save.Serialize(this, ID);
            return save;
        }

        public void SetPhysicsBodyLocation()
        {
            if (Parent != null)
            {
                if (Parent.HasComponent(typeof(PhysicsComponent)))
                {
                    PhysicsComponent pc = (PhysicsComponent)Parent.GetComponent(typeof(PhysicsComponent));
                    Vector2 physicsPosition = new Vector2(ConvertUnits.ToSimUnits(Position.X), ConvertUnits.ToSimUnits(Position.Y));
                    pc.Body.Position = physicsPosition;
                }

                if (Parent.HasComponent(typeof(SensorComponent)))
                {
                    SensorComponent pc = (SensorComponent)Parent.GetComponent(typeof(SensorComponent));
                    Vector2 physicsPosition = new Vector2(ConvertUnits.ToSimUnits(Position.X), ConvertUnits.ToSimUnits(Position.Y));
                    pc.Body.Position = physicsPosition;
                }
            }
        }
    }

    public class SpatialComponentSave : SaveObject
    {
        SpatialComponent spatialComponent = new SpatialComponent(null);
        public float xPosition = -1;
        public float yPosition = -1;

        public override void Serialize(object _spatialComponent, int ID)
        {
            SpatialComponent spatialComponent = (SpatialComponent)_spatialComponent;
            this.xPosition = spatialComponent.Position.X;
            this.yPosition = spatialComponent.Position.Y;
            base.Serialize(_spatialComponent, ID);
        }

        public override object Deserialize(SaveObject save)
        {
            SpatialComponentSave sSave = (SpatialComponentSave)save;
            spatialComponent.SetPosition(new Vector2(sSave.xPosition, sSave.yPosition));          
            return spatialComponent;
        }

    }


}
