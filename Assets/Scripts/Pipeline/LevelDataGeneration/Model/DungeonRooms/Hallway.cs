using System.Collections.Generic;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDataGeneration.Model
{
    public class Hallway
    {
        private Vector2Int _pointOne;
        private Vector2Int _pointTwo;
        public Vector2Int PointOne => _pointOne;
        public Vector2Int PointTwo => _pointTwo;
        public List<Door> Doors => new List<Door>(_doors);

        private List<Door> _doors;

        public Hallway(Vector2Int pointOne, Vector2Int pointTwo, Door[] doors = null)
        {
            _pointOne = pointOne;
            _pointTwo = pointTwo;

            _doors = doors == null ? new List<Door>() : new List<Door>(doors);
        }

        public RectInt GetArea()
        {
            int xOffset = 0, yOffset = 0, widthOffset = 0, heightOffset = 0;

            if (PointOne.x == PointTwo.x)
            {
                yOffset += 1;
                heightOffset -= 1;
            }
            else if (PointOne.y == PointTwo.y)
            {
                xOffset += 1;
                widthOffset -= 1;
            }


            var rect = new RectInt
            {
                x = Mathf.Min(PointOne.x, PointTwo.x) + xOffset,
                y = Mathf.Min(PointOne.y, PointTwo.x) + yOffset,
                width = Mathf.Max(Mathf.Abs(PointOne.x - PointTwo.x), 1) + widthOffset,
                height = Mathf.Max(Mathf.Abs(PointOne.y - PointTwo.y), 1) + heightOffset
            };

            return rect;
        }

        public void AddDoor(Door door)
        {
            _doors.Add(door);
        }

        public static Hallway ConstructNewHallway(Door doorOne, Room roomOne, Door doorTwo, Room roomTwo)
        {
            var doorOneAbsolutePos = roomOne.GetAbsolutePositionForDoor(doorOne);
            var doorTwoAbsolutePos = roomTwo.GetAbsolutePositionForDoor(doorTwo);

            if (doorOneAbsolutePos == doorTwoAbsolutePos) throw new System.ArgumentException("Doors have the same absolute position");

            Hallway newHallway = new Hallway(doorOneAbsolutePos,
                doorTwoAbsolutePos, new[] { doorOne, doorTwo });

            doorOne.Hallway = newHallway;
            doorTwo.Hallway = newHallway;

            roomOne.UseDoor(doorOne);
            roomTwo.UseDoor(doorTwo);

            return newHallway;
        }

    }
}