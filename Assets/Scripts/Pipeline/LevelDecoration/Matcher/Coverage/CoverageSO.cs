using ProcGenSys.Pipeline.LevelDecoration.Matcher.Rule;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher.Coverage
{
    public class CoverageSO : ScriptableObject
    {
        [SerializeField] public GameObject CoveragePrefab;
        [SerializeField] public int CoverageSpacing = 4;
        [Tooltip("Cells eligible to host coverage objects (e.g., Floor).")]
        [SerializeField] public TileMatchingRuleSO CoveragePlacementRule;
        [Tooltip("Neighbor cell rule that must be adjacent (e.g., Wall).")]
        [SerializeField] public TileMatchingRuleSO CoverageSupportRule;
        [Tooltip("Use 4-neighborhood for support detection; off = 8-neighborhood.")]
        [SerializeField] public bool CoverageUse4Neighbors = true;
    }
}