using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher
{
    public interface IDecorationMatch
    {
        public Vector2Int SpawnPosition { get; set; }
        public GameObject ObjectToSpawn { get; set; }
        public Vector3 Offset { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }
}