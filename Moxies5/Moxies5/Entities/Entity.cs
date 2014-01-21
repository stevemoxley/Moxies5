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
using Moxies5.Components;
using Moxies5.Serialization;
using Moxies5.Controllers;

namespace Moxies5.Entities
{
    public class Entity:ISerialize
    {

        #region Fields

        private List<Component> AllComponents = new List<Component>();
        private List<Component> ComponentWaitList = new List<Component>();
        private List<Component> ComponentDeleteList = new List<Component>();

        private Dictionary<string, Component> _componentsDictionary = new Dictionary<string, Component>();

        private bool _initialized = false;

        #endregion

        #region Properties
        public List<Component> Components
        {
            get
            {
                return AllComponents;
            }
        }

        public Dictionary<String, Component> ComponentsDictionary
        {
            get
            {
                return _componentsDictionary;
            }
        }

        #endregion

        #region Getters and Setters
        #endregion

        public Entity(List<Component> components)
        {
            this.AllComponents = components;
            foreach (Component comp in components)
            {
                _componentsDictionary.Add(comp.Name, comp);
            }
        }

        public Entity()
        {

        }

        /// <summary>
        /// Initialize this entity
        /// </summary>
        public void Inititalize()
        {
            if (_initialized)
            {
                return;
            }
            else
            {
                _initialized = true;
            }
        }

        /// <summary>
        /// Add a component to this entity
        /// </summary>
        /// <param name="aComponent">The component to add</param>
        public void AddComponent(Component aComponent)
        {
            if (aComponent == null)
            {
                throw new Exception("Component is null");
            }

            if(AllComponents.Contains(aComponent))
            {
                throw new Exception("Entity already contains component");
            }
            aComponent.Initialize();
            aComponent.Start();
            ComponentWaitList.Add(aComponent);

        }

        /// <summary>
        /// Add a component that will be initialized before first update is called
        /// </summary>
        /// <param name="aComponent">The component to add</param>
        public void AddInitialComponent(Component aComponent)
        {
            if (aComponent == null)
            {
                throw new Exception("Component is null");
            }

            if (AllComponents.Contains(aComponent))
            {
                throw new Exception("Entity already contains component");
            }
            aComponent.Initialize();
            aComponent.Start();
            AllComponents.Add(aComponent);
            _componentsDictionary.Add(aComponent.Name, aComponent);

        }

        public void RemoveComponent(string component)
        {
            if(_componentsDictionary.ContainsKey(component))
            {
                Component toRemove = _componentsDictionary[component];
                ComponentDeleteList.Add(toRemove);
            }
        }

        public void RemoveComponent(Type type)
        {
            if (HasComponent(type))
            {
                Component toRemove = GetComponent(type);
                ComponentDeleteList.Add(toRemove);
            }
        }

        /// <summary>
        /// Update this entity
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            RunThoughWaitLists();
        }

        public void RunThoughWaitLists()
        {
            for (int i = 0; i < ComponentWaitList.Count; i++)
            {
                Component aComponent = ComponentWaitList[i];
                AllComponents.Add(aComponent);
                _componentsDictionary.Add(aComponent.Name, aComponent);
            }
            ComponentWaitList.Clear();

            for (int i = 0; i < ComponentDeleteList.Count; i++)
            {
                Component dComponent = ComponentDeleteList[i];
                AllComponents.Remove(dComponent);
                _componentsDictionary.Remove(dComponent.Name);
            }
            ComponentDeleteList.Clear();

            SortComponents();
        }

        void SortComponents()
        {
            AllComponents.Sort(
                 delegate(Component p1, Component p2)
                 {
                     return p1.UpdateOrder.CompareTo(p2.UpdateOrder);
                 }
             );
        }
           
        /// <summary>
        /// Check to see if this entity has the component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool HasComponent(String componentName)
        {
            if(_componentsDictionary.ContainsKey(componentName))
                return true;
            else
            return false;
        }

        public bool HasComponent(Type _type)
        {
            for (int x = 0; x < AllComponents.Count; x++)
            {
                if (AllComponents[x].GetType() == _type)
                {
                    return true;
                }
            }
            return false;
        }

        public Component GetComponent(String componentName)
        {
            for (int x = 0; x < AllComponents.Count; x++)
            {
                if (AllComponents[x].Name == componentName)
                {
                    return AllComponents[x];
                }
            }

            return null;
        }

        public Component GetComponent(Type _type)
        {
            for (int x = 0; x < AllComponents.Count; x++)
            {
                if (AllComponents[x].GetType() == _type)
                {
                    return AllComponents[x];
                }
            }

            return null;
        }

        public void SetComponent(Component newComponent)
        {
            if (HasComponent(newComponent.Name))
            {
                _componentsDictionary[newComponent.Name] = newComponent;
            }
        }

        /// <summary>
        /// Call this to remove the entity from the game
        /// </summary>
        public void Remove()
        {
            if(HasComponent(typeof(PhysicsComponent)))
            {
                PhysicsComponent pc = (PhysicsComponent)GetComponent(typeof(PhysicsComponent));
                PhysicsController.RemoveBody(pc.Body);
            }
            if (HasComponent(typeof(SensorComponent)))
            {
                SensorComponent sc =(SensorComponent)GetComponent(typeof(SensorComponent));
                PhysicsController.RemoveBody(sc.Body);
            }
            //Remove all components
            for (int i = 0; i < Components.Count; i++)
            {
                RemoveComponent(Components[i].Name);
            }
            RunThoughWaitLists();
            EntityManager.RemoveEntity(this);
        }


        public SaveObject Serialize(int ID)
        {
            EntitySave save = new EntitySave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class EntitySave : SaveObject
    {

        public override void Serialize(object toSerialize, int ID)
        {
            Entity saveEntity = (Entity)toSerialize;
            base.Serialize(saveEntity, ID);
        }

        public override object Deserialize(SaveObject toDeserialize)
        {
            Entity entity = new Entity();
            EntitySave eSave =  (EntitySave)toDeserialize;
            return entity;
        }
    }
}
