using UnityEngine;

public class ObjectSpawnerSingleton : MonoBehaviour
{
    private static ObjectSpawnerSingleton _instance;
    public static ObjectSpawnerSingleton Instance
    {
        get
        {
            if (_instance == null) _instance = new ObjectSpawnerSingleton();
            return _instance;
        }
    }

    public T Spawn<T>(T objToSpawn, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where T : Object
    {
        if (objToSpawn == null)
        {
            Debug.LogError("Cannot spawn: Object to spawn is null.");
            return null;
        }

        return Instantiate(objToSpawn, position, rotation, parent);
    }

}
