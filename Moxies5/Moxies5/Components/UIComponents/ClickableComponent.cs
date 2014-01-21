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
using Moxies5.Utilities;
using Moxies5.Serialization;

namespace Moxies5.Components
{
    public enum ClickTypes
    {
        LeftSingle,
        LeftDouble,
        RightSingle,
        RightDouble
    }

    public class ClickableComponent: Component, ISerialize
    {
 
        #region Fields
        private Cameras _camera;
        private SpriteBatch _spriteBatch;
        private Rectangle _clickableArea;
        private ClickTypes _clickType;

        public delegate void ClickHandler(EventArgs e);
        public event ClickHandler OnClick;
       
        #endregion

        #region Properties
        public Cameras Camera
        {
            get
            {
                return _camera;
            }
        }

        public Rectangle ClickableArea
        {
            get
            {
                return _clickableArea;
            }
        }

        public ClickTypes ClickType
        {
            get
            {
                return _clickType;
            }
        }
        #endregion

        #region Getters and Setters
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ClickableComponent(Entity parentEntity, Cameras cameraType, Rectangle clickableArea, ClickTypes clickType)
            : base(parentEntity)
        {
            Name = "ClickableComponent";
            UpdateOrder = 4;
            _camera = cameraType;
            _clickableArea = clickableArea;
            _clickType = clickType;
            try
            {
                switch (cameraType)
                {
                    case Cameras.Dynamic:
                        {
                            _spriteBatch = MainController.CameraSpriteBatch;
                            break;
                        }
                    case Cameras.Static:
                        {
                            _spriteBatch = MainController.StaticSpriteBatch;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        #endregion

        #region Methods

        public new void Start()
        {
        }

        public override void Update(GameTime gameTime)
        {
            #region Check if Clicked
            bool clicked = false;
            switch (_clickType)
            {
                case ClickTypes.LeftSingle:
                    {
                        if (InputHandler.LeftMouseClick())
                            clicked = true;
                        break;
                    }
                case ClickTypes.LeftDouble:
                    {
                        throw new NotImplementedException();
                    }
                case ClickTypes.RightSingle:
                    {
                        if (InputHandler.RightMouseClick())
                            clicked = true;
                        break;
                    }
                case ClickTypes.RightDouble:
                    {
                        throw new NotImplementedException();
                    }
            }
            #endregion

            #region Invoke OnClick Event if clicked and in clickable Area
            if (clicked)
            {
                switch (_camera)
                {
                    case Cameras.Static:
                        {
                            Vector2 mousePosition = new Vector2(InputHandler.MouseState.X, InputHandler.MouseState.Y);
                            if (_clickableArea.Contains((int)mousePosition.X, (int)mousePosition.Y))
                            {
                                OnClick(null);
                            }
                            break;
                        }
                    case Cameras.Dynamic:
                        {
                            Vector2 mousePosition = MainController.Camera.get_mouse_pos(MainController.GraphicsDevice);
                            if (_clickableArea.Contains((int)mousePosition.X, (int)mousePosition.Y))
                            {
                                OnClick(null);
                            }
                            break;
                        }
                }
            }
            #endregion

            base.Update(gameTime);
        }


        public SaveObject Serialize(int ID)
        {
            ClickableComponentSave save = new ClickableComponentSave();
            save.Serialize(this, ID);
            return save;
        }

        #endregion
    }

    public class ClickableComponentSave : SaveObject
    {

        public Cameras Camera;
        public Rectangle ClickableArea;
        public ClickTypes ClickType;

        public override void Serialize(object _toSerialize, int ID)
        {
            ClickableComponent component = (ClickableComponent)_toSerialize;
            this.Camera = component.Camera;
            this.ClickableArea = component.ClickableArea;
            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject toDeserialize)
        {
            ClickableComponentSave save = (ClickableComponentSave)toDeserialize;
            ClickableComponent component = new ClickableComponent(null, save.Camera, save.ClickableArea, save.ClickType);
            return component;
        }
    }
}
