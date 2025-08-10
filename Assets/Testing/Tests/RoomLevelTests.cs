using UnityEngine;
using NUnit.Framework;
using DungeonGeneration.Map.Enum;
using DungeonGeneration.Map.Model.Rooms;
using DungeonGeneration.Utilities;

namespace DungeonGeneration.Tests
{
    public class RoomLevelTests
    {
        private RoomLevel _level;
        private MockGridUtility _gridUtility;

        [SetUp]
        public void SetUp()
        {
            _level = new RoomLevel(10, 10);
            _gridUtility = new MockGridUtility();
            GridUtility_SetInstance(_gridUtility); 
        }

        [TearDown]
        public void TearDown()
        {
            GridUtility_SetInstance(null);
        }

        [Test]
        public void AddRoom_RoomAddedToList()
        {
            var room = new Room(new RectInt(2, 2, 4, 4), 1);
            _level.AddRoom(room);

            Assert.Contains(room, _level.Rooms, "Room should be added to the level.");
        }

        [Test]
        public void GetBlockedMap_InitializesMapWithRoomsAndHallways()
        {
            var room = new Room(new RectInt(2, 2, 4, 4), 1);
            room.CalculateAllPossibleDoors();
            var door = room.PossibleDoors.Find(d => d.Side == WallSide.RIGHT);
            var hallway = new Hallway(new Vector2Int(5, 3), new Vector2Int(7, 3), new[] { door });
            door.Hallway = hallway;
            room.UseDoor(door);
            _level.AddRoom(room);

            var blockedMap = _level.GetBlockedMap();

            Assert.IsNotNull(blockedMap);
            Assert.AreEqual(10, blockedMap.GetLength(0), "Map height should match level height.");
            Assert.AreEqual(10, blockedMap.GetLength(1), "Map width should match level width.");
            Assert.IsFalse(blockedMap[3, 5], "Hallway tile should be unblocked.");
            Assert.IsFalse(blockedMap[3, 3], "Room tile should be unblocked.");
            Assert.IsTrue(blockedMap[0, 0], "Outside tile should be blocked.");
        }

        [Test]
        public void GetTileTypeAt_OutOfBounds_NoneTileType()
        {
            Assert.AreEqual(TileType.None, _level.GetTileTypeAt(10, 10), "Expected Out of range tile type to be none.");
        }

        // Mock GridUtility
        private void GridUtility_SetInstance(MockGridUtility instance)
        {
            typeof(GridUtility).GetField("_instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                ?.SetValue(null, instance);
        }
    }

    // Mock GridUtility for testing
    public class MockGridUtility
    {
        public void SetLineForGrid<T>(T[,] grid, T value, Vector2Int start, Vector2Int end)
        {
            if (start.x == end.x)
            {
                for (int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++)
                    grid[y, start.x] = value;
            }
            else if (start.y == end.y)
            {
                for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++)
                    grid[start.y, x] = value;
            }
        }

        public void SetRectForGrid<T>(T[,] grid, T value, RectInt rect)
        {
            for (int y = rect.y; y < rect.yMax; y++)
                for (int x = rect.x; x < rect.xMax; x++)
                    grid[y, x] = value;
        }

        public void SetTextureForGrid<T>(T[,] grid, T value, Texture2D texture, RectInt rect)
        {
            SetRectForGrid(grid, value, rect);
        }
    }
}