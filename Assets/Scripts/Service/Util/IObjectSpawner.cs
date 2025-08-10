
using UnityEngine;

public interface IObjectSpawner
{
    public T Spawn<T>(T objToSpawn, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where T : Object;
}
