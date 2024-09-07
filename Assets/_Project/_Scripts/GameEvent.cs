using GameCore;
using System;
using System.Linq;
using UnityEngine.Events;
namespace GameCore
{
    public static class GameEvent
    {
        public static UnityEvent<ItemID> OnItemChanged = new UnityEvent<ItemID>();
        public static void ClearDelegates()
        {
            Type eventType = typeof(UnityEventBase);
            typeof(GameEvent).GetFields()
                .Where(_x => _x.IsStatic && eventType.IsAssignableFrom(_x.FieldType))
                .ForEach(_x => (_x.GetValue(null) as UnityEventBase).RemoveAllListeners());
        }
    }
}