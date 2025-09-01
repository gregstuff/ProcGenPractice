// ProcGenSys.WFC.Marker/WFCPrefabExemplar.cs
using UnityEngine;

namespace ProcGenSys.WFC.Marker
{



    public class WFCPrefabExemplar : MonoBehaviour
    {
        public string PrefabId;

        public AnchorHint Anchor = AnchorHint.Auto;

        public FootprintSource FootprintMode = FootprintSource.AutoBounds;

        public Vector2Int[] ManualFootprintCells;

        public int RotationBins = 16;

        public bool UniformScale = true;

        public string Name => string.IsNullOrWhiteSpace(PrefabId) ? Clean(gameObject.name) : PrefabId;

        private static string Clean(string n) => n.Replace("(Clone)", "").Trim();
    }
}
