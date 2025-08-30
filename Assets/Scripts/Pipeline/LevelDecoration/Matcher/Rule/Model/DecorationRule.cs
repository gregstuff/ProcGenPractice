using ProcGenSys.Common.Tile;
using ProcGenSys.Editor.Model;
using ProcGenSys.Utilities.Extension;
using System;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher.Rule
{
    [Serializable]
    public class DecorationRule
    {
        public string Name;
        public TileMatchingRuleSO[] MatchingPattern1D;
        public Vector2Int SpawnCell;
        public Vector2Int[] PostSpawnBlockedCells;
        public GameObject Prefab;
        public Vector3 SpawnScale;
        public Vector3 SpawnRotation;
        public Vector3 SpawnPositionOffset;

        public int ChancesPerSpace;
        public int ChancesPerMap;
        public float ChanceToPlace;

        public int Priority = 0;

        public string CategoryId;

        public int SameCategoryMinDistance = 0;

        public int SamePrefabMinDistance = 0;

        public int BlockRadiusDistance = 0;

        public Vector2Int[] ExtraBlockOffsets;

        public int PatternHeight;
        public int PatternWidth;

        private TileMatchingRuleSO[,] _matchingPattern2D;
        public TileMatchingRuleSO[,] MatchingPattern2D => From1DTo2DArray();

        public DecorationRule() { }

        public DecorationRule(DecorationRuleUIModel model)
        {
            Name = model.Name;
            SpawnCell = model.SpawnCell;
            PostSpawnBlockedCells = model.PostSpawnBlockedCells;
            Prefab = model.Prefab;
            SpawnScale = model.SpawnScale;
            SpawnRotation = model.SpawnRotation;
            SpawnPositionOffset = model.SpawnPositionOffset;

            ChancesPerSpace = model.ChancesPerSpace;
            ChancesPerMap = model.ChancesPerMap;
            ChanceToPlace = model.ChanceToPlace;

            Priority = model.Priority;
            CategoryId = model.CategoryId;
            SameCategoryMinDistance = model.SameCategoryMinDistance;
            SamePrefabMinDistance = model.SamePrefabMinDistance;
            BlockRadiusDistance = model.BlockRadiusDistance;
            ExtraBlockOffsets = model.ExtraBlockOffsets;

            SetMatchingPattern(model.MatchingPattern);
        }

        public void Init()
        {
            _matchingPattern2D = null; // cache rebuild per run
        }

        public void Deconstruct(
            out string name,
            out TileMatchingRuleSO[,] matchingPattern,
            out Vector2Int spawnCell,
            out Vector2Int[] postSpawnBlockedCells,
            out GameObject prefab,
            out Vector3 spawnScale,
            out Vector3 spawnRotation,
            out Vector3 spawnPositionOffset)
        {
            name = Name;
            matchingPattern = From1DTo2DArray();
            spawnCell = SpawnCell;
            postSpawnBlockedCells = PostSpawnBlockedCells;
            prefab = Prefab;
            spawnScale = SpawnScale;
            spawnRotation = SpawnRotation;
            spawnPositionOffset = SpawnPositionOffset;
        }

        public TileMatchingRuleSO[,] From1DTo2DArray()
        {
            if (_matchingPattern2D != null) return _matchingPattern2D;

            var result = new TileMatchingRuleSO[PatternHeight, PatternWidth];
            for (int y = 0; y < PatternHeight; y++)
                for (int x = 0; x < PatternWidth; x++)
                    result[y, x] = MatchingPattern1D[y * PatternWidth + x];

            _matchingPattern2D = result;
            return result;
        }

        public void SetMatchingPattern(TileMatchingRuleSO[,] pattern)
        {
            PatternHeight = pattern.GetLength(0);
            PatternWidth = pattern.GetLength(1);

            MatchingPattern1D = new TileMatchingRuleSO[PatternHeight * PatternWidth];
            for (int y = 0; y < PatternHeight; y++)
                for (int x = 0; x < PatternWidth; x++)
                    MatchingPattern1D[y * PatternWidth + x] = pattern[y, x];

            _matchingPattern2D = null;
        }

        public bool Matches(int y, int x, TileTypeSO tileType)
        {
            var pattern = MatchingPattern2D;
            if (y < 0 || x < 0 || y >= pattern.GetLength(0) || x >= pattern.GetLength(1))
                throw new Exception($"Out of range for rule {Name} y:{y} x:{x}");
            return pattern[y, x].MatchesTile(tileType);
        }
    }
}