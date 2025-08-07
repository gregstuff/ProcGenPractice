using UnityEditor;
using UnityEngine;

public class TileMapOutput3d : IDungeonOutput
{
    private static readonly string DUNGEON_PARENT_TAG = "DungeonParent";
    private readonly DungeonOutputConfigSO _config;

    public TileMapOutput3d(DungeonOutputConfigSO config)
    {
        _config = config;
    }

    public void OutputMap(Level level)
    {
        if (!ValidateConfig() || !GetDungeonParentCleanChildren(out var dungeonParent))
        {
            return;
        }

        var marchingSquaresGrid = GenerateMarchingSquaresGrid(level);
        SpawnTiles(marchingSquaresGrid, dungeonParent.transform);
    }

    private bool ValidateConfig()
    {
        if (_config == null)
        {
            Debug.LogError("DungeonOutputConfigSO is not assigned.");
            return false;
        }
        if (_config.Tileset == null)
        {
            Debug.LogError("TilesetConfigSO is not assigned in DungeonOutputConfigSO.");
            return false;
        }
        return true;
    }

    private int[,] GenerateMarchingSquaresGrid(Level level)
    {
        var grid = level.ToBlockedMap();
        return MarchingSquaresUtility.ToMarchingSquaresInts(grid);
    }

    private bool GetDungeonParentCleanChildren(out GameObject parent)
    {
        parent = GameObject.FindGameObjectWithTag(DUNGEON_PARENT_TAG);
        if (parent == null)
        {
            Debug.Log("No Dungeon parent found!");
            return false;
        }

        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            if (Application.isPlaying)
            {
                Object.Destroy(child);
            }
            else
            {
                #if UNITY_EDITOR
                Undo.DestroyObjectImmediate(child);
                #endif
            }
        }

        return true;
    }

    private void SpawnTiles(int[,] marchingSquaresGrid, Transform parent)
    {
        Vector3 tileScale = _config.Tileset.tileScale;

        for (int y = 0; y < marchingSquaresGrid.GetLength(0); y++)
        {
            for (int x = 0; x < marchingSquaresGrid.GetLength(1); x++)
            {
                int tileIndex = marchingSquaresGrid[y, x];
                GameObject tilePrefab = _config.Tileset.tilePrefabs[tileIndex];

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

        Debug.Log($"3D dungeon generated with tileset: {_config.Tileset.name}, scale: {tileScale}");
    }

    private Vector3 CalculateTilePosition(int x, int y, Vector3 tileScale)
    {
        return new Vector3(x * tileScale.x, 0, y * tileScale.z);
    }

    private void ConfigureTile(GameObject tile, Vector3 tileScale)
    {
        tile.transform.localScale = tileScale;

        if (_config.Tileset.overrideMaterial != null)
        {
            var renderer = tile.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = _config.Tileset.overrideMaterial;
            }
        }
    }
}