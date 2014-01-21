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
using Moxies5.Controllers;

namespace Moxies5.Components.MoxieComponents.Actions
{
    public class ActionLayEggComponent: AbstractActionComponent
    {
        
        #region Fields
        MoxieEntity _mate;
        #endregion

        #region Properties

        #endregion

        #region Getters and Setters

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public ActionLayEggComponent(Entity parentEntity, MoxieEntity mate)
            : base(parentEntity)
        {
            UpdateOrder = 1;
            _mate = mate;
            Name = "ActionLayEggComponent";
        }

        public override void Update(GameTime gameTime)
        {
            //Create the egg entity
            //Determine its genetics
            Entity eggEntity = new Entity();
            EntityManager.AddEntity(eggEntity);

            //Spatial Component
            SpatialComponent sc = new SpatialComponent(eggEntity);
            eggEntity.AddInitialComponent(sc);

            //Physics Component
            PhysicsComponent pc = new PhysicsComponent(eggEntity);
            pc.CreateCircleBody(10, 1);
            eggEntity.AddInitialComponent(pc);

            MoxieGeneticsComponent genetics = new MoxieGeneticsComponent(eggEntity);
            eggEntity.AddInitialComponent(genetics);

            MoxieEntity moxie = (MoxieEntity)Parent;
            if (moxie.Genetics.Gender == GenderTrait.Male && _mate.Genetics.Gender == GenderTrait.Female)
            {
                //Father is moxie, mother is mate
                genetics.SetGeneticsFromParents(moxie.Genetics, _mate.Genetics);

                SpatialComponent motherSC = (SpatialComponent)_mate.GetComponent(typeof(SpatialComponent));
                sc.SetPosition(motherSC.Position);
            }
            else if (moxie.Genetics.Gender == GenderTrait.Female && _mate.Genetics.Gender == GenderTrait.Male)
            {
                //Mother is moxie, father is mate
                genetics.SetGeneticsFromParents(_mate.Genetics, moxie.Genetics);

                SpatialComponent motherSC = (SpatialComponent)moxie.GetComponent(typeof(SpatialComponent));
                sc.SetPosition(motherSC.Position);
            }

            //Egg component
            EggComponent eggComponent = new EggComponent(eggEntity, genetics);
            eggEntity.AddInitialComponent(eggComponent);

            //drawable component
            DrawableComponent dc = new DrawableComponent(eggEntity, "Moxie/egg", Cameras.Dynamic);
            dc.SetScale(0.05f);
            eggEntity.AddInitialComponent(dc);
            dc.SetLayerDepth(Layers.Moxie_Body);

            UIController.ChangeMoxieCount(1);

            Finish();

            base.Update(gameTime);
        }

        


    }


}
