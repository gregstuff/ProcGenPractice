using ProcGenSys.Common.Enum;
using System;
using UnityEngine;

namespace ProcGenSys.WFC.Bundle
{
    public class WFCModelBundle : ScriptableObject
    {
        public string[] TileIds;

        public AdjacencyPerDir[] Adjacency;

        public PrefabEntry[] Prefabs;

        [Serializable]
        public class AdjacencyPerDir
        {
            public Direction Dir;
            public float[] Weights; // flattened [A*TileCount + B]
            public int TileCount;
        }

        [Serializable]
        public class PrefabEntry
        {
            public string PrefabId;

            public int CellCount, EdgeHCount, EdgeVCount, CornerCount, SurfaceCount;

            public KeyWeight[] CellOnTile;
            public PairWeight[] EdgeH_AB;
            public PairWeight[] EdgeV_AB;

            public Vector2 AvgCellUV;
            public float AvgEdgeH_T;
            public float AvgEdgeV_T;

            public int RotationBins;
            public int[] RotationHist;

            public bool UniformScale;
            public Vector3 ScaleMean;
            public Vector3 ScaleStdDev;

            public FootprintVariant[] Footprints;

            public float SpacingMedian;
            public float SpacingP10;
        }

        [Serializable] public struct KeyWeight { public string Key; public int Count; }
        [Serializable] public struct PairWeight { public string Pair; public int Count; }

        [Serializable]
        public class FootprintVariant
        {
            public AnchorType Anchor;
            public Vector2Int[] Cells;
            public int Count;
        }
    }
}
