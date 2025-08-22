using UnityEngine;

namespace DungeonGeneration.Map.SO
{
    [CreateAssetMenu(fileName = "TilesetConfig", menuName = "Dungeon/TilesetConfig", order = 1)]
    public class TilesetConfigSO : ScriptableObject
    {
        [Tooltip("Array of 16 prefabs, one for each marching squares configuration (0-15). Use null for empty spaces.")]
        public GameObject[] tilePrefabs = new GameObject[16];

        // Optional: Add more customization, e.g., materials for overrides or scaling factors
        public Material overrideMaterial;
        public Vector3 tileScale = Vector3.one;
    }
}