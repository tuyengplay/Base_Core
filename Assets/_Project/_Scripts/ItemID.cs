using System.Collections.Generic;
using System;
using System.Linq;

public enum ItemID
{
    None,
    Ads,
    Coin,
    FXItem,
    Object1,
    //Pack
    pack_remove_ads = 1000,
    pack_end,
}
public static class ItemType
{
    public static List<ItemID> listIAPPack = ((ItemID[])Enum.GetValues(typeof(ItemID))).Where(_id => (int)_id >= (int)ItemID.pack_remove_ads && (int)_id < (int)ItemID.pack_end).ToList();

    public static bool IsIapPack(this ItemID _id)
    {
        return listIAPPack.Contains(_id);
    }
}