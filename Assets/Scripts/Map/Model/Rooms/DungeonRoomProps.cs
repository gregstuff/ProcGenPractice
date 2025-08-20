

public class DungeonRoomProps
{
    public int Width;
    public int Height;
    public TileTypeSO RoomType;
    public TileTypeSO HallwayType;
    public TileTypeSO DoorType;

    public void Deconstruct(
        out int width,
        out int height,
        out TileTypeSO roomType,
        out TileTypeSO hallwayType,
        out TileTypeSO doorType)
    {
        width = Width;
        height = Height;
        roomType = RoomType;
        hallwayType = HallwayType;
        doorType = DoorType;
    }

}
