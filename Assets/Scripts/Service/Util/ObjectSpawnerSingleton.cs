using UnityEngine;

public class ObjectSpawnerSingleton : MonoBehaviour
{
    private static ObjectSpawnerSingleton _instance;
    public static ObjectSpawnerSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObjectSpawnerSingleton>();

                if (_instance == null)
                {
                    var spawnerObject = new GameObject("ObjectSpawnerSingleton");
                    _instance = spawnerObject.AddComponent<ObjectSpawnerSingleton>();
                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(spawnerObject);
                    }
                    Debug.LogWarning("ObjectSpawnerSingleton was not found in the scene. Created a new instance.");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"Another instance of ObjectSpawnerSingleton already exists. Destroying this duplicate on {gameObject.name}.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
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
