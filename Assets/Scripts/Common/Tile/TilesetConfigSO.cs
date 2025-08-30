using UnityEngine;

namespace ProcGenSys.Common.Tile
{
    [CreateAssetMenu(menuName = "ProcGen/Tile/TilePrefab/Tile Set")]
    public class TilesetConfigSO : ScriptableObject
    {
        [Tooltip("Array of 16 prefabs, one for each marching squares configuration (0-15). Use null for empty spaces.")]
        public GameObject[] tilePrefabs = new GameObject[16];
        public Material overrideMaterial;
    }
}