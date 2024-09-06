using GameCore;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : ItemPool
{
    public Image icon;

    public void SetData(ItemID _id)
    {
        //icon.sprite = SpriteManager.Ins.GetSprite(_id);
    }
}
