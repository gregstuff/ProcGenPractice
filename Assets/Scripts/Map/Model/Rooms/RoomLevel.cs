using DungeonGeneration.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGeneration.Map.Model.Rooms
{
    public class RoomLevel : ICapabilityProvider
    {
        private readonly Dictionary<Type, object> _capabilities = new();
        private List<Room> _rooms;
        private int _width;
        private int _height;
        public int Width => _width;
        public int Height => _height;
        public List<Room> Rooms { get { return new List<Room>(_rooms); } }
        public List<Hallway> Hallways => GetHallways();
        public List<Door> AvailableDoors { get { return _rooms.SelectMany(room => room.PossibleDoors).ToList(); } }

        private bool[,] _blockedMap;
        private TileTypeSO[,] _tileTypeMap;

        private TileTypeSO _roomType;
        private TileTypeSO _hallwayType;
        private TileTypeSO _doorType;

        public RoomLevel(DungeonRoomProps props)
        {
            var (width, height, roomType, hallwayType, doorType) = props;


            _rooms = new List<Room>();
            _width = width;
            _height = height;
            _roomType = roomType;
            _hallwayType = hallwayType;
            _doorType = doorType;
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
            _tileTypeMap = new TileTypeSO[Height, Width];

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    _blockedMap[y, x] = true;
                    _tileTypeMap[y, x] = null;
                }
            }

            Hallways.ForEach(hallway =>
            {
                var points = GridUtility.SetLineForGrid(_blockedMap, false, hallway.PointOne, hallway.PointTwo);
                AddTileTypeForPoints(points, _hallwayType);
            });

            Rooms.ForEach(room =>
            {
                if (room.LayoutTexture == null)
                {
                    var points = GridUtility.SetRectForGrid(_blockedMap, false, room.Area);
                    AddTileTypeForPoints(points, _roomType);
                }
                else
                {
                    var points = GridUtility.SetTextureForGrid(_blockedMap, false, room.LayoutTexture, room.Area);
                    AddTileTypeForPoints(points, _roomType);
                }

                room.Doors.ForEach(existingDoor =>
                {
                    var roomPos = room.GetAbsolutePositionForDoor(existingDoor);
                    _blockedMap[roomPos.y, roomPos.x] = false;
                    _tileTypeMap[roomPos.y, roomPos.x] = _doorType;
                });

            });

            _capabilities.Add(typeof(IBlockMask), _blockedMap);
            _capabilities.Add(typeof(ITileLayer), _tileTypeMap);
        }

        public bool TryGet<T>(out T capability) where T : ICapability
        {
            _capabilities.TryGetValue(typeof(T), out var levelCapability);
            capability = (T) levelCapability;
            return capability != null;
        }

        private void AddTileTypeForPoints(List<Vector2Int> points, TileTypeSO type)
        {
            points.ForEach(point => _tileTypeMap[point.y, point.x] = type);
        }
    }
}