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


namespace Moxies5.Components.MoxieComponents
{
    public class MoxieBodyComponent: DrawableComponent, ISerialize
    {
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public MoxieBodyComponent(Entity parentEntity, Cameras cameraType)
            : base(parentEntity, "Moxie/moxieBlank", cameraType)
        {
            Name = "MoxieBodyComponent";
            UpdateOrder = 4;
            SetScale(0.1f);
            SetLayerDepth(Layers.Moxie_Body);
        }

        public new SaveObject Serialize(int ID)
        {
            MoxieBodyComponentSave save = new MoxieBodyComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }
    public class MoxieBodyComponentSave : DrawableComponentSave
    {
        public override void Serialize(object _toSerialize, int ID)
        {
            MoxieBodyComponent toSerialize = (MoxieBodyComponent)_toSerialize;

            base.Serialize(_toSerialize, ID);
        }

        public override object Deserialize(SaveObject _toDeserialize)
        {
            MoxieBodyComponentSave toDeserialize = (MoxieBodyComponentSave)_toDeserialize;
            MoxieBodyComponent component = new MoxieBodyComponent(null, Cameras.Dynamic);
            component.SetColor(new Color(toDeserialize.ColorR, toDeserialize.ColorG, toDeserialize.ColorB));
            return component;
        }
    }

}
