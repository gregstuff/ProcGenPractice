using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDataGeneration.Model
{
    public class Room
    {
        private RectInt _area;
        private Texture2D _layoutTexture;
        private int _minDoorDistFromEdge;
        private List<Door> _possibleDoors = new List<Door>();
        private List<Door> _doors = new List<Door>();
        public Texture2D LayoutTexture { get { return _layoutTexture; } }
        public RectInt Area { get { return _area; } }
        public List<Door> PossibleDoors => _possibleDoors;
        public List<Door> Doors => _doors;

        public Room(RectInt area,
            int minDoorDistFromEdge,
            Texture2D layoutTexture = null)
        {
            _area = area;
            _layoutTexture = layoutTexture;
            _minDoorDistFromEdge = minDoorDistFromEdge;
            CalculateAllPossibleDoors();
        }

        public void CalculateAllPossibleDoors()
        {
            if (LayoutTexture == null) CalculateAllPossibleDoorsForRect();
            else CalculateAllPossibleDoorsForTexture();
        }

        private void CalculateAllPossibleDoorsForTexture()
        {
            int width = LayoutTexture.width;
            int height = LayoutTexture.height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; ++x)
                {
                    Color pixelColor = LayoutTexture.GetPixel(x, y);
                    WallSide side = GetWallSide(pixelColor);
                    if (side != WallSide.UNDEFINED)
                        _possibleDoors.Add(new Door(side, new Vector2Int { x = x, y = y }));
                }
            }
        }

        private WallSide GetWallSide(Color pixelColor)
        {
            return WallSideExtension.GetDirectionForColor(pixelColor);
        }

        public void CalculateAllPossibleDoorsForRect()
        {
            var (width, height) = _area;


            int top = height;
            int bottom = -1;
            int minX = _minDoorDistFromEdge;
            int maxX = width - _minDoorDistFromEdge;

            int left = -1;
            int right = width;
            int minY = _minDoorDistFromEdge;
            int maxY = height - _minDoorDistFromEdge;

            for (int x = minX; x < maxX; ++x)
            {
                _possibleDoors.Add(new Door(WallSide.BACK, new Vector2Int(x, bottom)));
                _possibleDoors.Add(new Door(WallSide.FRONT, new Vector2Int(x, top)));
            }

            for (int y = minY; y < maxY; ++y)
            {
                _possibleDoors.Add(new Door(WallSide.LEFT, new Vector2Int(left, y)));
                _possibleDoors.Add(new Door(WallSide.RIGHT, new Vector2Int(right, y)));
            }
        }

        public void SetPosition(Vector2Int pos)
        {
            _area.position = pos;
        }

        public Vector2Int GetAbsolutePositionForDoor(Door door)
        {
            if (!_possibleDoors.Contains(door) && !_doors.Contains(door))
            {
                throw new Exception($"Room {Area} has no definition for door {door.RoomPosition}");
            }

            return new Vector2Int()
            {
                x = _area.position.x + door.RoomPosition.x,
                y = _area.position.y + door.RoomPosition.y,
            };
        }

        public void UseDoor(Door door)
        {
            //calc manhatten distance
            (int absX, int absY) GetAbsDiff(Door d1, Door d2)
            {
                return (
                    Mathf.Abs((d1.RoomPosition - d2.RoomPosition).x),
                    Mathf.Abs((d1.RoomPosition - d2.RoomPosition).y)
                );
            }

            //this room is not associated with this door
            if (!_possibleDoors.Contains(door)) return;

            _doors.Add(door);

            _possibleDoors = _possibleDoors.Where(possibleDoor =>
            {
                var (diffX, diffY) = GetAbsDiff(possibleDoor, door);
                return diffX + diffY > 1;
            }).ToList();

        }

        public void RemovePossibleDoor(Door door)
        {
            _possibleDoors.Remove(door);
        }

    }
}