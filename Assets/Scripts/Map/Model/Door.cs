using UnityEngine;

public class Door
{
    private WallSide _side;
    private Vector2Int _roomPosition;
    private Vector2Int _absolutePosition;
    private Hallway _hallway;

    public WallSide Side => _side;
    public Vector2Int RoomPosition => _roomPosition;
    public Vector2Int AbsolutePosition => _absolutePosition;
    public Hallway Hallway {  get { return _hallway; } set { _hallway = value; } }

    public Door(WallSide side, Vector2Int pos, Vector2Int roomPos)
    {
        _side = side;
        _roomPosition = pos;

        _absolutePosition = new Vector2Int
        {
            x = pos.x + roomPos.x,
            y = pos.y + roomPos.y
        };
    }
}
