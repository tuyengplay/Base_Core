using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore;
using EranCore;
using EranCore.Data;
using System;
namespace GameCore
{
    [CreateAssetMenu(fileName = "User", menuName = "Manager/User")]
    public class User : ScriptableSingleton<User>
    {
        public static User Ins
        {
            get
            {
                if (ins == null)
                    ins = Resources.Load<User>("DataManagers/User");
                return ins;
            }
        }

        private UserData userData;
        private string userDataPath = "userDataPath";

        public void Init()
        {
            userData = DataStream.ReadDataExist<UserData>(userDataPath);
            if (userData == null)
            {
                userData = new UserData();
            }
        }
        public int this[ItemID _id]
        {
            get
            {
                if (userData.items.ContainsKey(_id))
                {
                    return userData.items[_id];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (userData.items.ContainsKey(_id))
                {
                    userData.items[_id] = value;
                }
                else
                {
                    userData.items.Add(_id, value);
                }
                GameEvent.OnItemChanged?.Invoke(_id);
                Save();
            }
        }
        public ItemBuyADS GetItemADS(ItemID _id)
        {
            ItemBuyADS item = userData.listBuyADS.Find(x => x.ID == _id);
            if (item == null)
            {
                item = new ItemBuyADS(_id);
            }
            return item;
        }
        public void SetItemAds(ItemID id)
        {
            userData.listBuyADS.Find(x => x.ID == id).ADSWatched++;
            Save();
        }
        public void Save()
        {
            DataStream.SaveData<UserData>(userDataPath, userData);
        }

        protected override void OnLoadButton(Action _onDone)
        {
            _onDone?.Invoke();
        }
    }
    [System.Serializable]
    public class UserData
    {
        public Dictionary<ItemID, int> items;
        public List<ItemBuyADS> listBuyADS;
        public UserData()
        {
            items = new Dictionary<ItemID, int>();
            listBuyADS = new List<ItemBuyADS>();
        }
    }
    [System.Serializable]
    public class ItemBuyADS
    {
        private ItemID itemID;
        private int adsWatched;

        public ItemBuyADS(ItemID _itemID)
        {
            itemID = _itemID;
            adsWatched = 0;
        }

        public ItemID ID
        {
            get { return itemID; }
        }

        public int ADSWatched
        {
            get => adsWatched;
            set
            {
                adsWatched = value;
            }
        }
    }
    [System.Serializable]
    public class ItemValueInt
    {
        [SerializeField] private ItemID id;
        [SerializeField] private int value;

        public ItemValueInt(ItemID _id, int _value)
        {
            this.id = _id;
            this.value = _value;
        }

        public ItemID ID => this.id;
        public int Value => this.value;
    }

    [System.Serializable]
    public class ItemValueFloat
    {
        [SerializeField] private ItemID id;
        [SerializeField] private float value;

        public ItemValueFloat(ItemID _id, float _value)
        {
            this.id = _id;
            this.value = _value;
        }

        public ItemID ID => this.id;
        public float Value => this.value;
    }
}