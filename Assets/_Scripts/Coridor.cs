using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coridor
{
    public HashSet<Vector2Int> coridorFloors;
    public int coridorLength = 0;
    public Vector2Int coridorEntrancePosition1;
    public Vector2Int coridorEntrancePosition2;
    public Vector2Int coridorExitPosition;
    public Vector2Int coridorEntranceDirection;
    public Vector2Int coridorExitDirection;

    public Coridor(HashSet<Vector2Int> floors, int length, Vector2Int coridorEntrancePositon1, Vector2Int coridorEntrancePositon2, Vector2Int coridorEntranceDirection)
    {
        this.coridorLength = length;
        this.coridorFloors = floors;
        this.coridorEntrancePosition1 = coridorEntrancePositon1;
        this.coridorEntrancePosition2 = coridorEntrancePositon2;
        this.coridorEntranceDirection = coridorEntranceDirection;
    }
}
