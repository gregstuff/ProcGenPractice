using DungeonGeneration.Map.Model.Rooms;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Generated level layout", menuName = "Custom/Procedural Generation/Layout Config/Dungeon Room Level Layout Config")]
public class DungeonRoomLevelLayoutConfig : ScriptableObject
{
    [SerializeField] private int width = 64;
    [SerializeField] private int height = 64;
    [SerializeField] private int doorDistanceFromEdge = 1;
    [SerializeField] private int minHallwayLength = 2;
    [SerializeField] private int maxHallwayLength = 5;
    [SerializeField] private int maxRoomCount = 10;
    [SerializeField] private int minRoomDistance = 1;
    [SerializeField] private RoomTemplate[] roomTemplates;
    [SerializeField] private TileTypeSO _roomTile;
    [SerializeField] private TileTypeSO _hallwayTile;
    [SerializeField] private TileTypeSO _doorTile;
    [SerializeField] private TileTypeSO _unminedTile;

    public int DoorDistanceFromEdge => doorDistanceFromEdge;
    public int MinHallwayLength => minHallwayLength;
    public int MaxHallwayLength => maxHallwayLength;
    public int MaxRoomCount => maxRoomCount;
    public int MinRoomDistance => minRoomDistance;
    public RoomTemplate[] RoomTemplates => roomTemplates;
    public int Width => width;
    public int Height => height;

    public Dictionary<RoomTemplate, int> GetAvailableRooms()
    {
        Dictionary<RoomTemplate, int> availableRooms = new Dictionary<RoomTemplate, int>();
        for (int i = 0; i < roomTemplates.Length; ++i)
        {
            availableRooms.Add(roomTemplates[i], roomTemplates[i].NumberOfRooms);
        }
        return availableRooms.Where(kvp => kvp.Value > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public DungeonRoomProps GetDungeonRoomProps()
    {
        return new DungeonRoomProps()
        {
            Width = width,
            Height = height,
            RoomTile = _roomTile,
            HallwayTile = _hallwayTile,
            DoorTile = _doorTile,
            UnminedTile = _unminedTile
        };
    }

}
