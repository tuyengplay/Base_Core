using EranCore.PoolObject;
using UnityEngine;

public class BaseItemPool : BaseItem
{
    [SerializeField]
    private ItemID idPool;
    public ItemID IdPool
    {
        get => idPool;
    }
}
