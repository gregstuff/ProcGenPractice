using ProcGenSys.Common.LevelBundle;
using ProcGenSys.WFC.Bundle;
using ProcGenSys.Utilities.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProcGenSys.Service.Util;
using ProcGenSys.WFC;

namespace ProcGenSys.Pipeline.LevelDecoration
{
    [CreateAssetMenu(menuName = "ProcGen/Decorators/WFC Texture Preview Decorator")]
    public class WFCDebugTextureDecoratorSO : DecoratorSO
    {
        private static readonly string RENDERER_TAG = "DecorationTexture";

        [Header("Inputs")]
        [SerializeField] private WFCModelBundle _wfcModel;
        [SerializeField] private Renderer _decorationRenderer;

        private Renderer _renderer;
        private int _cellW;
        private int _cellH;
        private float _mapScale;
        private float _resolvedW;
        private float _resolvedH;
        private bool[,] _blockedGrid;
        private string[,] _tileNames;

        public override void ApplyDecorations(ICapabilityProvider level)
        {
            if (_wfcModel == null)
            {
                Debug.LogError("WFC Texture Preview: missing WFCModelBundle.");
                return;
            }

            GetOrCreateRenderer();
            InitLevel(level);

            var placements = GenerateCellPlacements();

            DrawLayout(placements);
        }

        private void GetOrCreateRenderer()
        {
            var existing = GameObject.FindGameObjectWithTag(RENDERER_TAG);
            if (existing != null && existing.TryGetComponent(out Renderer r))
            {
                _renderer = r;
                return;
            }
            _renderer = ObjectSpawnerSingleton.Instance.Spawn(_decorationRenderer);
        }

        private void InitLevel(ICapabilityProvider level)
        {
            if (!level.TryGet<Dimensions>(out var dims) ||
                !level.TryGet<Scale>(out var scale) ||
                !level.TryGet<BlockMask>(out var mask) ||
                !level.TryGet<TileLayer>(out var tileLayer) ||
                tileLayer.Tiles == null)
            {
                throw new Exception("WFC Texture Preview: level missing Dimensions/Scale/BlockMask/TileLayer.");
            }

            _cellW = dims.MapDimensions.x;
            _cellH = dims.MapDimensions.y;
            _mapScale = scale.MapScale;
            _resolvedW = _cellW * _mapScale;
            _resolvedH = _cellH * _mapScale;

            _blockedGrid = mask.Mask;

            _tileNames = new string[_cellH, _cellW];
            for (int y = 0; y < _cellH; y++)
            {
                for (int x = 0; x < _cellW; x++)
                {
                    var t = tileLayer.Tiles[y, x];
                    _tileNames[y, x] = t ? t.name : "None";
                }
            }
        }

        private sealed class Placement
        {
            public string PrefabId;
            public Vector2Int Anchor;
            public Vector2Int[] FootprintCells;
        }

        private List<Placement> GenerateCellPlacements()
        {
            var result = new List<Placement>();
            if (_wfcModel.Prefabs == null || _wfcModel.Prefabs.Length == 0)
                return result;

            var tileToCandidates = new Dictionary<string, List<(WFCModelBundle.PrefabEntry p, int weight)>>();
            foreach (var p in _wfcModel.Prefabs)
            {
                if (p.CellOnTile == null) continue;
                foreach (var kt in p.CellOnTile)
                {
                    if (!tileToCandidates.TryGetValue(kt.Key, out var list))
                        list = tileToCandidates[kt.Key] = new List<(WFCModelBundle.PrefabEntry, int)>();
                    if (kt.Count > 0)
                        list.Add((p, kt.Count));
                }
            }

            var prefabFootprints = new Dictionary<string, Vector2Int[]>();
            foreach (var p in _wfcModel.Prefabs)
            {
                Vector2Int[] cells = null;
                if (p.Footprints != null && p.Footprints.Length > 0)
                {
                    var cellVariants = p.Footprints
                        .Where(f => f.Anchor == AnchorType.Cell && f.Cells != null && f.Cells.Length > 0)
                        .OrderByDescending(f => f.Count)
                        .ToList();

                    if (cellVariants.Count() > 0)
                        cells = cellVariants[0].Cells;
                }
                prefabFootprints[p.PrefabId] = cells ?? new[] { Vector2Int.zero };
            }

            var prefabSpacing = new Dictionary<string, int>();
            foreach (var p in _wfcModel.Prefabs)
            {
                int r = Mathf.Clamp(Mathf.CeilToInt(p.SpacingP10), 0, 8);
                prefabSpacing[p.PrefabId] = r;
            }

            var occupied = new bool[_cellH, _cellW];

            var perPrefabPositions = new Dictionary<string, List<Vector2Int>>();

            for (int y = 0; y < _cellH; y++)
            {
                for (int x = 0; x < _cellW; x++)
                {
                    if (_blockedGrid[y, x]) continue;

                    string tile = _tileNames[y, x];
                    if (!tileToCandidates.TryGetValue(tile, out var candidates) || candidates.Count == 0)
                        continue;

                    var best = candidates.OrderByDescending(c => c.weight).ToList();

                    foreach (var (p, weight) in best)
                    {
                        int radius = prefabSpacing[p.PrefabId];
                        if (radius > 0 && ViolatesSpacing(p.PrefabId, new Vector2Int(x, y), radius, perPrefabPositions))
                            continue;

                        var cells = prefabFootprints[p.PrefabId];
                        if (!FootprintFits(new Vector2Int(x, y), cells, occupied))
                            continue;

                        result.Add(new Placement
                        {
                            PrefabId = p.PrefabId,
                            Anchor = new Vector2Int(x, y),
                            FootprintCells = cells
                        });

                        foreach (var rc in cells)
                        {
                            int cx = x + rc.x;
                            int cy = y + rc.y;
                            if (cx >= 0 && cx < _cellW && cy >= 0 && cy < _cellH)
                                occupied[cy, cx] = true;
                        }

                        if (!perPrefabPositions.TryGetValue(p.PrefabId, out var list))
                            perPrefabPositions[p.PrefabId] = list = new List<Vector2Int>();
                        list.Add(new Vector2Int(x, y));

                        break;
                    }
                }
            }

            return result;
        }

        private static bool FootprintFits(Vector2Int anchor, Vector2Int[] rel, bool[,] occupied)
        {
            int w = occupied.GetLength(1);
            int h = occupied.GetLength(0);
            for (int i = 0; i < rel.Length; i++)
            {
                int cx = anchor.x + rel[i].x;
                int cy = anchor.y + rel[i].y;
                if (cx < 0 || cy < 0 || cx >= w || cy >= h) return false;
                if (occupied[cy, cx]) return false;
            }
            return true;
        }

        private static bool ViolatesSpacing(string prefabId, Vector2Int pos, int radius, Dictionary<string, List<Vector2Int>> perPrefab)
        {
            if (radius <= 0) return false;
            if (!perPrefab.TryGetValue(prefabId, out var list) || list.Count == 0) return false;

            int r2 = radius * radius;
            for (int i = 0; i < list.Count; i++)
            {
                var d = list[i] - pos;
                if (d.x * d.x + d.y * d.y <= r2) return true;
            }
            return false;
        }

        private void DrawLayout(List<Placement> placements)
        {
            var colorMap = new Dictionary<string, Color>();
            foreach (var pid in placements.Select(p => p.PrefabId).Distinct())
                colorMap[pid] = ColorFromString(pid);

            Debug.Log("WFC Preview — Prefab color legend:");
            foreach (var kv in colorMap.OrderBy(k => k.Key))
            {
                var hex = ColorUtility.ToHtmlStringRGB(kv.Value);
                Debug.Log($"{kv.Key} -> #{hex}");
            }

            _renderer.transform.localScale = new Vector3(_cellH, _cellW, 1);
            var tex = (Texture2D)_renderer.sharedMaterial.mainTexture;
            tex.Reinitialize(_cellW, _cellH);
            tex.FillWithColor(Color.black);

            for (int y = 0; y < _cellH; y++)
            {
                for (int x = 0; x < _cellW; x++)
                {
                    if (!_blockedGrid[y, x])
                        tex.SetPixel(x, y, Color.white);
                }
            }

            foreach (var pl in placements)
            {
                var col = colorMap[pl.PrefabId];
                foreach (var rc in pl.FootprintCells)
                {
                    int cx = pl.Anchor.x + rc.x;
                    int cy = pl.Anchor.y + rc.y;
                    if (cx >= 0 && cx < _cellW && cy >= 0 && cy < _cellH)
                        tex.SetPixel(cx, cy, col);
                }
            }

            tex.SaveAsset();

            _renderer.transform.localScale = new Vector3(_resolvedW, _resolvedH, 1);
            _renderer.transform.position = new Vector3((_resolvedW / 2f) - _mapScale, 1f, (_resolvedH / 2f) - _mapScale);
            _renderer.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }

        private static Color ColorFromString(string s)
        {
            unchecked
            {
                uint hash = 2166136261;
                for (int i = 0; i < s.Length; i++)
                {
                    hash ^= s[i];
                    hash *= 16777619;
                }
                float hue = (hash % 360u) / 360f;

                return Color.HSVToRGB(hue, 0.65f, 0.95f);
            }
        }
    }
}
