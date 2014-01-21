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
    public class ThoughtBubbleComponent: DrawableComponent, ISerialize
    {
        private MoxieEntity _moxie;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ThoughtBubbleComponent(Entity parentEntity)
            : base(parentEntity, "UI/ThoughtBubbles/thoughtBubble", Cameras.Dynamic)
        {
            Name = "ThoughtBubbleComponent";
            UpdateOrder = 4;
            SetScale(0.1f);
            SetLayerDepth(Layers.UI_ThoughtBubble);
            if (parentEntity != null)
            {
                if (parentEntity.GetType() == typeof(MoxieEntity))
                {
                    _moxie = (MoxieEntity)parentEntity;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (UIController.ThoughtBubblesVisible)
            {
                if (_moxie.ThoughtProcess.Action != null)
                {
                    if (_moxie.ThoughtProcess.Action.ThoughtBubbleTexture != "")
                    {
                        Vector2 offset = new Vector2(30, -30);
                        SpatialComponent rotation = (SpatialComponent)Parent.GetComponent(typeof(SpatialComponent));
                        SetIndependentRotation(0);
                        SetOffset(offset);
                        SetVisible(true);
                    }
                    else
                        SetVisible(false);
                }
                else
                    SetVisible(false);
            }
            else
                SetVisible(false);
        }

        public new SaveObject Serialize(int ID)
        {
            ThoughtBubbleComponentSave save = new ThoughtBubbleComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class ThoughtBubbleComponentSave : DrawableComponentSave
    {
        public override void Serialize(object _drawableComponent, int ID)
        {
            ThoughtBubbleComponent dc = (ThoughtBubbleComponent)_drawableComponent;

            base.Serialize(_drawableComponent, ID);
        }

        public override object Deserialize(SaveObject toDeserialize)
        {
            ThoughtBubbleComponentSave tBubSave = (ThoughtBubbleComponentSave)toDeserialize;
            ThoughtBubbleComponent tBub = new ThoughtBubbleComponent(null);
            return tBub;
        }
    }
}
