using EranCore.PoolObject;
using UnityEngine;
namespace GameCore
{
    public class ItemPool : BaseItem
    {
        [SerializeField]
        private ItemID idPool;
        public ItemID IdPool
        {
            get => idPool;
        }
    }
}