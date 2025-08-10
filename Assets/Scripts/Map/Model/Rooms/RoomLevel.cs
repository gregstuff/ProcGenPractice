
using DungeonGeneration.Map.Enum;
using DungeonGeneration.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGeneration.Map.Model.Rooms
{
    public class RoomLevel : ILevel
    {

        private List<Room> _rooms;
        private int _width;
        private int _height;
        public int Width => _width;
        public int Height => _height;
        public List<Room> Rooms { get { return new List<Room>(_rooms); } }
        public List<Hallway> Hallways => GetHallways();
        public List<Door> AvailableDoors { get { return _rooms.SelectMany(room => room.PossibleDoors).ToList(); } }

        private bool[,] _blockedMap;
        private TileType[,] _tileTypeMap;

        public RoomLevel(int width, int height)
        {
            _rooms = new List<Room>();
            _width = width;
            _height = height;
        }

        public void AddRoom(Room newRoom)
        {
            _rooms.Add(newRoom);
        }

        private List<Hallway> GetHallways()
        {
            HashSet<Hallway> hallways = new HashSet<Hallway>();
            foreach (Room room in _rooms)
            {
                foreach (Door door in room.Doors)
                {
                    hallways.Add(door.Hallway);
                }
            }
            return hallways.ToList();
        }

        private void InitMapData()
        {
            _blockedMap = new bool[Height, Width];
            _tileTypeMap = new TileType[Height, Width];

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    _blockedMap[y, x] = true;
                    _tileTypeMap[y, x] = TileType.None;
                }
            }

            Hallways.ForEach(hallway =>
            {
                GridUtility.SetLineForGrid(_blockedMap, false, hallway.PointOne, hallway.PointTwo);
                GridUtility.SetLineForGrid(_tileTypeMap, TileType.Hallway, hallway.PointOne, hallway.PointTwo);
            });

            Rooms.ForEach(room =>
            {
                if (room.LayoutTexture == null)
                {
                    GridUtility.SetRectForGrid(_blockedMap, false, room.Area);
                    GridUtility.SetRectForGrid(_tileTypeMap, TileType.Room, room.Area);
                }
                else
                {
                    GridUtility.SetTextureForGrid(_blockedMap, false, room.LayoutTexture, room.Area);
                    GridUtility.SetTextureForGrid(_tileTypeMap, TileType.Room, room.LayoutTexture, room.Area);
                }

                room.Doors.ForEach(existingDoor =>
                {
                    var roomPos = room.GetAbsolutePositionForDoor(existingDoor);
                    _blockedMap[roomPos.y, roomPos.x] = false;
                    _tileTypeMap[roomPos.y, roomPos.x] = TileType.Door;
                });

            });

        }

        public bool[,] GetBlockedMap()
        {
            if (_blockedMap == null) InitMapData();
            return _blockedMap;
        }

        public TileType GetTileTypeAt(int x, int y)
        {
            if (_tileTypeMap == null) InitMapData();
            if (y >= Height || y < 0 || x >= Width || x < 0) return TileType.None;
            return _tileTypeMap[y, x];
        }

    }
}