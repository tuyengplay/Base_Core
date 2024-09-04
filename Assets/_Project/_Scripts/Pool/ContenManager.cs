using EranCore.Panel.Prefabs;
using EranCore.PoolObject;
using System;
using UnityEngine;

public class ContenManager : ContentPrefabs
{
    public override GameObject Spawn(GameObject _refContent)
    {
        return ManagerPool.Spawn(_refContent);
    }
    public static BaseItemPool Emit(ItemID _id)
    {
        BaseItemPool prefab = GetPrefab<BaseItemPool>(x =>
        {
            return x.IdPool == _id;
        });

        return Emit<BaseItemPool>(prefab);
    }
    public static void EmitResource<T>(string _name, Action<T> _result, string path = "Prefabs")
    {
        ManagerResouce.GetPrefab(_name, (data) =>
        {
            GameObject clone = ManagerPool.Spawn(data);
            T get = clone.GetComponent<T>();
            _result?.Invoke(get);
        }, path);
    }
    public static void EmitResource(string _name, Action<GameObject> _result, string path = "Prefabs")
    {
        ManagerResouce.GetPrefab(_name, (data) =>
        {
            _result?.Invoke(ManagerPool.Spawn(data));
        }, path);
    }
}
