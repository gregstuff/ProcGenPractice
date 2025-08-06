
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class Level 
{

    private List<Room> _rooms;
    private int _width;
    private int _height;
    public int Width => _width;
    public int Height => _height;
    public List<Room> Rooms { get { return new List<Room>(_rooms); } }
    public List<Hallway> Hallways => GetHallways();
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

    private List<Hallway> GetHallways()
    {
        HashSet<Hallway> hallways = new HashSet<Hallway>();
        foreach (Room room in _rooms)
        {
            foreach (Door door in room.Doors)
            {
                hallways.Add(door.Hallway);
            }
        }
        return hallways.ToList();
    }

    public bool[,] ToBlockedMap()
    {
        bool[,] blockedMap = new bool[Height, Width];

        Hallways.ForEach(hallway => GridUtility.SetLineForGrid(blockedMap, true, hallway.PointOne, hallway.PointTwo));

        Rooms.ForEach(room =>
        {
            if (room.LayoutTexture == null) GridUtility.SetRectForGrid(blockedMap, true, room.Area);
            else GridUtility.SetTextureForGrid(blockedMap, true, room.LayoutTexture, room.Area);

            room.Doors.ForEach(existingDoor =>
            {
                var roomPos = room.GetAbsolutePositionForDoor(existingDoor);
                blockedMap[roomPos.y, roomPos.x] = true;

            });

        });



        return blockedMap;

    }

}
