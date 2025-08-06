using UnityEngine;

[CreateAssetMenu(fileName = "DungeonOutputConfig", menuName = "Dungeon/OutputConfig")]
public class DungeonOutputConfigSO : ScriptableObject
{
    [SerializeField] private Renderer _levelLayoutDisplay;
    [SerializeField] private GameObject[] _tiles3d;
    private Renderer _instance;

    public Renderer LevelLayoutDisplay 
    { 
        get 
        {
            if (_instance == null) _instance = ObjectSpawnerSingleton.Instance.Spawn(_levelLayoutDisplay);
            return _instance; 
        } 
    }

    public GameObject[] Tiles3d => _tiles3d;
}