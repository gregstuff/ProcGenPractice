using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using DungeonGeneration.Map.Enum;
using DungeonGeneration.Map.SO;
using DungeonGeneration.Map.Model.Rooms;
using DungeonGeneration.Service.Util;

namespace DungeonGeneration.Map.Gen.Impl
{
    public class DungeonRoomGen : IDungeonLevelGenerator
    {
        public ICapabilityProvider GenerateDungeonLevel(GeneratedLevelLayoutSO levelConfig)
        {
            ValidateLevelLayout(levelConfig);
            return GenerateLevel(levelConfig);
        }

        private void ValidateLevelLayout(GeneratedLevelLayoutSO levelConfig)
        {
            if (levelConfig.RoomTemplates.Count() == 0)
                throw new ArgumentException("No room templates configured!");
            if (levelConfig.Width <= 0 || levelConfig.Height <= 0)
                throw new ArgumentException("Level dimensions must be positive!");
            if (levelConfig.MaxRoomCount <= 0)
                throw new ArgumentException("MaxRoomCount must be positive!");
            if (levelConfig.MinRoomDistance < 0)
                throw new ArgumentException("MinRoomDistance cannot be negative!");
        }

        private Room GetStartRoom(GeneratedLevelLayoutSO levelConfig)
        {
            var randService = RandomSingleton.Instance;
            var randRoomTemplate = levelConfig.RoomTemplates[randService.NextInt(levelConfig.RoomTemplates.Count())];

            RectInt roomSize = randRoomTemplate.GenerateRoomCandidateRect();

            int availableWidth = levelConfig.Width / 2 - roomSize.width;
            int randomX = randService.NextInt(0, availableWidth);

            int roomX = randomX + (levelConfig.Width / 4);

            int availableHeight = levelConfig.Height / 2 - roomSize.height;
            int randomY = randService.NextInt(0, availableHeight);

            int roomY = randomY + (levelConfig.Height / 4);

            var pos = new RectInt()
            {
                x = roomX,
                y = roomY,
                width = roomSize.width,
                height = roomSize.height
            };

            return new Room(pos, levelConfig.DoorDistanceFromEdge);
        }

        void AdjustCandidateRoomPosition(
            Room existingRoom,
            Door existingDoor,
            Room candidateRoom,
            Door candidateDoor,
            int distance)
        {
            var roomPosition = existingRoom.GetAbsolutePositionForDoor(existingDoor);
            int roomWidth = candidateRoom.Area.width;
            int roomHeight = candidateRoom.Area.height;

            switch (existingDoor.Side)
            {
                case WallSide.LEFT:
                    roomPosition.x -= distance + roomWidth;
                    roomPosition.y -= candidateDoor.RoomPosition.y;
                    break;
                case WallSide.RIGHT:
                    roomPosition.x += distance + 1;
                    roomPosition.y -= candidateDoor.RoomPosition.y;
                    break;
                case WallSide.FRONT:
                    roomPosition.x -= candidateDoor.RoomPosition.x;
                    roomPosition.y += distance + 1;
                    break;
                case WallSide.BACK:
                    roomPosition.x -= candidateDoor.RoomPosition.x;
                    roomPosition.y -= distance + roomHeight;
                    break;
            }

            candidateRoom.SetPosition(roomPosition);
        }

        Room ConstructAdjacentRoom(
            Room existingRoom,
            Door potentialDoor,
            RoomTemplate candidateTemplate,
            GeneratedLevelLayoutSO levelTemplate,
            RoomLevel level)
        {
            var randService = RandomSingleton.Instance;
            RectInt roomCandRect = candidateTemplate.GenerateRoomCandidateRect();
            Room candidateRoom = new Room(roomCandRect, levelTemplate.DoorDistanceFromEdge, candidateTemplate.LayoutTexture);
            Door candidateDoor = candidateRoom.PossibleDoors
                .Where(d => d.Side == potentialDoor.Side.GetOpposite())
                .OrderBy(_ => randService.Next()).FirstOrDefault();

            if (candidateDoor == null)
            {
                return null;
            }

            var resolvedHallwayLength = randService.NextInt(levelTemplate.MinHallwayLength, levelTemplate.MaxHallwayLength);

            AdjustCandidateRoomPosition(existingRoom, potentialDoor, candidateRoom, candidateDoor, resolvedHallwayLength);

            if (!IsRoomCandidateValid(candidateRoom.Area, levelTemplate, level)) return null;

            /*
                also adds references to this hallway to the doors here
                room has references to doors, doors have references to hallways
                hallways also have references to doors
            */

            Hallway newHallway = Hallway.ConstructNewHallway(
                potentialDoor,
                existingRoom,
                candidateDoor,
                candidateRoom);

            return candidateRoom;
        }

        private bool IsRoomCandidateValid(
            RectInt roomCandRect,
            GeneratedLevelLayoutSO levelTemplate,
            RoomLevel level)
        {
            RectInt levelRect = new RectInt(1, 1, levelTemplate.Width - 2, levelTemplate.Height - 2);

            return levelRect.Contains(roomCandRect) && !CheckRoomOverlap(roomCandRect, level, levelTemplate.MinRoomDistance);

        }

        private bool CheckRoomOverlap(RectInt roomCandidateRect,
            RoomLevel level,
            int minRoomDistance)
        {
            var rooms = level.Rooms;
            var hallways = level.Rooms.SelectMany(room => room.Doors.Select(door => door.Hallway));

            RectInt paddedRoomRect = new RectInt
            {
                x = roomCandidateRect.x - minRoomDistance,
                y = roomCandidateRect.y - minRoomDistance,
                width = roomCandidateRect.width + 2 * minRoomDistance,
                height = roomCandidateRect.height + 2 * minRoomDistance
            };

            var overlapsRoom = rooms.ToList().Any(room => paddedRoomRect.Overlaps(room.Area));
            var overlapsHallway = hallways.ToList().Any(hallway => paddedRoomRect.Overlaps(hallway.GetArea()));

            return overlapsRoom || overlapsHallway;
        }

        private RoomLevel GenerateLevel(GeneratedLevelLayoutSO levelLayout)
        {
            var randService = RandomSingleton.Instance;
            Dictionary<RoomTemplate, int> roomTemplatesToCount = levelLayout.GetAvailableRooms();
            RoomLevel level = new RoomLevel(_roomProps);
            level.AddRoom(GetStartRoom(levelLayout));

            while (level.AvailableDoors.Count > 0
                && level.Rooms.Count < levelLayout.MaxRoomCount
                && roomTemplatesToCount.Count > 0)
            {
                //should always return something, we already checked for available doors in the loop condition
                Room randExistingRoom = level.Rooms.Where(room => room.PossibleDoors.Count > 0).OrderBy(_ => randService.Next()).FirstOrDefault();
                Door randPossibleDoor = randExistingRoom.PossibleDoors.OrderBy(_ => randService.Next()).FirstOrDefault();
                RoomTemplate randTemplate = roomTemplatesToCount.Keys.OrderBy(_ => randService.Next()).FirstOrDefault();

                Room newRoom = ConstructAdjacentRoom(
                    randExistingRoom,
                    randPossibleDoor,
                    randTemplate,
                    levelLayout,
                    level);

                if (newRoom == null)
                {
                    randExistingRoom.RemovePossibleDoor(randPossibleDoor);
                    continue;
                }

                RemoveOrDecrementAvailableRoom(randTemplate, roomTemplatesToCount);

                level.AddRoom(newRoom);
            }

            return level;

        }
        private void RemoveOrDecrementAvailableRoom(RoomTemplate roomTemplate, Dictionary<RoomTemplate, int> roomTemplatesToCount)
        {
            if (!roomTemplatesToCount.TryGetValue(roomTemplate, out var numLeft)) return;
            roomTemplatesToCount[roomTemplate] = numLeft - 1;
            if (roomTemplatesToCount[roomTemplate] <= 0) roomTemplatesToCount.Remove(roomTemplate);
        }

    }
}