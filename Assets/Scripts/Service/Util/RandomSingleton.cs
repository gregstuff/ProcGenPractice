using System;

public class RandomSingleton
{

    private static RandomSingleton _instance;
    public static RandomSingleton Instance
    {
        get
        {
            if (_instance == null) _instance = new RandomSingleton();
            return _instance;
        }
    }

    private int _seed;
    private System.Random _random;

    public RandomSingleton() {
        RandSeed();
    }

    public void RandSeed()
    {
        _seed = Environment.TickCount;
        SetRandom();
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


}
