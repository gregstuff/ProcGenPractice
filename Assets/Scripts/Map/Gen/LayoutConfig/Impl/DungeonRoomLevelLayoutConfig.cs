

using DungeonGeneration.Map.Model.Rooms;
using DungeonGeneration.Map.SO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonRoomLevelLayoutConfig : LevelLayoutConfigSO
{
    [SerializeField] private int doorDistanceFromEdge = 1;
    [SerializeField] private int minHallwayLength = 2;
    [SerializeField] private int maxHallwayLength = 5;
    [SerializeField] private int maxRoomCount = 10;
    [SerializeField] private int minRoomDistance = 1;
    [SerializeField] private RoomTemplate[] roomTemplates;

    public int DoorDistanceFromEdge => doorDistanceFromEdge;
    public int MinHallwayLength => minHallwayLength;
    public int MaxHallwayLength => maxHallwayLength;
    public int MaxRoomCount => maxRoomCount;
    public int MinRoomDistance => minRoomDistance;
    public RoomTemplate[] RoomTemplates => roomTemplates;

    public Dictionary<RoomTemplate, int> GetAvailableRooms()
    {
        Dictionary<RoomTemplate, int> availableRooms = new Dictionary<RoomTemplate, int>();
        for (int i = 0; i < roomTemplates.Length; ++i)
        {
            availableRooms.Add(roomTemplates[i], roomTemplates[i].NumberOfRooms);
        }
        return availableRooms.Where(kvp => kvp.Value > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

}
