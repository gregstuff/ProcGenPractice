
using System.Collections.Generic;
using System.Linq;
public class Level 
{

    private List<Room> _rooms;
    private int _width;
    private int _height;
    public int Width => _width;
    public int Height => _height;
    public List<Room> Rooms { get { return new List<Room>(_rooms); } }

    public List<Door> AvailableDoors { get { return _rooms.SelectMany(room => room.PossibleDoors).ToList(); } }

    public Level(int width, int height)
    {
        _rooms = new List<Room>();
        _width = width;
        _height = height;
    }

    public void AddRoom(Room newRoom)
    {
        _rooms.Add(newRoom);
    }

}
