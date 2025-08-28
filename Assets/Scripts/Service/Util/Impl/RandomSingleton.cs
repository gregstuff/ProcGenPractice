using System;

namespace DungeonGeneration.Service.Util
{
    public class RandomSingleton : IRandomService
    {

        private static IRandomService _instance;
        public static IRandomService Instance
        {
            get
            {
                if (_instance == null) _instance = new RandomSingleton();
                return _instance;
            }
        }

        private int _seed;
        private System.Random _random;

        public RandomSingleton()
        {
            RandSeed();
        }

        public void RandSeed()
        {
            _seed = Environment.TickCount;
            SetRandom();
        }

        public int GetSeed()
        {
            return _seed;
        }

        public void SetSeed(int seed)
        {
            _seed = seed;
            SetRandom();
        }

        private void SetRandom()
        {
            _random = new System.Random(_seed);
        }

        public int NextInt(int max)
        {
            return _random.Next(max);
        }

        public int NextInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        public int Next()
        {
            return _random.Next();
        }

        public float NextFloat()
        {
            return (float) _random.NextDouble();
        }


    }
}