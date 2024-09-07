using EranCore;
using GameCore;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace GameCore
{
[CreateAssetMenu(fileName = "SpriteManager", menuName = "Manager/SpriteManager")]
    public class SpriteManager : ScriptableSingleton<SpriteManager>
    {
        public static SpriteManager Ins
        {
            get
            {
                if (ins == null)
                    ins = Resources.Load<SpriteManager>("DataManagers/SpriteManager");
                return ins;
            }
        }
        [Searchable, TableList]
        [SerializeField] List<ItemSprite> itemSprites;

        public Sprite GetSprite(ItemID _itemID, int _index = 0)
        {
            ItemSprite itemSprite = itemSprites.Find(_x => _x.ID == _itemID);
            if (itemSprite != null)
            {
                if (_index == 0)
                {
                    return itemSprite.sprite;
                }
                else
                {
                    if (itemSprite.others.Count() < _index) return null;
                    return itemSprite.others[_index - 1];
                }
            }

            return null;
        }

        public Sprite GetSpriteTheme(ItemID _itemID, int _indexTheme = 0)
        {
            ItemSprite itemSprite = itemSprites.Find(_x => _x.ID == _itemID);
            if (itemSprite != null)
            {
                if (BaseScene.Instance.ThemeGame == Theme.Nomal)
                {
                    return itemSprite.sprite;
                }
                else
                {
                    if (itemSprite.others.Count > 0 && _indexTheme < itemSprite.others.Count)
                    {
                        return itemSprite.others[_indexTheme];
                    }
                }
            }

            return null;
        }

        public Sprite GetSpriteOther(ItemID _id, int _indexSpriteOther = 0)
        {
            ItemSprite itemSprite = itemSprites.Find(_x => _x.ID == _id);
            if (itemSprite != null)
            {
                if (itemSprite.others.Count > 0 && _indexSpriteOther < itemSprite.others.Count)
                {
                    return itemSprite.others[0];
                }
            }

            return null;
        }

        protected override void OnLoadButton(Action _onDone)
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public class ItemSprite
    {
        public ItemID ID;
        public Sprite sprite;
        public List<Sprite> others;

        public ItemSprite(ItemID _iD, Sprite _sprite)
        {
            ID = _iD;
            this.sprite = _sprite;
        }
    }
}