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
using Moxies5.Entities;

namespace Moxies5.Entities
{
    public static class EntityManager
    {
        #region Fields
        static List<Entity> _entityMasterList = new List<Entity>();
        private static List<Entity> _entityRemoveWaitList = new List<Entity>();
        #endregion

        #region Properties
        public static List<Entity> EntityMasterList
        {
            get
            {
                return _entityMasterList;
            }
        }

        public static int EntityCount
        {
            get
            {
                return _entityMasterList.Count;
            }
        }
        #endregion

        static EntityManager()
        {

        }

        public static void Update(GameTime gameTime)
        {
            for (int i = 0; i < _entityRemoveWaitList.Count; i++)
            {
                _entityMasterList.Remove(_entityRemoveWaitList[i]);
            }
            _entityRemoveWaitList.Clear();
        }

        public static void AddEntity(Entity aEntity)
        {
            _entityMasterList.Add(aEntity);
        }

        public static void RemoveEntity(Entity rEntity)
        {
            _entityRemoveWaitList.Add(rEntity);
        }

        public static List<Entity> GetEntityMasterList()
        {
            return _entityMasterList;
        }

    }
}
