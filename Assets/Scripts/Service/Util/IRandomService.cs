using UnityEngine;

public interface IRandomService
{
    public void RandSeed();
    public void SetSeed(int seed);
    public int GetSeed();
    public int NextInt(int max);
    public int NextInt(int min, int max);
    public int Next();
    public float NextFloat();   
}
