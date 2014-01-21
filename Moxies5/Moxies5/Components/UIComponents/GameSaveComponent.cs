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
    public class GameSaveComponent: Component, ISerialize
    {
        #region Fields
        private List<Item> _unlockedItems = new List<Item>();
        private int _money;
        public int MoxieCount;
        public int MaxMoxies = 10;
        #endregion

        #region Properties
        public int Money
        {
            get
            {
                return _money;
            }

            set
            {
                _money = value;
            }
        }

        public List<Item> UnlockedItemList
        {
            get
            {
                return _unlockedItems;
            }
        }


        #endregion

        #region Getters and Setters
        public void SetMoney(int newValue)
        {
            this._money = newValue;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentEntity">The parent entity for this component</param>
        public GameSaveComponent(Entity parentEntity)
            : base(parentEntity)
        {
            Name = "UISaveComponent";
            UpdateOrder = 4;
        }

        #endregion

        #region Methods

        public new void Start()
        {

        }

        public void AddUnlockedItem(Item item)
        {
            _unlockedItems.Add(item);
        }

        /// <summary>
        /// Add or subtract from player money
        /// </summary>
        /// <param name="amount">Positive or negative amount to add or subtract</param>
        public void ChangeMoney(int amount)
        {
            _money += amount;
        }
        #endregion

        public SaveObject Serialize(int ID)
        {
            GameSaveComponentSave save = new GameSaveComponentSave();
            save.Serialize(this, ID);
            return save;
        }
    }

    public class GameSaveComponentSave : SaveObject
    {
        GameSaveComponent UIS = null;
        public int Money { get; set; }
        public Item[] _unlockedItemList;
        public UIStates UIState;
        public int MoxieCount;

        public override void Serialize(object _uiSaveComponent, int ID)
        {
            GameSaveComponent uiSaveComponent = (GameSaveComponent)_uiSaveComponent;
            this.Money = uiSaveComponent.Money;
            this._unlockedItemList = uiSaveComponent.UnlockedItemList.ToArray();
            this.UIState = UIController.UIState;
            this.MoxieCount = uiSaveComponent.MoxieCount;
            base.Serialize(_uiSaveComponent, ID);
        }

        public override object Deserialize(SaveObject toDeserialize)
        {
            GameSaveComponentSave uilcSave = (GameSaveComponentSave)toDeserialize;
            UIS = new GameSaveComponent(null);
            UIS.Money = uilcSave.Money;
            UIS.MoxieCount = uilcSave.MoxieCount;
            for (int i = 0; i < uilcSave._unlockedItemList.Count(); i++)
            {
                UIS.AddUnlockedItem(uilcSave._unlockedItemList[i]);
            }
            UIController.ChangeUIState(uilcSave.UIState);
            UIController.SetGameSaveComponent(UIS);
            return UIS;
        }
    }
}
