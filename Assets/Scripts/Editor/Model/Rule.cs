using DungeonGeneration.Map.Enum;

[System.Serializable]
public class Rule
{
    public string id = "Rule_" + System.Guid.NewGuid().ToString().Substring(0, 4);
    public int width = 3;
    public int height = 3;
    public TileType[,] pattern;
    public bool foldout = true;
    public int maxApplications = -1; // -1 for infinite

    public Rule()
    {
        pattern = new TileType[height, width];
    }

    public void ResizeGrid(int newHeight, int newWidth)
    {
        newHeight = Mathf.Max(3, newHeight);
        newWidth = Mathf.Max(3, newWidth);
        var newPattern = new TileType[newHeight, newWidth];
        for (int y = 0; y < Mathf.Min(height, newHeight); y++)
            for (int x = 0; x < Mathf.Min(width, newWidth); x++)
                newPattern[y, x] = pattern[y, x];
        pattern = newPattern;
        height = newHeight;
        width = newWidth;
    }
}