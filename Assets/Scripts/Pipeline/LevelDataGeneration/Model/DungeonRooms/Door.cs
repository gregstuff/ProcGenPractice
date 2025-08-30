using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDataGeneration.Model
{
    public class Door
    {
        private WallSide _side;
        private Vector2Int _roomPosition;
        private Hallway _hallway;

        public WallSide Side => _side;
        public Vector2Int RoomPosition => _roomPosition;
        public Hallway Hallway { get { return _hallway; } set { _hallway = value; } }

        public Door(WallSide side, Vector2Int pos)
        {
            _side = side;
            _roomPosition = pos;
        }
    }
}