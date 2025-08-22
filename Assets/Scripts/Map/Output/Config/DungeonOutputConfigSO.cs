using DungeonGeneration.Map.SO;
using DungeonGeneration.Service.Util;
using UnityEngine;

namespace DungeonGeneration.Map.Output.SO
{

    [CreateAssetMenu(fileName = "DungeonOutputConfig", menuName = "Dungeon/OutputConfig")]
    public class DungeonOutputConfigSO : ScriptableObject
    {
        [SerializeField] private Renderer _levelLayoutDisplay;
        [SerializeField] private TilesetConfigSO _tileset;
        private Renderer _instance;

        public TilesetConfigSO Tileset => _tileset;

        public Renderer LevelLayoutDisplay
        {
            get
            {
                if (_instance == null) _instance = ObjectSpawnerSingleton.Instance.Spawn(_levelLayoutDisplay);
                return _instance;
            }
        }

    }
}