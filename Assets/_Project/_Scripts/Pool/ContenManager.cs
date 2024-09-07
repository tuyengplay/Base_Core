using EranCore.Panel.Prefabs;
using EranCore.PoolObject;
using System;
using UnityEngine;
namespace GameCore
{
    public class ContenManager : ContentPrefabs
    {
        public override GameObject Spawn(GameObject _refContent)
        {
            return ManagerPool.Spawn(_refContent);
        }
        public static ItemPool Emit(ItemID _id)
        {
            ItemPool prefab = GetPrefab<ItemPool>(_x =>
            {
                return _x.IdPool == _id;
            });

            return Emit<ItemPool>(prefab);
        }
        public static void EmitResource<T>(string _name, Action<T> _result, string _path = "Prefabs")
        {
            ManagerResouce.GetPrefab(_name, (_data) =>
            {
                GameObject clone = ManagerPool.Spawn(_data);
                T get = clone.GetComponent<T>();
                _result?.Invoke(get);
            }, _path);
        }
        public static void EmitResource(string _name, Action<GameObject> _result, string _path = "Prefabs")
        {
            ManagerResouce.GetPrefab(_name, (_data) =>
            {
                _result?.Invoke(ManagerPool.Spawn(_data));
            }, _path);
        }
    }
}