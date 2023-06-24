using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;

    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    private RoomNodeGraph roomNodeGraph;

    public UnityEvent OnFinishedRoomGeneration;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        roomNodeGraph = new RoomNodeGraph();

        var roomsList = ProceduralGenerationAlgorithms.binarySpacePartitioning(new BoundsInt((Vector3Int)startPosition,
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);


        if (randomWalkRooms)
        {
            CreateNonRectagularRooms(roomsList);
        }
        //else
        //{
        //    floor = CreateSimpleRooms(roomsList);
        //}

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        ConnetRooms();
        //floor.UnionWith(corridors);

        HashSet<Vector2Int> floor = roomNodeGraph.CreateGraphFloor();

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        //Roboczo
     

        tilemapVisualizer.PaintDoorsTiles(roomNodeGraph.roomNodesList);
    }

    private void CreateNonRectagularRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        RoomNode roomCreated = null;
        for (int i = 0; i < roomsList.Count; i++)
        {
            roomCreated = CreateNonRectagularRoom(roomsList[i]);
            roomNodeGraph.AddRoomToGraph(roomCreated);
        }
    }

    private RoomNode CreateNonRectagularRoom(BoundsInt roomBound)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBound.center.x), Mathf.RoundToInt(roomBound.center.y));
        var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);

        foreach (var position in roomFloor)
        {
            if (position.x >= (roomBound.xMin + offset) && position.x <= (roomBound.xMax - offset) &&
                position.y >= (roomBound.yMin + offset) && position.y <= (roomBound.yMax - offset))
            {
                floor.Add(position);
            }
        }
        return new RoomNode(floor, roomBound);
    }

    private void ConnetRooms()
    {
        List<RoomNode> roomNodesList = new List<RoomNode>(roomNodeGraph.roomNodesList);
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        var currentRoom = roomNodesList[UnityEngine.Random.Range(0, roomNodesList.Count)];
        roomNodesList.Remove(currentRoom);

        while (roomNodesList.Count > 0)
        {
            RoomNode closest = FindClosePointTo(currentRoom, roomNodesList);
            roomNodesList.Remove(closest);

            Coridor newCorridor = CreateCoridor(currentRoom, closest);
            currentRoom.AddRoomConnectionTo(closest, newCorridor);
            closest.AddRoomConnectionFrom(currentRoom, newCorridor);

            currentRoom = closest;
        }

    }

    private RoomNode FindClosePointTo(RoomNode currentRoom, List<RoomNode> roomNodesList)
    {
        RoomNode closest = null;
        float distance = float.MaxValue;

        foreach (var room in roomNodesList)
        {
            float currentDistance = Vector2.Distance(room.roomCenter, currentRoom.roomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = room;
            }
        }
        return closest;
    }

    private Coridor CreateCoridor(RoomNode currentRoom, RoomNode destinationRoom)
    {
        List<Vector2Int> coridorFloors = new List<Vector2Int>();

        int coridorLength = 0;
        var position = currentRoom.roomCenter;
        coridorFloors.Add(position);
        bool insideFirstRoom = true;
        bool insideSecondRoom = false;
        Vector2Int lastDirection = Vector2Int.zero;
        Vector2Int coridorDirection = Vector2Int.zero;
        Vector2Int coridorEntrancePositon = Vector2Int.zero; ;

        while (position.y != destinationRoom.roomCenter.y)
        {
            if (destinationRoom.roomCenter.y > position.y)
            {
                position += Vector2Int.up;
                lastDirection = Vector2Int.up;
            }
            else if (destinationRoom.roomCenter.y < position.y)
            {
                position += Vector2Int.down;
                lastDirection = Vector2Int.down;
            }
            coridorLength++;
            coridorFloors.Add(position);

            if (!currentRoom.roomFloors.Contains(position) && insideFirstRoom)
            {
                coridorEntrancePositon = position;
                coridorDirection = lastDirection;
                insideFirstRoom = false;
            }
            
        }
        while (position.x != destinationRoom.roomCenter.x)
        {
            if (destinationRoom.roomCenter.x > position.x)
            {
                position += Vector2Int.right;
                lastDirection = Vector2Int.right;
            }
            else if (destinationRoom.roomCenter.x < position.x)
            {
                position += Vector2Int.left;
                lastDirection = Vector2Int.left;
            }
            coridorLength++;
            coridorFloors.Add(position);

            if (!currentRoom.roomFloors.Contains(position) && insideFirstRoom)
            {
                coridorEntrancePositon = position;
                coridorDirection = lastDirection;
                insideFirstRoom = false;
            }
        }
        var coridor = new HashSet<Vector2Int>(IncreaseCorridorSizeByOne(coridorFloors));
        return new Coridor(coridor, coridorLength, coridorEntrancePositon,  coridorEntrancePositon + GetDirection90From(coridorDirection), coridorDirection);
    }

    private List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int previousDirection = Vector2Int.zero;

        for (int i = 1; i < corridor.Count; i++)
        {
            Vector2Int directionFromCell = corridor[i] - corridor[i - 1];

            //if corrner
            if (previousDirection != Vector2Int.zero && directionFromCell != previousDirection)
            {
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                    }
                }
                previousDirection = directionFromCell;
            }
            else
            {
                Vector2Int newCorridorTileOffset = GetDirection90From(directionFromCell);
                newCorridor.Add(corridor[i - 1]);
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset);
            }
        }
        return newCorridor;
    }

    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
            return Vector2Int.right;
        if (direction == Vector2Int.down)
            return Vector2Int.left;
        if (direction == Vector2Int.left)
            return Vector2Int.up;
        if (direction == Vector2Int.right)
            return Vector2Int.down;
        return Vector2Int.zero;
    }
}

