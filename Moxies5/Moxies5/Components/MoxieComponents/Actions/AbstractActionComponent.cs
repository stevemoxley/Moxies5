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
using Moxies5.Components.ItemComponents;

namespace Moxies5.Components.MoxieComponents.Actions
{
    public abstract class AbstractActionComponent: Component
    {
        
        #region Fields
        private MoxieEntity _moxie;
        private bool _actionTextureSet = false;
        #endregion

        #region Properties
        public MoxieEntity Moxie
        {
            get
            {
                return _moxie;
            }
        }

        public ThoughtProcess ThoughtProcess
        {
            get
            {
                return _moxie.ThoughtProcess;
            }
        }

        public String ThoughtBubbleTexture { get; set; }
        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public AbstractActionComponent(Entity parentEntity)
            : base(parentEntity)
        {
            UpdateOrder = 1;

            Name = "AbstractActionComponent";

            ThoughtBubbleTexture = "";

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
            if (_moxie == null)
            {
                _moxie = (MoxieEntity)Parent;
                if (Parent != null && Parent.GetType() == typeof(MoxieEntity))
                {
                    if (_moxie.ThoughtProcess.Action == null)
                    {
                        _moxie.ThoughtProcess.SetAction(this);
                    }
                }
            }
            else
            {
                if (!_actionTextureSet)
                {
                    if (_moxie.ThoughtProcess != null)
                    {
                        if (_moxie.HasComponent(typeof(ThoughtBubbleComponent)))
                        {
                            string path = "UI/ThoughtBubbles/";
                            ThoughtBubbleComponent tbub = (ThoughtBubbleComponent)_moxie.GetComponent(typeof(ThoughtBubbleComponent));
                            if (ThoughtBubbleTexture != null && ThoughtBubbleTexture != string.Empty)
                            {
                                tbub.SetTexturePath(path + ThoughtBubbleTexture);
                                _actionTextureSet = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This will null out the Moxie's current action
        /// </summary>
        public void Finish()
        {
            if (_moxie != null)
            {
                _moxie.RemoveComponent(this.Name);
                _moxie.RunThoughWaitLists();
                _moxie.ThoughtProcess.SetAction(null);
                if (_moxie.HasComponent(typeof(ThoughtBubbleComponent)))
                {
                    ThoughtBubbleComponent tBub = (ThoughtBubbleComponent)_moxie.GetComponent(typeof(ThoughtBubbleComponent));
                    string path = "UI/ThoughtBubbles/thoughtBubble";
                    tBub.SetTexturePath(path);
                    tBub.SetVisible(false);
                }
                
            }
        }



    }



}
