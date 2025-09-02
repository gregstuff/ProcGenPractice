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
            var learner = new Accumulator();
            List<GameObject> roots = new List<GameObject>();
            try
            {
                foreach (var assetGO in Selection.GetFiltered<GameObject>(SelectionMode.Assets))
                {
                    var assetPath = AssetDatabase.GetAssetPath(assetGO);
                    if (string.IsNullOrEmpty(assetPath)) continue;

                    var levelRoot = PrefabUtility.LoadPrefabContents(assetPath);

                    roots.Add(levelRoot);
                }

                if (roots.Count == 0) { Debug.LogError("Select 1+ exemplar roots."); return; }

                foreach (var root in roots)
                {
                    var prov = root.GetComponent<WFCLevelExemplar>();
                    if (prov == null || !prov.TryGetGrid(out var tiles, out var rows, out var cols, out var cellSize))
                    {
                        Debug.LogWarning($"Skip '{root.name}' (no grid).");
                        continue;
                    }

                    for (int r = 0; r < rows; r++)
                    {
                        for (int c = 0; c < cols; c++)
                        {
                            learner.CountAdj(tiles, r, c);
                            learner.Count2x2(tiles, r, c);
                        }
                    }

                    var markers = root.GetComponentsInChildren<WFCPrefabExemplar>(true);

                    var perPrefabPositions = new Dictionary<string, List<Vector3>>();

                    foreach (var marker in markers)
                    {
                        string pid = marker.Name;

                        var (atype, x, y, uv) = ResolveCellPositionAnchor(marker.transform.position, root.transform.position, cellSize);

                        // ensures resolved prefab cells are not off grid
                        x = Mathf.Clamp(x, 0, cols - 1);
                        y = Mathf.Clamp(y, 0, rows - 1);

                        string centerKey = tiles[y, x].Key;

                        switch (atype)
                        {
                            case AnchorType.Cell:
                                learner.CountPrefabCell(pid, centerKey);
                                learner.AddCellOffset(pid, uv);
                                break;

                            case AnchorType.EdgeH:
                                {
                                    string northCellKey = y > 0 ? tiles[y - 1, x].Key : "None";
                                    learner.CountPrefabEdge(pid, true, northCellKey, centerKey);
                                    learner.AddEdgeT(pid, true, uv.x);
                                    break;
                                }
                            case AnchorType.EdgeV:
                                {
                                    string westCellKey = x > 0 ? tiles[y, x - 1].Key : "None";
                                    learner.CountPrefabEdge(pid, false, westCellKey, centerKey);
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

                        // get rotation bins and calculate correct bin for prefab
                        int bins = Mathf.Max(4, marker.RotationBins);
                        float yaw = Mathf.Repeat(marker.transform.eulerAngles.y - root.transform.eulerAngles.y, 360f);
                        int bin = Mathf.Clamp(Mathf.FloorToInt(yaw / (360f / bins)), 0, bins - 1);
                        learner.AddRotationSample(pid, bins, bin);


                        Vector3 s = marker.transform.lossyScale;
                        learner.AddScaleSample(pid, marker.UniformScale, s);

                        // if foot print is manual, use that else calculate it
                        var footprint = (marker.FootprintMode == FootprintSource.ManualCells && marker.ManualFootprintCells != null && marker.ManualFootprintCells.Length > 0)
                            ? new HashSet<Vector2Int>(marker.ManualFootprintCells)
                            : ComputeFootprintCells(marker.transform, root.transform.position, cellSize, rows, cols, x, y);
                        learner.AddFootprintSample(pid, atype, footprint);


                        if (!perPrefabPositions.TryGetValue(pid, out var list))
                            perPrefabPositions[pid] = list = new List<Vector3>();
                        list.Add(marker.transform.position);
                    }

                    // work out spacing between prefabs
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
            finally
            {
                foreach (var root in roots)
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
        }

        private static (AnchorType type, int i, int j, Vector2 uv)
            ResolveCellPositionAnchor(Vector3 worldPos, Vector3 gridOrigin, float cellSize)
        {
            // Step 1: Convert world position into local grid space
            Vector3 localPos3D = worldPos - gridOrigin;

            // Step 2: Find which cell we are in
            int cellX = Mathf.FloorToInt(localPos3D.x / cellSize);
            int cellZ = Mathf.FloorToInt(localPos3D.z / cellSize);

            // Step 3: Find fractional position within the cell (0..1 range)
            float u = (localPos3D.x / cellSize) - cellX;
            float v = (localPos3D.z / cellSize) - cellZ;

            // Step 4: Define "bands" near edges and corners
            const float edgeThreshold = 0.18f;

            bool nearLeft = u < edgeThreshold;
            bool nearRight = (1f - u) < edgeThreshold;
            bool nearBottom = v < edgeThreshold;
            bool nearTop = (1f - v) < edgeThreshold;

            // Step 5: Classify based on which edges we're near

            // Near both horizontal and vertical edges → Corner
            if ((nearLeft || nearRight) && (nearBottom || nearTop))
            {
                return (AnchorType.Corner, cellX, cellZ, new Vector2(u, v));
            }

            // Near vertical edge (left or right)
            if (nearLeft || nearRight)
            {
                int edgeCellX = nearLeft ? cellX : cellX + 1; // shift to the correct edge cell
                return (AnchorType.EdgeV, edgeCellX, cellZ, new Vector2(v, 0f));
            }

            // Near horizontal edge (bottom or top)
            if (nearBottom || nearTop)
            {
                int edgeCellZ = nearBottom ? cellZ : cellZ + 1; // shift to the correct edge cell
                return (AnchorType.EdgeH, cellX, edgeCellZ, new Vector2(u, 0f));
            }

            // Otherwise → Inside cell
            return (AnchorType.Cell, cellX, cellZ, new Vector2(u, v));
        }

        /*
         * Based on the render bounds for this prefab, what cells does it cover?
         * Returns relative, not absolute cell positions
         */
        private static HashSet<Vector2Int> ComputeFootprintCells(Transform t, Vector3 origin, float cellSize, int rows, int cols, int baseX, int baseY)
        {
            var covered = new HashSet<Vector2Int>();
            var rends = t.GetComponentsInChildren<Renderer>(true);
            foreach (var r in rends)
            {
                var b = r.bounds;
                int i0 = Mathf.Clamp(Mathf.FloorToInt((b.min.x - origin.x) / cellSize), 0, cols - 1);
                int i1 = Mathf.Clamp(Mathf.FloorToInt((b.max.x - origin.x) / cellSize), 0, cols - 1);
                int j0 = Mathf.Clamp(Mathf.FloorToInt((b.min.z - origin.z) / cellSize), 0, rows - 1);
                int j1 = Mathf.Clamp(Mathf.FloorToInt((b.max.z - origin.z) / cellSize), 0, rows - 1);
                for (int i = i0; i <= i1; i++)
                    for (int j = j0; j <= j1; j++)
                        covered.Add(new Vector2Int(i, j));
            }
            // Convert to relative offsets w.r.t base cell
            var rel = new HashSet<Vector2Int>();
            foreach (var p in covered) rel.Add(new Vector2Int(p.x - baseX, p.y - baseY));
            if (rel.Count == 0) rel.Add(Vector2Int.zero);
            return rel;
        }
#endif

    }
}