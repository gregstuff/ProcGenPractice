using DungeonGeneration.Map.Model.Rooms;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGeneration.Map.SO
{
    [CreateAssetMenu(fileName = "Generated level layout", menuName = "Custom/Procedural Generation/Level layout Config")]
    public abstract class LevelLayoutConfigSO : ScriptableObject
    {

        [SerializeField] private int width = 64;
        [SerializeField] private int height = 64;

        public int Width => width;
        public int Height => height;
    }
}
