
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


        //initially set entire map to blocked
        for (int y = 0; y < blockedMap.GetLength(0); ++y)
        {
            for (int x = 0; x < blockedMap.GetLength(1); ++x)
            {
                blockedMap[y, x] = true;
            }
        }

        Hallways.ForEach(hallway => GridUtility.SetLineForGrid(blockedMap, false, hallway.PointOne, hallway.PointTwo));

        Rooms.ForEach(room =>
        {
            if (room.LayoutTexture == null) GridUtility.SetRectForGrid(blockedMap, false, room.Area);
            else GridUtility.SetTextureForGrid(blockedMap, false, room.LayoutTexture, room.Area);

            room.Doors.ForEach(existingDoor =>
            {
                var roomPos = room.GetAbsolutePositionForDoor(existingDoor);
                blockedMap[roomPos.y, roomPos.x] = false;

            });

        });



        return blockedMap;

    }

}
