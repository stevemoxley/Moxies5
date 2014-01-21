using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Moxies5.Entities;
using Moxies5.Components;
using Moxies5.Components.ItemComponents;
using Moxies5.Components.MoxieComponents;
using Moxies5.Components.MoxieComponents.Actions;


namespace Moxies5.Serialization
{
    [
    XmlInclude(typeof(MoxieEntitySave)),
    XmlInclude(typeof(SpatialComponentSave)),
    XmlInclude(typeof(FoodComponentSave)),
    XmlInclude(typeof(EntitySave)),
    XmlInclude(typeof(DrawableComponentSave)),
    XmlInclude(typeof(TileBlockComponentSave)),
    XmlInclude(typeof(MoxieGeneticsComponentSave)),
    XmlInclude(typeof(MoxieMouthComponentSave)),
    XmlInclude(typeof(MoxieBodyComponentSave)),
    XmlInclude(typeof(MoxieEyeComponentSave)),
    XmlInclude(typeof(ItemCreatingComponentSave)),
    XmlInclude(typeof(RecentlyReproducedComponentSave)),
    XmlInclude(typeof(ActionFindMateComponentSave)),
    XmlInclude(typeof(ActionWaitForMateComponentSave)),
    XmlInclude(typeof(MoneyMakingComponentSave)),
    XmlInclude(typeof(ClickableComponentSave)),
    XmlInclude(typeof(UIItemButtonComponentSave)),
    XmlInclude(typeof(ActionRandomMoveActionSave)),
    XmlInclude(typeof(GameSaveComponentSave)),
    XmlInclude(typeof(ThoughtBubbleComponentSave)),
    XmlInclude(typeof(SensorComponentSave)),
    XmlInclude(typeof(EggComponentSave)),
    XmlInclude(typeof(MovementComponentSave)),
    XmlInclude(typeof(PhysicsComponentSave))
    ]

    public abstract class SaveObject
    {
        public int ID = -1;
        public int ParentEntityID = -1;

        /// <summary>
        /// This will serialize the object
        /// </summary>
        /// <param name="toSerialize">The entity/component to be serialized</param>
        /// <param name="ID">The ID of the object to serialize</param>
        public virtual void Serialize(object toSerialize, int ID)
        {
            this.ID = ID;
        }

        /// <summary>
        /// This will take a save object and return the appropriate component or entity
        /// </summary>
        /// <param name="toDeserialize">The save object to serialize</param>
        /// <returns>An entity or component</returns>
        public abstract object Deserialize(SaveObject toDeserialize);

    }
}
