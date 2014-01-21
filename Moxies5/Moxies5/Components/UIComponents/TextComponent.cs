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

namespace Moxies5.Components
{
    public enum Alignment
    { None = 0x0, Center = 0x1, Left = 0x2, Right = 0x4, Top = 0x8, Bottom = 0x01 }

    public class TextComponent: Component, IDrawableComponent
    {
 
        #region Fields

        private int _drawOrder = -1;

        private bool _visible = true;

        private float _rotation = 0;

        private float _layerDepth = 0.9f;

        private Color _color = Color.White;

        private Vector2 _origin = Vector2.Zero;

        private float _scale = 1;

        private SpriteBatch _spriteBatch = null;

        private Cameras _camera = Cameras.Dynamic;

        private SpriteFont _font;

        private string _text;

        private Alignment _alignment;

        #endregion

        #region Properties
        public bool Visible
        {
            get { return _visible; }
        }

        public int DrawOrder
        {
            get { return _drawOrder; }
        }

        public Cameras Camera
        {
            get { return _camera; }
        }

        bool IDrawableComponent.Visible
        {
            get { return _visible; }
        }

        public SpriteFont Font
        {
            get
            {
                return _font;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }
        }

        public Alignment Alignment
        {
            get
            {
                return _alignment;
            }
            set
            {
                _alignment = value;
            }
        }
        #endregion

        #region Getters and Setters
        public void SetText(string text)
        {
            this._text = text;
        }

        public void SetColor(Color color)
        {
            this._color = color;
        }

        public void SetLayerDepth(Layers layer)
        {
            this._layerDepth = (float)((float)layer / 1000);
        }
        public void SetVisible(bool value)
        {
            this._visible = value;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public TextComponent(Entity parentEntity, string text, Cameras cameraType)
            : base(parentEntity)
        {
            this._text = text;
            Name = "TextComponent";
            UpdateOrder = 4;
            _camera = cameraType;
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
                _origin = Vector2.Zero;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            _font = MainController.Game.Content.Load<SpriteFont>("font");
        }

        #endregion

        #region Methods

        public new void Start()
        {
            if (_drawOrder == -1)
            {
                throw new Exception("Draw Order must be set");
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (_visible)
            {
                if (Parent.HasComponent("SpatialComponent"))
                {
                    if (_alignment == Alignment.Right)
                    {
                        Vector2 textLength = _font.MeasureString(_text);
                        _origin.X = textLength.X * 0.5f;
                        _origin.X += textLength.X / 2;
                    }
                    else if (_alignment == Alignment.Left)
                    {
                        
                    }
                    SpatialComponent spatialComponent = (SpatialComponent)Parent.GetComponent("SpatialComponent");
                    _spriteBatch.DrawString(_font, _text, spatialComponent.Position, _color, _rotation, _origin, _scale, SpriteEffects.None, _layerDepth);
                }
                else
                {
                    throw new Exception("Entities with a drawable component also need a spatial component");
                }
            }
        }

        #endregion
    }
}
