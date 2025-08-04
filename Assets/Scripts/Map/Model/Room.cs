using System.Collections.Generic;
using UnityEngine;

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
                    _possibleDoors.Add(new Door(side, new Vector2Int { x = x, y = y }, _area.position));
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
            _possibleDoors.Add(new Door(WallSide.BACK, new Vector2Int(x, bottom), _area.position));
            _possibleDoors.Add(new Door(WallSide.FRONT, new Vector2Int(x, top), _area.position));
        }

        for (int y = minY; y < maxY; ++y)
        {
            _possibleDoors.Add(new Door(WallSide.LEFT, new Vector2Int(left, y), _area.position));
            _possibleDoors.Add(new Door(WallSide.RIGHT, new Vector2Int(right, y), _area.position));
        }
    }

    public void SetPosition(Vector2Int pos)
    {
        _area.position = pos;
    }

    public void UseDoor(Door door)
    {
        //this room does not know about this door....
        if (!_possibleDoors.Contains(door)) return;

        //possible door to door
        //remove adjacent possible doors
    }

    public void RemovePossibleDoor(Door door)
    {
        _possibleDoors.Remove(door);
    }

}
