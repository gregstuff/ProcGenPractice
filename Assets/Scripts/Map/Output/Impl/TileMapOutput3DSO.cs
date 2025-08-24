using UnityEngine;
using DungeonGeneration.Utilities;
using DungeonGeneration.Service.Util;
using DungeonGeneration.Map.SO;
using System;

namespace DungeonGeneration.Map.Output.Impl
{
    [CreateAssetMenu(menuName = "ProcGen/Output/Tile Map 3D")]
    public class TileMapOutput3DSO : OutputGeneratorSO
    {
        private static readonly string DUNGEON_PARENT_TAG = "DungeonParent";

        [SerializeField] private GameObject _dungeonRoot;
        [SerializeField] private TilesetConfigSO _tileset;

        public override void OutputMap(ICapabilityProvider level)
        {
            var marchingSquaresGrid = GenerateMarchingSquaresGrid(level);
            var parent = GetDungeonParentCleanChildren();
            SpawnTiles(marchingSquaresGrid, parent.transform);
        }


        private int[,] GenerateMarchingSquaresGrid(ICapabilityProvider level)
        {
            if (!level.TryGet<BlockMask>(out var blockingMask))
            {
                throw new ArgumentException("");
            }

            var grid = blockingMask.Mask;
            return MarchingSquaresUtility.ToMarchingSquaresInts(grid);
        }

        private GameObject GetDungeonParentCleanChildren()
        {
            var existing = GameObject.FindGameObjectWithTag(DUNGEON_PARENT_TAG);
            if (existing != null)
            {

#if UNITY_EDITOR
            UnityEngine.Object.DestroyImmediate(existing);
#else
            Destroy(existing);
#endif
            }
            return ObjectSpawnerSingleton.Instance.Spawn(_dungeonRoot);
        }

        private void SpawnTiles(int[,] marchingSquaresGrid, Transform parent)
        {
            Vector3 tileScale = _tileset.tileScale;
            for (int y = 0; y < marchingSquaresGrid.GetLength(0); y++)
            {
                for (int x = 0; x < marchingSquaresGrid.GetLength(1); x++)
                {
                    int tileIndex = marchingSquaresGrid[y, x];
                    GameObject tilePrefab = _tileset.tilePrefabs[tileIndex];
                    if (tilePrefab != null)
                    {
                        Vector3 position = CalculateTilePosition(x, y, tileScale);
                        GameObject tile = ObjectSpawnerSingleton.Instance.Spawn(
                            tilePrefab,
                            position,
                            Quaternion.identity,
                            parent
                        );
                        ConfigureTile(tile, tileScale);
                    }
                }
            }
        }

        private Vector3 CalculateTilePosition(int x, int y, Vector3 tileScale)
        {
            return new Vector3(x * tileScale.x, 0, y * tileScale.z);
        }

        private void ConfigureTile(GameObject tile, Vector3 tileScale)
        {
            tile.transform.localScale = tileScale;
            tile.isStatic = true;
            if (_tileset.overrideMaterial != null)
            {
                var renderer = tile.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = _tileset.overrideMaterial;
                }
            }
        }

    }
}