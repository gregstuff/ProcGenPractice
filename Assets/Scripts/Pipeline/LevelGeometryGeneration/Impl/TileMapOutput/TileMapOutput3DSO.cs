using DungeonGeneration.Utilities.ProcGenSys.Utilities;
using ProcGenSys.Common.LevelBundle;
using ProcGenSys.Common.Tile;
using ProcGenSys.Service.Util;
using System;
using System.Linq;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelGeometryGeneration
{
    [CreateAssetMenu(menuName = "ProcGen/Output/Tile/3D/Tile Map 3D")]
    public class TileMapOutput3DSO : OutputGeneratorSO
    {
        protected static readonly string DUNGEON_PARENT_TAG = "DungeonParent";
        protected static readonly string TILE_CHILD_TAG = "TILES";

        [SerializeField] private GameObject _dungeonRoot;
        [SerializeField] private TilesetConfigSO _tileset;

        private float _mapScale;

        public override void OutputMap(ICapabilityProvider level)
        {
            var marchingSquaresGrid = GenerateMarchingSquaresGrid(level);
            var parent = GetDungeonParentCleanChildren();
            SpawnTiles(marchingSquaresGrid, parent.transform);
        }


        private int[,] GenerateMarchingSquaresGrid(ICapabilityProvider level)
        {
            if (!level.TryGet<BlockMask>(out var blockingMask)
                || !level.TryGet<Scale>(out var mapScale))
            {
                throw new ArgumentException("Level Generation Selected did not have required data for Tile Map 3d Output");
            }

            var grid = blockingMask.Mask;
            _mapScale = mapScale.MapScale;
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

            var spawned = ObjectSpawnerSingleton.Instance.Spawn(_dungeonRoot);

            var tileRoot = spawned.transform.Cast<Transform>()
                             .Select(t => t.gameObject)
                             .FirstOrDefault(go => go.CompareTag(TILE_CHILD_TAG));

            return tileRoot;
        }

        private void SpawnTiles(int[,] marchingSquaresGrid, Transform parent)
        {
            Vector3 tileScale = new Vector3(_mapScale, 1, _mapScale);
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