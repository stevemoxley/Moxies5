using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Moxies5.Entities;
using Moxies5.Components;
using Moxies5.Controllers;

namespace Moxies5.Serialization
{
    public struct SaveGameData
    {
        public List<SaveObject> data;
    }

    public static class Serializer
    {

        #region Fields
        //Saving and loading states
        static StorageDevice storageDevice;
        static SavingState savingState = SavingState.NotSaving;
        static LoadingState loadingState = LoadingState.NotLoading;
        static StorageContainer storageContainer;
        static IAsyncResult asyncResult;
        static PlayerIndex playerIndex = PlayerIndex.One;
        public static SaveGameData saveGameData;
        static string filename = "MoxieSave.sav";
        #endregion

        #region Enums

        enum SavingState
        {
            NotSaving,
            ReadyToSelectStorageDevice,
            SelectingStorageDevice,
            ReadyToOpenStorageContainer,    // once we have a storage device start here
            OpeningStorageContainer,
            ReadyToSave
        }

        enum LoadingState
        {
            NotLoading,
            ReadyToSelectStorageDevice,
            SelectingStorageDevice,
            ReadyToOpenStorageContainer,    // once we have a storage device start here
            OpeningStorageContainer,
            ReadyToLoad
        }
        #endregion

        /// <summary>
        /// This holds all of the data that is going to be serialized
        /// </summary>


        public static void Initalize()
        {
            saveGameData = new SaveGameData();
            saveGameData.data = new List<SaveObject>();
        }

        public static void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            UpdateSaving();
            UpdateLoading();
        }

        /// <summary>
        /// Call this to save the game
        /// </summary>
        public static void Save()
        {
            if (UIController.TargetMoxie != null)
                UIController.SetTargetMoxie(null);

            saveGameData.data.Clear();

            #region Serialize Entities and Components

            int ID = 0;
            int componentID = 0;
            for (int i = 0; i < EntityManager.EntityCount; i++)
            {
                if (EntityManager.EntityMasterList[i] is ISerialize)
                {
                    //Serialize the entity
                    Entity entity = EntityManager.EntityMasterList[i];
                    ISerialize toSerialize = (ISerialize)entity;
                    AddToSaveObjects(toSerialize.Serialize(ID));
        

                    //Serialize the entity's components
                    for (int y = 0; y < entity.Components.Count; y++)
                    {
                        ISerialize componentToSerialize = entity.Components[y] as ISerialize;
    
                        if (componentToSerialize != null)
                        {
                            SaveObject componentSaveObject = componentToSerialize.Serialize(componentID);
                            componentSaveObject.ParentEntityID = ID;
                            AddToSaveObjects(componentSaveObject);
                        }
                        componentID++;
                    }
                    ID++;
                }
            }
            #endregion

            if (savingState == SavingState.NotSaving && loadingState == LoadingState.NotLoading)
            {
                savingState = SavingState.ReadyToOpenStorageContainer;
            }

        }

        /// <summary>
        /// Call this to load the game
        /// </summary>
        public static void Load()
        {

            //This section should go up near the top. before the actual load stuff is called
            if (savingState == SavingState.NotSaving && loadingState == LoadingState.NotLoading)
            {
                loadingState = LoadingState.ReadyToOpenStorageContainer;
            }

            UpdateLoading();


        }

        /// <summary>
        /// NO TOUCHING!! NO TOUCHING!!!
        /// </summary>
        #region Saving and Loading Methods
        static void UpdateLoading()
        {

            switch (loadingState)
            {
                case LoadingState.ReadyToSelectStorageDevice:
#if XBOX
                    if (!Guide.IsVisible)
#endif
                    {
                        asyncResult = StorageDevice.BeginShowSelector(playerIndex, null, null);
                        loadingState = LoadingState.SelectingStorageDevice;
                    }
                    break;

                case LoadingState.SelectingStorageDevice:
                    if (asyncResult.IsCompleted)
                    {
                        storageDevice = StorageDevice.EndShowSelector(asyncResult);
                        loadingState = LoadingState.ReadyToOpenStorageContainer;
                    }
                    break;

                case LoadingState.ReadyToOpenStorageContainer:
                    if (storageDevice == null || !storageDevice.IsConnected)
                    {
                        loadingState = LoadingState.ReadyToSelectStorageDevice;
                    }
                    else
                    {
                        asyncResult = storageDevice.BeginOpenContainer("Game1StorageContainer", null, null);
                        loadingState = LoadingState.OpeningStorageContainer;
                    }
                    break;

                case LoadingState.OpeningStorageContainer:
                    if (asyncResult.IsCompleted)
                    {
                        storageContainer = storageDevice.EndOpenContainer(asyncResult);
                        loadingState = LoadingState.ReadyToLoad;
                    }
                    break;

                case LoadingState.ReadyToLoad:
                    if (storageContainer == null)
                    {
                        loadingState = LoadingState.ReadyToOpenStorageContainer;
                    }
                    else
                    {
                        try
                        {
                            LoadStream();
                        }
                        catch (IOException e)
                        {
                            // Replace with in game dialog notifying user of error
                            Console.WriteLine(e.ToString());
                        }
                        finally
                        {
                            storageContainer.Dispose();
                            storageContainer = null;
                            loadingState = LoadingState.NotLoading;
                        }
                    }
                    break;
            }
        }

        static void DeleteExisting()
        {
            if (storageContainer.FileExists(filename))
            {
                storageContainer.DeleteFile(filename);
            }
        }

        static void UpdateSaving()
        {
            switch (savingState)
            {
                case SavingState.ReadyToSelectStorageDevice:
#if XBOX
                    if (!Guide.IsVisible)
#endif
                    {
                        asyncResult = StorageDevice.BeginShowSelector(playerIndex, null, null);
                        savingState = SavingState.SelectingStorageDevice;
                    }
                    break;

                case SavingState.SelectingStorageDevice:
                    if (asyncResult.IsCompleted)
                    {
                        storageDevice = StorageDevice.EndShowSelector(asyncResult);
                        savingState = SavingState.ReadyToOpenStorageContainer;
                    }
                    break;

                case SavingState.ReadyToOpenStorageContainer:
                    if (storageDevice == null || !storageDevice.IsConnected)
                    {
                        savingState = SavingState.ReadyToSelectStorageDevice;
                    }
                    else
                    {
                        asyncResult = storageDevice.BeginOpenContainer("Game1StorageContainer", null, null);
                        savingState = SavingState.OpeningStorageContainer;
                    }
                    break;

                case SavingState.OpeningStorageContainer:
                    if (asyncResult.IsCompleted)
                    {
                        storageContainer = storageDevice.EndOpenContainer(asyncResult);
                        savingState = SavingState.ReadyToSave;
                    }
                    break;

                case SavingState.ReadyToSave:
                    if (storageContainer == null)
                    {
                        savingState = SavingState.ReadyToOpenStorageContainer;
                    }
                    else
                    {
                        try
                        {
                            DeleteExisting();
                            SerializeStream();
                        }
                        catch (IOException e)
                        {
                            // Replace with in game dialog notifying user of error
                            Console.WriteLine(e.ToString());
                        }
                        finally
                        {
                            storageContainer.Dispose();
                            storageContainer = null;
                            savingState = SavingState.NotSaving;
                        }
                    }
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Use this to add a save object to the list of things to be serialized
        /// </summary>
        /// <param name="toSave"></param>
        public static void AddToSaveObjects(SaveObject toSave)
        {
            saveGameData.data.Add(toSave);
        }

        static void SerializeStream()
        {
            using (Stream stream = storageContainer.CreateFile(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                serializer.Serialize(stream, saveGameData);
            }
        }

        static void LoadStream()
        {
            MainController.Reset();
            using (Stream stream = storageContainer.OpenFile(filename, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                SaveGameData data = (SaveGameData)serializer.Deserialize(stream);
                List<SaveObject> gameSaveObjects = data.data;

                Dictionary<int, Entity> EntityDictionary = new Dictionary<int, Entity>();

                foreach (SaveObject save in gameSaveObjects)
                {
                    object saveObject = save.Deserialize(save);
                    if (saveObject is Entity)
                    {
                        EntityDictionary.Add(save.ID, (Entity)saveObject);
                        EntityManager.AddEntity((Entity)saveObject);
                    }
                    if (saveObject is Component)
                    {
                        Entity parent = EntityDictionary[save.ParentEntityID];
                        Component component = (Component)saveObject;
                        component.SetParent(parent);
                        if (parent.HasComponent(component.Name))
                        {
                            parent.RemoveComponent(component.Name);
                            parent.RunThoughWaitLists();
                        }
                        parent.AddInitialComponent((Component)saveObject);
                    }
                }

            }
        }
    }
}
