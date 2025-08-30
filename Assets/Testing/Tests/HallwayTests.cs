using UnityEngine;
using NUnit.Framework;
using System.Linq;
using ProcGenSys.Pipeline.LevelDataGeneration.Model;

namespace DungeonGeneration.Tests
{
    public class HallwayTests
    {
        [Test]
        public void ConstructNewHallway_ValidDoors_CreatesHallway()
        {
            var room1 = new Room(new RectInt(0, 0, 5, 5), 1);
            var room2 = new Room(new RectInt(10, 0, 5, 5), 1);
            room1.CalculateAllPossibleDoors();
            room2.CalculateAllPossibleDoors();
            var door1 = room1.PossibleDoors.Find(d => d.Side == WallSide.RIGHT);
            var door2 = room2.PossibleDoors.Find(d => d.Side == WallSide.LEFT);

            var hallway = Hallway.ConstructNewHallway(door1, room1, door2, room2);

            Assert.IsNotNull(hallway);
            Assert.AreEqual(door1.Hallway, hallway, "Door1 should reference the hallway.");
            Assert.AreEqual(door2.Hallway, hallway, "Door2 should reference the hallway.");
            Assert.Contains(door1, hallway.Doors, "Hallway should contain door1.");
            Assert.Contains(door2, hallway.Doors, "Hallway should contain door2.");
        }

        [Test]
        public void GetArea_HorizontalHallway_ReturnsCorrectRect()
        {
            var hallway = new Hallway(new Vector2Int(2, 2), new Vector2Int(6, 2));

            var area = hallway.GetArea();

            Assert.AreEqual(new RectInt(3, 2, 3, 1), area, "Horizontal hallway area should be correct.");
        }

        [Test]
        public void GetArea_VerticalHallway_ReturnsCorrectRect()
        {
            var hallway = new Hallway(new Vector2Int(2, 2), new Vector2Int(2, 6));

            var area = hallway.GetArea();

            Assert.AreEqual(new RectInt(2, 3, 1, 3), area, "Vertical hallway area should be correct.");
        }

        [Test]
        public void ConstructNewHallway_InvalidPoints_ThrowsException()
        {
            var room = new Room(new RectInt(0, 0, 5, 5), 1);
            var door = room.PossibleDoors.First();

            Assert.Throws<System.ArgumentException>(() => Hallway.ConstructNewHallway(door, room, door, room), "Expected exception for identical points.");
        }
    }
}