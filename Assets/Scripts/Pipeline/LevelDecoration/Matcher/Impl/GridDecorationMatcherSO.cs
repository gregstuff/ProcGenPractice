using ProcGenSys.Common.LevelBundle;
using ProcGenSys.Common.Tile;
using ProcGenSys.Pipeline.LevelDecoration.Matcher.Coverage;
using ProcGenSys.Pipeline.LevelDecoration.Matcher.Rule;
using ProcGenSys.Service.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher
{
    [CreateAssetMenu(menuName = "ProcGen/Decorators/Matchers/Grid Decoration Matcher")]
    public class GridDecorationMatcherSO : DecorationMatcherSO
    {
        [SerializeField] private DecorationRulesetSO _decorationRuleset;
        [SerializeField] private CoverageSO _coverage;


        private Dictionary<Vector2Int, List<DecorationRule>> _dimensionsToDecorationRules;
        private Dictionary<DecorationRule, int> _ruleIndex;
        private Dictionary<Vector2Int, string> _spaceMap;
        private int _maxWidth, _maxHeight;
        private int _seed;

        public override IEnumerable<IDecorationMatch> GetDecorationMatches(ICapabilityProvider level)
        {
            if (!level.TryGet<TileLayer>(out var tileLayer) || !level.TryGet<SpaceMap>(out var spaceMap))
                throw new Exception($"{nameof(GridDecorationMatcherSO)} requires TileLayer and SpaceMap.");

            Init(spaceMap.Map);

            var tiles = tileLayer.Tiles;
            InitPatternData();

            var candidates = GenerateCandidates(tiles);
            var results = ResolveCandidates(candidates, tiles);

            if (_coverage != null)
            {
                var extended = new List<IDecorationMatch>(results);
                var hardBlocked = BuildOccupiedSetFromResults(extended);
                var coverage = GenerateCoverageMatches(tiles, hardBlocked);
                extended.AddRange(coverage);
                return extended;
            }

            return results;
        }

        private struct Candidate
        {
            public DecorationRule Rule;
            public Vector2Int Anchor;
            public List<Vector2Int> HardCells;
            public int Priority;
            public int Tie;
        }

        private List<Candidate> GenerateCandidates(TileTypeSO[,] tiles)
        {
            int rows = tiles.GetLength(0), cols = tiles.GetLength(1);
            var outList = new List<Candidate>(Mathf.Max(256, rows * cols / 2));

            for (int y = 0; y < rows; ++y)
            {
                for (int x = 0; x < cols; ++x)
                {
                    for (int h = 1; h <= _maxHeight && y + h <= rows; ++h)
                        for (int w = 1; w <= _maxWidth && x + w <= cols; ++w)
                        {
                            var dims = new Vector2Int(w, h);
                            if (!_dimensionsToDecorationRules.TryGetValue(dims, out var rules)) continue;

                            foreach (var rule in rules)
                            {
                                bool failed = false;
                                for (int ry = 0; ry < h && !failed; ++ry)
                                    for (int rx = 0; rx < w; ++rx)
                                        if (!rule.Matches(ry, rx, tiles[y + ry, x + rx])) { failed = true; break; }
                                if (failed) continue;

                                var anchor = new Vector2Int(x + rule.SpawnCell.x, y + rule.SpawnCell.y);
                                if (anchor.x < 0 || anchor.x >= cols || anchor.y < 0 || anchor.y >= rows) continue;

                                var hardCells = BuildHardBlockCells(x, y, rule, anchor);

                                int rIndex = _ruleIndex[rule];
                                int tie = StableHash(_seed, anchor.x, anchor.y, rIndex);

                                outList.Add(new Candidate
                                {
                                    Rule = rule,
                                    Anchor = anchor,
                                    HardCells = hardCells,
                                    Priority = rule.Priority,
                                    Tie = tie
                                });
                            }
                        }
                }
            }
            return outList;
        }

        private static List<Vector2Int> BuildHardBlockCells(int originX, int originY, DecorationRule rule, Vector2Int anchor)
        {
            int extra = (rule.PostSpawnBlockedCells?.Length ?? 0) + (rule.ExtraBlockOffsets?.Length ?? 0);
            var list = new List<Vector2Int>(1 + extra) { anchor };

            if (rule.PostSpawnBlockedCells != null)
            {
                for (int i = 0; i < rule.PostSpawnBlockedCells.Length; ++i)
                {
                    var bc = rule.PostSpawnBlockedCells[i];
                    list.Add(new Vector2Int(originX + bc.x, originY + bc.y));
                }
            }

            if (rule.ExtraBlockOffsets != null)
            {
                for (int i = 0; i < rule.ExtraBlockOffsets.Length; ++i)
                {
                    var off = rule.ExtraBlockOffsets[i];
                    list.Add(anchor + off);
                }
            }

            return list;
        }

        private IEnumerable<IDecorationMatch> ResolveCandidates(List<Candidate> candidates, TileTypeSO[,] tiles)
        {
            int rows = tiles.GetLength(0), cols = tiles.GetLength(1);
            var budgets = new RuleBudgetContext();

            candidates.Sort((a, b) =>
            {
                int p = b.Priority.CompareTo(a.Priority);
                if (p != 0) return p;
                return a.Tie.CompareTo(b.Tie);
            });

            var hardBlocked = new HashSet<Vector2Int>();

            var blockedByCategory = new Dictionary<string, HashSet<Vector2Int>>();
            var blockedByPrefab = new Dictionary<GameObject, HashSet<Vector2Int>>();
            var personalKeepout = new HashSet<Vector2Int>();

            var results = new List<IDecorationMatch>(candidates.Count);

            bool InBounds(Vector2Int p) => p.x >= 0 && p.x < cols && p.y >= 0 && p.y < rows;

            foreach (var c in candidates)
            {
                bool conflict = false;
                for (int i = 0; i < c.HardCells.Count; ++i)
                {
                    var cell = c.HardCells[i];
                    if (!InBounds(cell) || hardBlocked.Contains(cell)) { conflict = true; break; }
                }
                if (conflict) continue;

                if (!string.IsNullOrEmpty(c.Rule.CategoryId) && c.Rule.SameCategoryMinDistance > 0)
                {
                    if (IsBlocked(blockedByCategory, c.Rule.CategoryId, c.Anchor)) continue;
                }
                if (c.Rule.SamePrefabMinDistance > 0)
                {
                    if (IsBlocked(blockedByPrefab, c.Rule.Prefab, c.Anchor)) continue;
                }
                if (c.Rule.BlockRadiusDistance > 0)
                {
                    if (personalKeepout.Contains(c.Anchor)) continue;
                }

                if (!_spaceMap.TryGetValue(c.Anchor, out var spaceId)) continue;
                if (!budgets.TryConsume(c.Rule, spaceId)) continue;

                for (int i = 0; i < c.HardCells.Count; ++i)
                    hardBlocked.Add(c.HardCells[i]);

                if (!string.IsNullOrEmpty(c.Rule.CategoryId) && c.Rule.SameCategoryMinDistance > 0)
                    AddRadiusBlock(blockedByCategory, c.Rule.CategoryId, c.Anchor, c.Rule.SameCategoryMinDistance, rows, cols);

                if (c.Rule.SamePrefabMinDistance > 0)
                    AddRadiusBlock(blockedByPrefab, c.Rule.Prefab, c.Anchor, c.Rule.SamePrefabMinDistance, rows, cols);

                if (c.Rule.BlockRadiusDistance > 0)
                    AddRadiusBlock(personalKeepout, c.Anchor, c.Rule.BlockRadiusDistance, rows, cols);

                results.Add(new DecorationMatch
                {
                    SpawnPosition = c.Anchor,
                    ObjectToSpawn = c.Rule.Prefab,
                    Offset = c.Rule.SpawnPositionOffset,
                    Rotation = c.Rule.SpawnRotation,
                    Scale = c.Rule.SpawnScale
                });
            }

            return results;
        }

        private IEnumerable<IDecorationMatch> GenerateCoverageMatches(TileTypeSO[,] tiles, HashSet<Vector2Int> occupied)
        {
            var matches = new List<IDecorationMatch>();
            int rows = tiles.GetLength(0), cols = tiles.GetLength(1);

            var candidates = new List<Vector2Int>();

            for (int y = 0; y < rows; ++y)
            {
                for (int x = 0; x < cols; ++x)
                {
                    var t = tiles[y, x];
                    if (_coverage != null && _coverage.CoverageSupportRule.MatchesTile(t)) continue;
                    if (!HasSupportNeighbor(y, x, tiles)) continue;

                    candidates.Add(new Vector2Int(x, y));
                }
            }

            // Deterministic ordering (y,x) then stride by spacing
            candidates.Sort((a, b) => a.y != b.y ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));

            for (int i = 0; i < candidates.Count; i += Mathf.Max(1, _coverage.CoverageSpacing))
            {
                var p = candidates[i];
                if (occupied.Contains(p)) continue;

                occupied.Add(p);
                matches.Add(new DecorationMatch
                {
                    SpawnPosition = p,
                    ObjectToSpawn = _coverage.CoveragePrefab,
                    Offset = Vector3.zero,
                    Rotation = Vector3.zero,
                    Scale = Vector3.one
                });
            }

            return matches;
        }

        private bool HasSupportNeighbor(int y, int x, TileTypeSO[,] tiles)
        {
            if (_coverage == null) return true;
            int rows = tiles.GetLength(0), cols = tiles.GetLength(1);

            if (_coverage.CoverageUse4Neighbors)
            {
                int[] dx = { 0, 0, -1, 1 };
                int[] dy = { -1, 1, 0, 0 };
                for (int k = 0; k < 4; ++k)
                {
                    int ny = y + dy[k], nx = x + dx[k];
                    if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
                        if (_coverage.CoverageSupportRule.MatchesTile(tiles[ny, nx])) return true;
                }
            }
            else
            {
                for (int ny = y - 1; ny <= y + 1; ++ny)
                    for (int nx = x - 1; nx <= x + 1; ++nx)
                    {
                        if (nx == x && ny == y) continue;
                        if (nx >= 0 && nx < tiles.GetLength(1) && ny >= 0 && ny < tiles.GetLength(0))
                            if (_coverage.CoverageSupportRule.MatchesTile(tiles[ny, nx])) return true;
                    }
            }
            return false;
        }

        private HashSet<Vector2Int> BuildOccupiedSetFromResults(List<IDecorationMatch> matches)
        {
            var set = new HashSet<Vector2Int>();
            foreach (var m in matches)
                set.Add(m.SpawnPosition);
            return set;
        }

        private sealed class RuleBudgetContext
        {
            private struct State
            {
                public int Total;
                public Dictionary<string, int> PerSpace;
            }
            private readonly Dictionary<DecorationRule, State> _states = new();

            public bool TryConsume(DecorationRule rule, string spaceId)
            {
                var p = Mathf.Clamp01(rule.ChanceToPlace);
                if (RandomSingleton.Instance.NextFloat() >= p) return false;

                if (!_states.TryGetValue(rule, out var st))
                    st = new State { Total = 0, PerSpace = new Dictionary<string, int>() };

                st.PerSpace.TryGetValue(spaceId, out var usedInSpace);

                if (rule.ChancesPerMap > 0 && st.Total >= rule.ChancesPerMap) return false;
                if (rule.ChancesPerSpace > 0 && usedInSpace >= rule.ChancesPerSpace) return false;

                st.Total++;
                st.PerSpace[spaceId] = usedInSpace + 1;
                _states[rule] = st;
                return true;
            }
        }

        private void Init(Dictionary<Vector2Int, string> spaceMap)
        {
            _dimensionsToDecorationRules = new Dictionary<Vector2Int, List<DecorationRule>>();
            _ruleIndex = new Dictionary<DecorationRule, int>();
            _spaceMap = spaceMap;
            _maxWidth = _maxHeight = 0;
            _seed = RandomSingleton.Instance.GetSeed();
        }

        private void InitPatternData()
        {
            _maxWidth = _maxHeight = 0;
            int idx = 0;

            foreach (var rule in _decorationRuleset.DecorationRules)
            {
                rule.Init();          // clears caches
                _ruleIndex[rule] = idx++;

                var pattern = rule.MatchingPattern2D;
                int h = pattern.GetLength(0), w = pattern.GetLength(1);

                if (w > _maxWidth) _maxWidth = w;
                if (h > _maxHeight) _maxHeight = h;

                var dims = new Vector2Int(w, h);
                if (!_dimensionsToDecorationRules.TryGetValue(dims, out var list))
                    _dimensionsToDecorationRules[dims] = list = new List<DecorationRule>();
                list.Add(rule);
            }
        }

        private static bool IsBlocked<TKey>(Dictionary<TKey, HashSet<Vector2Int>> map, TKey key, Vector2Int pos)
        {
            return map.TryGetValue(key, out var set) && set.Contains(pos);
        }

        private static void AddRadiusBlock<TKey>(
            Dictionary<TKey, HashSet<Vector2Int>> map, TKey key,
            Vector2Int center, int radius, int rows, int cols)
        {
            if (radius <= 0) return;
            if (!map.TryGetValue(key, out var set))
                map[key] = set = new HashSet<Vector2Int>();

            foreach (var p in ChebyshevDisk(center, radius, rows, cols))
                set.Add(p);
        }

        private static void AddRadiusBlock(HashSet<Vector2Int> set, Vector2Int center, int radius, int rows, int cols)
        {
            if (radius <= 0) return;
            foreach (var p in ChebyshevDisk(center, radius, rows, cols))
                set.Add(p);
        }

        private static IEnumerable<Vector2Int> ChebyshevDisk(Vector2Int c, int r, int rows, int cols)
        {
            for (int dy = -r; dy <= r; ++dy)
            {
                int y = c.y + dy;
                if (y < 0 || y >= rows) continue;
                for (int dx = -r; dx <= r; ++dx)
                {
                    int x = c.x + dx;
                    if (x < 0 || x >= cols) continue;
                    yield return new Vector2Int(x, y);
                }
            }
        }

        private static int StableHash(int seed, int ax, int ay, int ruleIndex)
        {
            unchecked
            {
                uint h = (uint)seed;
                h = (h * 16777619u) ^ (uint)ax;
                h = (h * 16777619u) ^ (uint)ay;
                h = (h * 16777619u) ^ (uint)ruleIndex;

                h ^= (h >> 16);
                h *= 0x7feb352du;
                h ^= (h >> 15);
                h *= 0x846ca68bu;
                h ^= (h >> 16);

                return (int)h;
            }
        }

        private class DecorationMatch : IDecorationMatch
        {
            public Vector2Int SpawnPosition { get; set; }
            public GameObject ObjectToSpawn { get; set; }
            public Vector3 Offset { get; set; }
            public Vector3 Rotation { get; set; }
            public Vector3 Scale { get; set; }
        }
    }

}