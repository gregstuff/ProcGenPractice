#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ProcGenSys.WFC.Marker;
using ProcGenSys.WFC.Bundle;

namespace ProcGenSys.WFC.Learner
{
    public static class WFCExemplarLearner
    {
        [MenuItem("ProcGen/WFC/Learn From Selected Exemplars")]
        public static void LearnFromSelection()
        {
            int processed = 0;
            var learner = new Accumulator();
            List<GameObject> roots = new List<GameObject>();

            foreach (var assetGO in Selection.GetFiltered<GameObject>(SelectionMode.Assets))
            {
                var assetPath = AssetDatabase.GetAssetPath(assetGO);
                if (string.IsNullOrEmpty(assetPath)) continue;

                var tempRoot = PrefabUtility.LoadPrefabContents(assetPath);
                try
                {
                    processed += ProcessOne(tempRoot, learner);
                    roots.Add(tempRoot);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(tempRoot);
                }
            }

            if (processed == 0)
            {
                EditorUtility.DisplayDialog("WFC Learner", "No valid exemplars selected (scene or prefab).", "OK");
                return;
            }

            if (roots == null || roots.Count == 0) { Debug.LogError("Select 1+ exemplar roots."); return; }

            foreach (var root in roots)
            {
                var prov = root.GetComponent<WFCLevelExemplar>();
                if (prov == null || !prov.TryGetGrid(out var tiles, out var rows, out var cols, out var cellSize))
                {
                    Debug.LogWarning($"Skip '{root.name}' (no grid).");
                    continue;
                }

                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        var t = tiles[r, c]; string id = t ? t.name : "None";
                        learner.TouchTile(id);
                    }

                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        string a = tiles[r, c] ? tiles[r, c].name : "None";
                        learner.CountAdj(a, Direction.North, r > 0 ? (tiles[r - 1, c] ? tiles[r - 1, c].name : "None") : null);
                        learner.CountAdj(a, Direction.East, c < cols - 1 ? (tiles[r, c + 1] ? tiles[r, c + 1].name : "None") : null);
                        learner.CountAdj(a, Direction.South, r < rows - 1 ? (tiles[r + 1, c] ? tiles[r + 1, c].name : "None") : null);
                        learner.CountAdj(a, Direction.West, c > 0 ? (tiles[r, c - 1] ? tiles[r, c - 1].name : "None") : null);
                    }

                var markers = root.GetComponentsInChildren<WFCPrefabExemplar>(true);

                var perPrefabPositions = new Dictionary<string, List<Vector3>>();

                foreach (var m in markers)
                {
                    string pid = string.IsNullOrWhiteSpace(m.PrefabId) ? Clean(m.gameObject.name) : m.PrefabId;

                    var (atype, i, j, uv) = Classify(m.transform.position, root.transform.position, cellSize);
                    i = Mathf.Clamp(i, 0, cols - 1);
                    j = Mathf.Clamp(j, 0, rows - 1);

                    string center = tiles[j, i] ? tiles[j, i].name : "None";

                    // Context counts
                    switch (atype)
                    {
                        case AnchorType.Cell:
                            learner.CountPrefabCell(pid, center);
                            learner.AddCellOffset(pid, uv);
                            break;

                        case AnchorType.EdgeH:
                            {
                                string a = j > 0 ? (tiles[j - 1, i] ? tiles[j - 1, i].name : "None") : "None";
                                string b = tiles[j, i] ? tiles[j, i].name : "None";
                                learner.CountPrefabEdge(pid, true, a, b);
                                learner.AddEdgeT(pid, true, uv.x);
                                break;
                            }
                        case AnchorType.EdgeV:
                            {
                                string a = i > 0 ? (tiles[j, i - 1] ? tiles[j, i - 1].name : "None") : "None";
                                string b = tiles[j, i] ? tiles[j, i].name : "None";
                                learner.CountPrefabEdge(pid, false, a, b);
                                learner.AddEdgeT(pid, false, uv.x);
                                break;
                            }
                        case AnchorType.Corner:
                            learner.CountPrefabCorner(pid);
                            break;
                        default:
                            learner.CountPrefabSurface(pid);
                            break;
                    }

                    int bins = Mathf.Max(4, m.RotationBins);
                    float yaw = Mathf.Repeat(m.transform.eulerAngles.y - root.transform.eulerAngles.y, 360f);
                    int bin = Mathf.Clamp(Mathf.FloorToInt(yaw / (360f / bins)), 0, bins - 1);
                    learner.AddRotationSample(pid, bins, bin);

                    Vector3 s = m.transform.lossyScale;
                    learner.AddScaleSample(pid, m.UniformScale, s);

                    var footprint = (m.FootprintMode == FootprintSource.ManualCells && m.ManualFootprintCells != null && m.ManualFootprintCells.Length > 0)
                        ? new HashSet<Vector2Int>(m.ManualFootprintCells)
                        : ComputeFootprintCells(m.transform, root.transform.position, cellSize, rows, cols, i, j);
                    learner.AddFootprintSample(pid, atype, footprint);

                    if (!perPrefabPositions.TryGetValue(pid, out var list))
                        perPrefabPositions[pid] = list = new List<Vector3>();
                    list.Add(m.transform.position);
                }

                foreach (var kv in perPrefabPositions)
                {
                    var pid = kv.Key;
                    var pts = kv.Value;
                    if (pts.Count < 2) continue;
                    for (int a = 0; a < pts.Count; a++)
                    {
                        float best = float.PositiveInfinity;
                        for (int b = 0; b < pts.Count; b++)
                        {
                            if (a == b) continue;
                            float d = Vector3.Distance(pts[a], pts[b]) / cellSize;
                            if (d < best) best = d;
                        }
                        if (best < float.PositiveInfinity)
                            learner.AddSpacingSample(pid, best);
                    }
                }
            }

            var asset = ScriptableObject.CreateInstance<WFCModelBundle>();
            learner.FillAsset(asset);

            var path = EditorUtility.SaveFilePanelInProject("Save WFC Model Bundle", "WFCModelBundle", "asset", "");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                EditorGUIUtility.PingObject(asset);
                Debug.Log($"Saved: {path}");
            }
        }

        private static int ProcessOne(GameObject root, Accumulator learner)
        {
            var prov = root.GetComponent<WFCLevelExemplar>();
            if (prov == null || !prov.TryGetGrid(out var tiles, out var rows, out var cols, out var cellSize))
            {
                Debug.LogWarning($"Skip '{root.name}' — no DungeonLevelBundleProvider or grid.");
                return 0;
            }

            return 1;
        }

        private static string Clean(string n) => n.Replace("(Clone)", "").Trim();

        private enum Dir { N = 0, E = 1, S = 2, W = 3 }

        private static (AnchorType type, int i, int j, Vector2 uv)
        Classify(Vector3 worldPos, Vector3 origin, float cell)
        {
            var local = worldPos - origin;
            int i = Mathf.FloorToInt(local.x / cell);
            int j = Mathf.FloorToInt(local.z / cell);
            float u = (local.x / cell) - i;
            float v = (local.z / cell) - j;

            const float edgeBand = 0.18f, cornerBand = 0.12f;
            bool nearL = u < edgeBand, nearR = (1f - u) < edgeBand;
            bool nearB = v < edgeBand, nearT = (1f - v) < edgeBand;

            if ((nearL || nearR) && (nearB || nearT)) return (AnchorType.Corner, i, j, new Vector2(u, v));
            if (nearL || nearR) return (AnchorType.EdgeV, nearL ? i : i + 1, j, new Vector2(v, 0f));
            if (nearB || nearT) return (AnchorType.EdgeH, i, nearB ? j : j + 1, new Vector2(u, 0f));
            return (AnchorType.Cell, i, j, new Vector2(u, v));
        }

        private static HashSet<Vector2Int> ComputeFootprintCells(Transform t, Vector3 origin, float cell, int rows, int cols, int baseI, int baseJ)
        {
            var covered = new HashSet<Vector2Int>();
            var rends = t.GetComponentsInChildren<Renderer>(true);
            foreach (var r in rends)
            {
                var b = r.bounds;
                int i0 = Mathf.Clamp(Mathf.FloorToInt((b.min.x - origin.x) / cell), 0, cols - 1);
                int i1 = Mathf.Clamp(Mathf.FloorToInt((b.max.x - origin.x) / cell), 0, cols - 1);
                int j0 = Mathf.Clamp(Mathf.FloorToInt((b.min.z - origin.z) / cell), 0, rows - 1);
                int j1 = Mathf.Clamp(Mathf.FloorToInt((b.max.z - origin.z) / cell), 0, rows - 1);
                for (int i = i0; i <= i1; i++)
                    for (int j = j0; j <= j1; j++)
                        covered.Add(new Vector2Int(i, j));
            }
            // Convert to relative offsets w.r.t base cell
            var rel = new HashSet<Vector2Int>();
            foreach (var p in covered) rel.Add(new Vector2Int(p.x - baseI, p.y - baseJ));
            if (rel.Count == 0) rel.Add(Vector2Int.zero);
            return rel;
        }
#endif

    }
}