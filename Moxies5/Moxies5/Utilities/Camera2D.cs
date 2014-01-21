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
using Moxies5.Components;

namespace Moxies5.Utilities
{
    public class Camera2D
    {

        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation
        private Vector2 _LockPos;
        private Vector2 MinMaxZoom = new Vector2(0.25f, 2.0f);
        public DrawableGameComponent Follower;


        public Camera2D()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom < MinMaxZoom.X)
                    _zoom = MinMaxZoom.X;
                else if (_zoom > MinMaxZoom.Y)
                    _zoom = MinMaxZoom.Y;
            } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }
        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public void Rotate(float amount)
        {
            Rotation += amount;
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return _transform;
        }

        /// <summary>
        /// Gets the mouse position from the dynamic camera view.
        /// Use inputhandler mouse state to get the static mouse position
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        public Vector2 get_mouse_pos(GraphicsDevice graphicsDevice)
        {
            float MouseWorldX = (Mouse.GetState().X - graphicsDevice.Viewport.Width * 0.5f + (Pos.X) * (float)Math.Pow(Zoom, 1)) /
            (float)Math.Pow(Zoom, 1);

            float MouseWorldY = ((Mouse.GetState().Y - graphicsDevice.Viewport.Height * 0.5f + (Pos.Y) * (float)Math.Pow(Zoom, 1))) /
            (float)Math.Pow(Zoom, 1);

            return new Vector2(MouseWorldX, MouseWorldY);
        }

        public Vector2 get_world_position(GraphicsDevice graphicsDevice)
        {
            int MouseWorldX = (int)(Pos.X - (graphicsDevice.Viewport.Width * 0.5f) * (1 / Zoom));

            int MouseWorldY = (int)(Pos.Y - (graphicsDevice.Viewport.Height * 0.5f) * (1 / Zoom));

            return new Vector2(MouseWorldX, MouseWorldY);
        }

        public void setPositionFromWorldPosition(Vector2 worldPosition, GraphicsDevice graphicsDevice)
        {
            int MouseWorldX = (int)(Pos.X + (graphicsDevice.Viewport.Width * 0.5f) * Zoom);

            int MouseWorldY = (int)(Pos.Y + (graphicsDevice.Viewport.Height * 0.5f) * Zoom);

            _pos = new Vector2(MouseWorldX, MouseWorldY);
        }

        //This will return a matrix that can be fed into the farseer debugxnaview to line them up properly
        public Matrix ToFarseerProjection(int GraphicsDeviceWidth, int GraphicsDeviceHeight)
        {
            Matrix proj = Matrix.CreateOrthographic(GraphicsDeviceWidth / Zoom / 100.0f, -GraphicsDeviceHeight / Zoom / 100.0f, 0, 1000000);
            return proj;
        }

        public Matrix ToFarseerView()
        {
            Vector3 campos = new Vector3();
            campos.X = -(Pos.X / 100.0f);
            campos.Y = -Pos.Y / 100.0f;
            campos.Z = 0;
            Matrix tran = Matrix.Identity;
            tran.Translation = campos;
            Matrix view = tran;
            return view;
        }


        //Lock the camera in position
        public void CameraLock(bool locked)
        {
            _LockPos = _pos;
        }

        public void ChangeFocusEntity(Entity newFocus)
        {
            for (int i = 0; i < EntityManager.EntityCount; i++)
            {
                if (EntityManager.EntityMasterList[i].HasComponent(typeof(CameraComponent)))
                {
                    EntityManager.EntityMasterList[i].RemoveComponent(typeof(CameraComponent));
                }
            }

            newFocus.AddComponent(new CameraComponent(newFocus));
        }

    }
}
