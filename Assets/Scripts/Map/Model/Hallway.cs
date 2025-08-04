using System.Collections.Generic;
using UnityEngine;

public class Hallway
{
    private Vector2Int PointOne { get; }
    private Vector2Int PointTwo {  get; }

    private List<Door> _doors;

    public Hallway(Vector2Int pointOne, Vector2Int pointTwo, Door[] doors = null)
    {
        PointOne = pointOne;
        PointTwo = pointTwo;

        _doors = doors == null ? new List<Door>() : new List<Door>(doors);
    }

    public RectInt GetArea()
    {
        int xOffset = 0, yOffset = 0, widthOffset = 0, heightOffset = 0;

        if (PointOne.x == PointTwo.x)
        {
            yOffset += 1;
            heightOffset -= 1;
        }
        else if (PointOne.y == PointTwo.y)
        {
            xOffset += 1;
            widthOffset -= 1;
        }


        var rect = new RectInt
        {
            x = Mathf.Min(PointOne.x, PointTwo.x) + xOffset,
            y = Mathf.Min(PointOne.y, PointTwo.x) + yOffset,
            width = Mathf.Max(Mathf.Abs(PointOne.x - PointTwo.x), 1) + widthOffset,
            height = Mathf.Max(Mathf.Abs(PointOne.y - PointTwo.y), 1) + heightOffset
        };

        return rect;
    }

    public void AddDoor(Door door) 
    { 
        _doors.Add(door);
    }

    public static Hallway ConstructNewHallway(Door doorOne, Door doorTwo)
    {
        Hallway newHallway = new Hallway(doorOne.AbsolutePosition, doorTwo.AbsolutePosition, new[] { doorOne, doorTwo } );

        doorOne.Hallway = newHallway;
        doorTwo.Hallway = newHallway;

        return newHallway;
    }

}
