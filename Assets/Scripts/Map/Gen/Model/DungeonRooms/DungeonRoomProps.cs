

public class DungeonRoomProps
{
    public int Width;
    public int Height;
    public TileTypeSO RoomTile;
    public TileTypeSO HallwayTile;
    public TileTypeSO DoorTile;
    public TileTypeSO UnminedTile;

    public void Deconstruct(
        out int width,
        out int height,
        out TileTypeSO roomTile,
        out TileTypeSO hallwayTile,
        out TileTypeSO doorTile,
        out TileTypeSO unminedTile)
    {
        width = Width;
        height = Height;
        roomTile = RoomTile;
        hallwayTile = HallwayTile;
        doorTile = DoorTile;
        unminedTile = UnminedTile;
    }

}
