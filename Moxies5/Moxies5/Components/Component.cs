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

namespace Moxies5.Components
{
    public abstract class Component: IEntityComponent, IUpdateableComponent
    {

        #region Fields
        private Entity _parent;
        private int _updateOrder = -1;
        private bool _enabled = true;
        private string _name = String.Empty;
        #endregion

        #region Properties
        public Entity Parent
        {
            get
            {
                return _parent;
            }
        }
        #endregion

        #region Getters and Setters
        public void SetParent(Entity parent)
        {
            this._parent = parent;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public Component(Entity parentEntity)
        {
            this._parent = parentEntity;
        }

        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
        }

        public int UpdateOrder
        {
            get
            {
                return _updateOrder;
            }
            set
            {
                _updateOrder = value;
            }
        }

        #endregion

        /// <summary>
        /// Update the component
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime){ }

        /// <summary>
        /// This is called after Initialize
        /// </summary>
        public virtual void Start()
        {
            if (_updateOrder == -1)
            {
                throw new Exception("Update Order needs to be set");
            }

            if (_name == String.Empty)
            {
                throw new Exception("You must name the component");
            }
        }

        /// <summary>
        /// This is called first. In order of the update order
        /// </summary>
        public virtual void Initialize(){ }

        public Component Copy(Entity newParent)
        {
            Component newComponent = this;
            newComponent._parent = newParent;
            return this;
        }
        
    }
}
