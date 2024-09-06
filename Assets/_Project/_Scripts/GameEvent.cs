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
                .Where(x => x.IsStatic && eventType.IsAssignableFrom(x.FieldType))
                .ForEach(x => (x.GetValue(null) as UnityEventBase).RemoveAllListeners());
        }
    }
}