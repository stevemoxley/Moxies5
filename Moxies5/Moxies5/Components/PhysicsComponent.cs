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
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Factories;
using FarseerPhysics;
using Moxies5.Controllers;
using Moxies5.Serialization;


namespace Moxies5.Components
{
    public enum PhysicsBodyShapes
    {
        Circle,
        Rectangle
    }


    public class PhysicsComponent : Component, ISerialize
    {
        #region Fields
        private Body _body = null;
        private float _radius;
        private float _density;
        private float _width;
        private float _height;
        private PhysicsBodyShapes _physicsBodyShape;
        #endregion

        #region Properties

        public Body Body
        {
            get
            {
                return _body;
            }

        }

        public float Radius
        {
            get
            {
                return _radius;
            }

            set
            {
                _radius = value;
            }
        }

        public float Density
        {
            get
            {
                return _density;
            }

            set
            {
                _density = value;
            }
        }

        public float Width
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
            }
        }

        public float Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
            }
        }

        public PhysicsBodyShapes PhysicsBodyShape
        {
            get
            {
                return _physicsBodyShape;
            }
            set
            {
                _physicsBodyShape = value;
            }
        }

        public BodyType BodyType
        {
            get
            {
                return _body.BodyType;
            }
        }
        #endregion

        #region Getters and Setters
        public void SetBody(Body body)
        {
            this._body = body;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        ///
        public PhysicsComponent(Entity parentEntity)
            : base(parentEntity)
        {
            Name = "PhysicsComponent";
            UpdateOrder = 1;

            RemoveExistingBody();
        }

        public override void Update(GameTime gameTime)
        {
            if (_body != null)
            {
                if (_body.UserData == null)
                {
                    _body.UserData = Parent;
                }

                if (Parent.HasComponent(typeof(SpatialComponent)))
                {
                    SpatialComponent sc = (SpatialComponent)Parent.GetComponent(typeof(SpatialComponent));
                    Vector2 spatialPosition = new Vector2(ConvertUnits.ToDisplayUnits(Body.Position.X), ConvertUnits.ToDisplayUnits(Body.Position.Y));
                    sc.SetPosition(spatialPosition);
                }
            }
            else
            {
                throw new Exception("Body is needed");
            }

        }

        public void CreateCircleBody(float radius, float density)
        {
            Body body = BodyFactory.CreateCircle(PhysicsController.World, ConvertUnits.ToSimUnits(radius), 1);
            body.BodyType = BodyType.Dynamic;
            body.Restitution = 0.0f;
            body.Friction = 0.5f;
            body.Mass = 25;
            body.AngularDamping = 1000f;
            body.LinearDamping = 2500;
            this.PhysicsBodyShape = PhysicsBodyShapes.Circle;
            this._radius = radius;
            this._density = density;
            this.SetBody(body);
        }

        public void CreateRectangleBody(float width, float height, float density)
        {
            Body body = BodyFactory.CreateRectangle(PhysicsController.World, width, height, density);
            body.BodyType = BodyType.Dynamic;
            body.Restitution = 0.0f;
            body.Friction = 0.5f;
            body.Mass = 5;
            this._width = width;
            this._height = height;
            this._density = density;
            this.PhysicsBodyShape = PhysicsBodyShapes.Rectangle;
            this.SetBody(body);
        }

        private void RemoveExistingBody()
        {
            if (Parent != null)
            {
                if(Parent.HasComponent(typeof(PhysicsComponent)))
                {
                    PhysicsComponent pc = (PhysicsComponent)Parent.GetComponent(typeof(PhysicsComponent));
                    PhysicsController.World.RemoveBody(pc.Body);
                }
            }
        }

        SaveObject ISerialize.Serialize(int ID)
        {
            PhysicsComponentSave save = new PhysicsComponentSave();
            save.Serialize(this, ID);
            return save;
        }

        /// <summary>
        /// Returns a list of the Bodies in contact with this Physics components body
        /// </summary>
        /// <returns>A List of Bodies</returns>
        public List<Body> GetBodiesInContactWithBody()
        {
            var bodiesInContact = new List<Body>();
            var c = _body.ContactList;
            while (c != null && c.Next != null)
            {
                if (c.Contact.IsTouching())
                {
                    bodiesInContact.Add(c.Other);
                    // if the above doesn't work ( haven't tried this code) try the below:
                    // if (c.Contact.FixtureA.Body == body)
                    //     bodiesInContact.Add(c.Contact.FixtureB.Body);// FixtureA is the body we're getting contacts for, so add fixtureb
                    // else 
                    //     bodiesInContact.Add(c.Contact.FixtureA.Body);

                }
                c = c.Next;
            }
            return bodiesInContact;
        }
    }

    public class PhysicsComponentSave : SaveObject
    {
        public float _radius;
        public float _density;
        public float _width;
        public float _height;
        public PhysicsBodyShapes _physicsBodyShape;
        public Vector2 _physicsBodyLocation;
        public BodyType _bodyType;

        public override void Serialize(object _physicsComponent, int ID)
        {
            PhysicsComponent physicsComponent = (PhysicsComponent)_physicsComponent;
            this._radius = physicsComponent.Radius;
            this._density = physicsComponent.Density;
            this._height = physicsComponent.Height;
            this._width = physicsComponent.Width;
            this._physicsBodyShape = physicsComponent.PhysicsBodyShape;
            this._physicsBodyLocation = physicsComponent.Body.Position;
            this._bodyType = physicsComponent.BodyType;
            base.Serialize(_physicsComponent, ID);
        }
        
        public override object Deserialize(SaveObject toDeserialize)
        {
            PhysicsComponentSave pcSave = (PhysicsComponentSave)toDeserialize;
            PhysicsComponent physicsComponent = new PhysicsComponent(null);
            if (pcSave._physicsBodyShape == PhysicsBodyShapes.Circle)
            {
                physicsComponent.CreateCircleBody(pcSave._radius, pcSave._density);
            }
            else if (pcSave._physicsBodyShape == PhysicsBodyShapes.Rectangle)
            {
                physicsComponent.CreateRectangleBody(pcSave._width, pcSave._height, pcSave._density);
            }
            physicsComponent.Body.Position = pcSave._physicsBodyLocation;
            physicsComponent.Body.BodyType = pcSave._bodyType;
            return physicsComponent;
        }
    }
}
