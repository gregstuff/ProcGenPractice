using DungeonGeneration.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DungeonGeneration.Map.Model.Rooms
{
    public class DungeonRoomLevel : ICapabilityProvider
    {
        private readonly Dictionary<Type, object> _capabilities = new();
        private List<Room> _rooms;
        private int _width;
        private int _height;
        private float _scale;
        private Dictionary<Vector2Int, string> _spaces = new Dictionary<Vector2Int, string>();
        public List<Room> Rooms { get { return new List<Room>(_rooms); } }
        public List<Hallway> Hallways => GetHallways();
        public List<Door> AvailableDoors { get { return _rooms.SelectMany(room => room.PossibleDoors).ToList(); } }

        private bool[,] _blockedMap;
        private TileTypeSO[,] _tileTypeMap;

        private TileTypeSO _roomType;
        private TileTypeSO _hallwayType;
        private TileTypeSO _doorType;
        private TileTypeSO _unminedTile;

        public DungeonRoomLevel(DungeonRoomProps props)
        {
            var (width, height, mapScale, roomType, hallwayType, doorType, unminedTile) = props;

            _rooms = new List<Room>();
            _width = width;
            _height = height;
            _scale = mapScale;
            _roomType = roomType;
            _hallwayType = hallwayType;
            _doorType = doorType;
            _unminedTile = unminedTile;
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

        public void InitMapData()
        {
            _blockedMap = new bool[_height, _width];
            _tileTypeMap = new TileTypeSO[_height, _width];

            for (int y = 0; y < _height; ++y)
            {
                for (int x = 0; x < _width; ++x)
                {
                    _blockedMap[y, x] = true;
                    _tileTypeMap[y, x] = _unminedTile;
                }
            }

            Hallways.ForEach(hallway =>
            {
                var points = GridUtility.SetLineForGrid(_blockedMap, false, hallway.PointOne, hallway.PointTwo);
                AddTileTypeForPoints(points, _hallwayType);
                AddSpaceForPoints(points);
            });

            Rooms.ForEach(room =>
            {
                var points = room.LayoutTexture == null 
                    ? GridUtility.SetRectForGrid(_blockedMap, false, room.Area) 
                        : GridUtility.SetTextureForGrid(_blockedMap, false, room.LayoutTexture, room.Area);
                AddTileTypeForPoints(points, _roomType);
                AddSpaceForPoints(points);

                room.Doors.ForEach(existingDoor =>
                {
                    var doorPos = room.GetAbsolutePositionForDoor(existingDoor);
                    _blockedMap[doorPos.y, doorPos.x] = false;
                    _tileTypeMap[doorPos.y, doorPos.x] = _doorType;
                    AddSpaceForPoint(doorPos);
                });

            });

            _capabilities.Add(typeof(BlockMask), new BlockMask() { Mask = _blockedMap });
            _capabilities.Add(typeof(TileLayer), new TileLayer() { Tiles = _tileTypeMap });
            _capabilities.Add(typeof(Dimensions), new Dimensions(){ MapDimensions = new Vector2Int(_width, _height) });
            _capabilities.Add(typeof(Scale), new Scale() { MapScale = _scale });
            _capabilities.Add(typeof(SpaceMap), new SpaceMap() { Map = _spaces });
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

        private void AddSpaceForPoint(Vector2Int point)
        {
            string spaceID = GUID.Generate().ToString();
            if(_spaces.TryAdd(point, spaceID)){
                _spaces.Remove(point);
                _spaces.Add(point, spaceID);
            }
        }

        private void AddSpaceForPoints(List<Vector2Int> points)
        {
            string spaceID = GUID.Generate().ToString();
            points.ForEach((point) => 
            {
                if (_spaces.TryAdd(point, spaceID))
                {
                    _spaces.Remove(point);
                    _spaces.Add(point, spaceID);
                }
            });
        }
    }
}