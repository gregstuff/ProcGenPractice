
using System.Collections.Generic;
using System.Linq;

public class Level 
{

    private List<Room> _rooms;

    public List<Room> Rooms { get { return new List<Room>(_rooms); } }

    public List<Door> AvailableDoors { get { return _rooms.SelectMany(room => room.PossibleDoors).ToList(); } }

    public void AddRoom(Room newRoom)
    {
        _rooms.Add(newRoom);
    }

}
