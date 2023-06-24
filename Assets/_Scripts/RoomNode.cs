using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    public Guid roomId;
    public string roomType;

    public HashSet<Vector2Int> roomFloors;
    public HashSet<Vector2Int> roomWalls;

    public RoomNode roomNodeConnectedTo;
    public RoomNode roomNodeConnectedFrom;

    public Coridor roomEntranceCoridor = null;
    public Coridor roomExitCoridor = null;

    public Vector2Int roomEntrancePosition;
    public Vector2Int roomExitPosition;

    public BoundsInt roomBound;
    public Vector2Int roomCenter;

    public RoomNode(HashSet<Vector2Int> roomFloors, BoundsInt roomBound)
    {
        this.roomFloors = roomFloors;
        this.roomBound = roomBound;
        roomCenter = (Vector2Int)Vector3Int.RoundToInt(roomBound.center);
        roomId = Guid.NewGuid();
    }

    public void AddRoomConnectionTo(RoomNode roomNodeConnectedTo, Coridor roomExitCoridor)
    {
        this.roomNodeConnectedTo = roomNodeConnectedTo;
        this.roomExitCoridor = roomExitCoridor;
    }

    public void AddRoomConnectionFrom(RoomNode roomNodeConnectedFrom, Coridor roomExitCoridor)
    {
        this.roomNodeConnectedFrom = roomNodeConnectedFrom;
        this.roomEntranceCoridor = roomExitCoridor;
    }

}
