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
using Moxies5.Serialization;
using Moxies5.Utilities;


namespace Moxies5.Components
{
    public class DrawableComponent: Component, IDrawableComponent, ISerialize
    {
        #region Fields

        private Texture2D _texture;

        private int _drawOrder = -1;

        private bool _enabled = true;

        private bool _visible = true;

        private float _layerDepth = 0.9f;

        private Color _color = Color.White;

        private Rectangle _sourceRectangle;

        private Vector2 _origin = Vector2.Zero;

        private float _scale = 1;

        private Cameras _camera = Cameras.Dynamic;

        private string _texturePath;

        private SpriteBatch _spriteBatch = null;

        private float _transparency = 1;

        private Vector2 _offset = Vector2.Zero;

        private float _rotation;

        private bool _independentRotationSet = false;

        #endregion

        #region Properties

        public int DrawOrder
        {
            get
            {
                return _drawOrder;
            }
        }

        public new bool Enabled
        {
            get
            {
                return _enabled;
            }
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return _texture;
            }
        }

        public float LayerDepth
        {
            get
            {
                return _layerDepth;
            }
        }

        public Color Color
        {

            get
            {
                return _color;
            }
        }

        public Rectangle SourceRectangle
        {

            get
            {
                return _sourceRectangle;
            }
         }

        public Vector2 Origin
        {
            get
            {
                return _origin;
            }
        }

        public float Scale
        {
            get
            {
                return _scale;
            }
        }

        public Cameras Camera
        {
            get
            {
                return _camera;
            }
        }

        public float Width
        {
            get
            {
                return _texture.Width * _scale;
            }
        }

        public float Height
        {
            get
            {
                return _texture.Height * _scale;
            }
        }

        public SpriteBatch SpriteBatch
        {
            get
            {
                return _spriteBatch;
            }
        }

        public string TexturePath
        {
            get
            {
                return _texturePath;
            }
        }

        public float Transparency
        {
            get
            {
                return _transparency;
            }
        }

        public Vector2 Offset
        {
            get
            {
                return _offset;
            }
        }

        public float IndependentRotation
        {
            get
            {
                return _rotation;
            }
        }

        public bool IndependentRotationSet
        {
            get
            {
                return _independentRotationSet;
            }
        }

        #endregion

        #region Getters and Setters

        public int GetDrawOrder()
        {
            return _drawOrder;
        }

        public void SetDrawOrder(int drawOrder)
        {
            this._drawOrder = drawOrder;
        }

        public float GetScale()
        {
            return _scale;
        }

        public void SetScale(float scale)
        {
            if (scale > 0)
                this._scale = scale;
            else
                throw new ArgumentOutOfRangeException("Scale must be greater than 0");             
        }

        public void SetSourceRectangle(Rectangle rectangle)
        {
            this._sourceRectangle = rectangle;
        }

        public void SetOrigin(Vector2 origin)
        {
            this._origin = origin;
        }

        public void SetLayerDepth(float layerDepth)
        {
            this._layerDepth = layerDepth;
        }

        public void SetLayerDepth(Layers layer)
        {
            this._layerDepth = (float)((float)layer / 1000);
        }

        public void SetEnabled(bool value)
        {
            this._enabled = value;
        }
        public void SetVisible(bool value)
        {
            this._visible = value;
        }
        public void SetColor(Color color)
        {
            this._color = color;
        }
        public void SetTexturePath(string path)
        {
            this._texturePath = path;
            _texture = MainController.Game.Content.Load<Texture2D>(path);
        }

        public void SetTransparency(float value)
        {
            _transparency = value;
        }

        public void SetOffset(Vector2 offset)
        {
            _offset = offset;
        }

        public void SetIndependentRotation(float rotation)
        {
            _independentRotationSet = true;
            _rotation = rotation;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public DrawableComponent(Entity parentEntity, string texturePath, Cameras cameraType): base(parentEntity)
        {
            Name = "DrawableComponent";
            UpdateOrder = 4;
            _camera = cameraType;
            this._texturePath = texturePath;
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
                _texture = MainController.Content.Load<Texture2D>(texturePath);
                _sourceRectangle = new Rectangle(0, 0, _texture.Width, _texture.Height);
                _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (_visible)
            {
                if (Parent.HasComponent("SpatialComponent"))
                {
                    SpatialComponent spatialComponent = (SpatialComponent)Parent.GetComponent("SpatialComponent");

                    float rotation;
                    if (_independentRotationSet)
                        rotation = _rotation;
                    else
                    {
                        rotation = spatialComponent.GetRotation();
                    }

                    _spriteBatch.Draw(_texture, spatialComponent.Position + _offset, _sourceRectangle, _color * _transparency, rotation, _origin, _scale, SpriteEffects.None, _layerDepth);
                }
                else
                {
                    throw new Exception("Entities with a drawable component also need a spatial component");
                }
            }
        }

        public SaveObject Serialize(int ID)
        {
            DrawableComponentSave save = new DrawableComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class DrawableComponentSave : SaveObject
    {
        public string TexturePath;
        public int DrawOrder;
        public bool Enabled;
        public bool Visible;
        public float Rotation;
        public float LayerDepth;
        public int ColorR;
        public int ColorG;
        public int ColorB;
        public float OriginX;
        public float OriginY;
        public float Scale;
        public Cameras Camera;
        public float Transparency;
        public Rectangle SourceRectangle;
        public bool IndependentRotationSet = false;

        public override void Serialize(object _drawableComponent, int ID)
        {
            DrawableComponent dc = (DrawableComponent)_drawableComponent;
            this.TexturePath = dc.TexturePath;
            this.DrawOrder = dc.DrawOrder;
            this.Enabled = dc.Enabled;
            this.Visible = dc.Visible;
            this.LayerDepth = dc.LayerDepth;
            this.ColorR = dc.Color.R;
            this.ColorG = dc.Color.G;
            this.ColorB = dc.Color.B;
            this.OriginX = dc.Origin.X;
            this.OriginY = dc.Origin.Y;
            this.Scale = dc.Scale;
            this.Camera = dc.Camera;
            this.Transparency = dc.Transparency;
            this.SourceRectangle = dc.SourceRectangle;
            this.Rotation = dc.IndependentRotation;
            this.IndependentRotationSet = dc.IndependentRotationSet;
            base.Serialize(_drawableComponent, ID);
        }

        public override object Deserialize(SaveObject toDeserialize)
        {
            DrawableComponentSave dcSave = (DrawableComponentSave)toDeserialize;
            DrawableComponent dc = new DrawableComponent(null, dcSave.TexturePath, dcSave.Camera);
            dc.SetDrawOrder(dcSave.DrawOrder);
            dc.SetEnabled(dcSave.Enabled);
            dc.SetVisible(dcSave.Visible);
            dc.SetLayerDepth(dcSave.LayerDepth);
            dc.SetColor(new Color(dcSave.ColorR, dcSave.ColorG, dcSave.ColorB));
            dc.SetOrigin(new Vector2(dcSave.OriginX, dcSave.OriginY));
            dc.SetScale(dcSave.Scale);
            dc.SetTransparency(dcSave.Transparency);
            dc.SetSourceRectangle(dcSave.SourceRectangle);
            if (dcSave.IndependentRotationSet)
                dc.SetIndependentRotation(dcSave.Rotation);
            return dc;
        }
    }
}
