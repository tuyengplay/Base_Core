using EranCore.PoolObject;
using UnityEngine;

public class ItemPool : BaseItem
{
    [SerializeField]
    private ItemID idPool;
    public ItemID IdPool
    {
        get => idPool;
    }
}
