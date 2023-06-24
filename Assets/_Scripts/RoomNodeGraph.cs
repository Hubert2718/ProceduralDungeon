using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class RoomNodeGraph
{
    public List<RoomNode> roomNodesList = new List<RoomNode>();
    

    public void AddRoomToGraph(RoomNode roomNode)
    {
        roomNodesList.Add(roomNode);

    }

    public HashSet<Vector2Int> CreateGraphFloor()
    {
        HashSet<Vector2Int> graphFloor = new HashSet<Vector2Int>();

        foreach (RoomNode roomNode in roomNodesList)
        {
            graphFloor.UnionWith(roomNode.roomFloors);
            if (roomNode.roomExitCoridor != null)
                graphFloor.UnionWith(roomNode.roomExitCoridor.coridorFloors);
        }

        return graphFloor;
    }

    public void DijkstraShortestPath(RoomNode source)
    {
        //TODO
    }
}