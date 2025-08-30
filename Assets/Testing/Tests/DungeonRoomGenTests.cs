
namespace DungeonGeneration.Tests
{
    public class DungeonRoomGenTests
    {
        /*
        private DungeonRoomGenSO _generator;
        private IRandomService _randomService;
        private GeneratedLevelLayoutSO _layoutConfig;
        private RoomTemplate _roomTemplate;

        [SetUp]
        public void SetUp()
        {
            _generator = new DungeonRoomGenSO();
            _randomService = new MockRandomService();
            RandomSingleton_SetInstance(_randomService);
            _roomTemplate = new RoomTemplate();
            _layoutConfig = ScriptableObject.CreateInstance<GeneratedLevelLayoutSO>();
            _layoutConfig.SetField("width", 20);
            _layoutConfig.SetField("height", 20);
            _layoutConfig.SetField("maxRoomCount", 3);
            _layoutConfig.SetField("minRoomDistance", 1);
            _layoutConfig.SetField("doorDistanceFromEdge", 1);
            _layoutConfig.SetField("minHallwayLength", 2);
            _layoutConfig.SetField("maxHallwayLength", 4);
            _layoutConfig.SetField("roomTemplates", new[] { _roomTemplate });
        }

        [TearDown]
        public void TearDown()
        {
            RandomSingleton_SetInstance(null);
            Object.DestroyImmediate(_layoutConfig);
        }

        [Test]
        public void GenerateDungeonLevel_ValidConfig_CreatesLevelWithRooms()
        {
            ((MockRandomService)_randomService).SetNextInts(0, 5, 5, 0, 0, 2, 0, 0); // Control random room placement and door selection
            var level = (DungeonRoomLevel) _generator.GenerateDungeonLevel(_layoutConfig);

            Assert.IsNotNull(level);
            Assert.IsTrue(level.Rooms.Count > 0, "Level should contain at least one room.");
            Assert.IsTrue(level.Rooms.All(r => level.Width >= r.Area.xMax && level.Height >= r.Area.yMax), "Rooms should be within level bounds.");
        }

        [Test]
        public void GenerateDungeonLevel_NoRoomTemplates_ThrowsException()
        {
            _layoutConfig.SetField("roomTemplates", new RoomTemplate[0]);
            Assert.Throws<System.ArgumentException>(() => _generator.GenerateDungeonLevel(_layoutConfig), "Expected exception for empty room templates.");
        }

        [Test]
        public void GenerateDungeonLevel_RoomOverlapAvoided()
        {
            ((MockRandomService)_randomService).SetNextInts(0, 5, 5, 0, 0, 2, 0, 0); // Control room placement
            var level = (DungeonRoomLevel)_generator.GenerateDungeonLevel(_layoutConfig);

            var rooms = level.Rooms;
            for (int i = 0; i < rooms.Count; i++)
            {
                for (int j = i + 1; j < rooms.Count; j++)
                {
                    var paddedRect1 = new RectInt(
                        rooms[i].Area.x - _layoutConfig.MinRoomDistance,
                        rooms[i].Area.y - _layoutConfig.MinRoomDistance,
                        rooms[i].Area.width + 2 * _layoutConfig.MinRoomDistance,
                        rooms[i].Area.height + 2 * _layoutConfig.MinRoomDistance);
                    Assert.IsFalse(paddedRect1.Overlaps(rooms[j].Area), $"Rooms {i} and {j} overlap.");
                }
            }
        }

        [Test]
        public void GenerateDungeonLevel_RespectsMaxRoomCount()
        {
            _layoutConfig.SetField("maxRoomCount", 2);
            ((MockRandomService)_randomService).SetNextInts(0, 5, 5, 0, 0, 2, 0, 0, 0, 5, 5, 0, 0, 2);
            var level = (DungeonRoomLevel)_generator.GenerateDungeonLevel(_layoutConfig);

            Assert.LessOrEqual(level.Rooms.Count, 2, "Level should not exceed max room count.");
        }

        // Mock RandomSingleton
        private void RandomSingleton_SetInstance(IRandomService instance)
        {
            typeof(RandomSingleton).GetField("_instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .SetValue(null, instance);
        }
    }
    public class MockRandomService : IRandomService
    {
        private Queue<int> _nextInts = new Queue<int>();

        public void SetNextInts(params int[] values)
        {
            _nextInts = new Queue<int>(values);
        }

        public int NextInt(int max)
        {
            return _nextInts.Count > 0 ? _nextInts.Dequeue() : 0;
        }

        public int NextInt(int min, int max)
        {
            return _nextInts.Count > 0 ? _nextInts.Dequeue() : min;
        }

        public int Next()
        {
            return _nextInts.Count > 0 ? _nextInts.Dequeue() : 0;
        }

        public void RandSeed()
        {
            throw new System.NotImplementedException();
        }

        public void SetSeed(int seed)
        {
            throw new System.NotImplementedException();
        }
    }

    public static class ScriptableObjectExtensions
    {
        public static void SetField<T>(this ScriptableObject obj, string fieldName, T value)
        {
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(obj, value);
        }
        */
    }
}