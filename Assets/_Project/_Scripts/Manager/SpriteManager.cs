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

        public Sprite GetSprite(ItemID itemID, int index = 0)
        {
            ItemSprite itemSprite = itemSprites.Find(x => x.ID == itemID);
            if (itemSprite != null)
            {
                if (index == 0)
                {
                    return itemSprite.sprite;
                }
                else
                {
                    if (itemSprite.others.Count() < index) return null;
                    return itemSprite.others[index - 1];
                }
            }

            return null;
        }

        public Sprite GetSpriteTheme(ItemID itemID, int indexTheme = 0)
        {
            ItemSprite itemSprite = itemSprites.Find(x => x.ID == itemID);
            if (itemSprite != null)
            {
                if (BaseScene.Instance.ThemeGame == Theme.Nomal)
                {
                    return itemSprite.sprite;
                }
                else
                {
                    if (itemSprite.others.Count > 0 && indexTheme < itemSprite.others.Count)
                    {
                        return itemSprite.others[indexTheme];
                    }
                }
            }

            return null;
        }

        public Sprite GetSpriteOther(ItemID id, int indexSpriteOther = 0)
        {
            ItemSprite itemSprite = itemSprites.Find(x => x.ID == id);
            if (itemSprite != null)
            {
                if (itemSprite.others.Count > 0 && indexSpriteOther < itemSprite.others.Count)
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

        public ItemSprite(ItemID iD, Sprite sprite)
        {
            ID = iD;
            this.sprite = sprite;
        }
    }
}