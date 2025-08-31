// ProcGenSys.WFC.Marker/WFCPrefabExemplar.cs
using UnityEngine;

namespace ProcGenSys.WFC.Marker
{
    public enum AnchorHint { Auto, Cell, EdgeH, EdgeV, Corner, Surface }
    public enum FootprintSource { AutoBounds, ManualCells }

    public class WFCPrefabExemplar : MonoBehaviour
    {
        public string PrefabId;

        public AnchorHint Anchor = AnchorHint.Auto;

        public FootprintSource FootprintMode = FootprintSource.AutoBounds;

        public Vector2Int[] ManualFootprintCells;

        public int RotationBins = 16;

        public bool UniformScale = true;
    }
}
