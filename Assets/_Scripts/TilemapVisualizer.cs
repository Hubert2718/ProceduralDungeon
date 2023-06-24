using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap;
    [SerializeField]
    private Tilemap wallTilemap;
    [SerializeField]
    private Tilemap objectTilemap;
    [SerializeField]
    private TileBase floorTile_1, floorTile_2, floorTile_3, floorTile_4, floorTile_5, floorTile_6,
        floorTile_7, floorTile_8, floorTile_9, floorTile_10, floorTile_11, floorTile_12;
    [SerializeField]
    private TileBase wallTop_1, wallTop_2, wallTop_3, wallTop_4;
    [SerializeField]
    private TileBase wallSideRight, wallSideLeft, wallBottom, wallFull,
        wallInnerCornerDownLeft, wallInnerCornerDownRight, wallDiagonalCornerDownRight, 
        wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;
    [SerializeField]
    private TileBase doors_up_left, doors_up_right, doors_down_left, doors_down_right, doors_left_up, doors_left_down, doors_right_up, doors_right_down;

    [SerializeField]
    private TileBase corner_web, corner_skull, corner_bones, corner_fire_1, corner_fire_2;

    private List<TileBase> floorTileBases = new List<TileBase>();
    private List<TileBase> wallTopTileBases;
    private List<TileBase> cornerTileBases = new List<TileBase>();


    // Do poprawy !!!!!!!!!!
    public List<Vector2Int> corners = new List<Vector2Int>();

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        floorTileBases = createFloorTileBasesList();
        wallTopTileBases = createWallTileTopList();
        PaintTiles(floorPositions, floorTilemap, floorTile_1);
    }


    public void PaintDoorsTiles(List<RoomNode> roomNodes) 
    {
        foreach (var room in roomNodes)
        {
            Coridor coridor = room.roomEntranceCoridor;
            if (coridor != null)
            {
                var tilePosition1 = objectTilemap.WorldToCell((Vector3Int)coridor.coridorEntrancePosition1);
                var tilePosition2 = objectTilemap.WorldToCell((Vector3Int)coridor.coridorEntrancePosition2);

                if (coridor.coridorEntranceDirection == Vector2Int.up)
                {
                    objectTilemap.SetTile(tilePosition1, doors_up_left);
                    objectTilemap.SetTile(tilePosition2, doors_up_right);
                }
                else if (coridor.coridorEntranceDirection == Vector2Int.down)
                {
                    objectTilemap.SetTile(tilePosition1, doors_down_left);
                    objectTilemap.SetTile(tilePosition2, doors_down_right);
                }
                else if (coridor.coridorEntranceDirection == Vector2Int.left)
                {
                    objectTilemap.SetTile(tilePosition1, doors_left_down);
                    objectTilemap.SetTile(tilePosition2, doors_left_up);
                }
                else if (coridor.coridorEntranceDirection == Vector2Int.left)
                {
                    objectTilemap.SetTile(tilePosition1, doors_right_down);
                    objectTilemap.SetTile(tilePosition2, doors_right_up);
                }
            }
        }
        PaintCornerTiles();
    }

    public void PaintCornerTiles()
    {
        cornerTileBases = createCornerTileList();
        foreach (var corrner in corners)
        {
            TileBase tile= cornerTileBases[UnityEngine.Random.Range(0, cornerTileBases.Count)];
            var tilePosition = objectTilemap.WorldToCell((Vector3Int)corrner);
            objectTilemap.SetTile(tilePosition, tile);
        }
    }


    private void PaintTiles(IEnumerable<Vector2Int> floorPositions, Tilemap floorTilemap, TileBase floorTile)
    {
        
        foreach (var position in floorPositions)
        {
            TileBase tile = floorTileBases[UnityEngine.Random.Range(0, floorTileBases.Count)];
            PaintSingleTile(floorTilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap floorTilemap, TileBase floorTile, Vector2Int position)
    {
        var tilePosition = floorTilemap.WorldToCell((Vector3Int)position);
        floorTilemap.SetTile(tilePosition, floorTile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        objectTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryValue)
    {
        //Debug.Log(position + " type: " + binaryValue);
        int binaryAsInt = Convert.ToInt32(binaryValue, 2);
        TileBase tile = null;
        if (WallByteTypesHelper.wallTop.Contains(binaryAsInt))
        {
            tile = wallTopTileBases[UnityEngine.Random.Range(0, wallTopTileBases.Count)];
        }
        else if (WallByteTypesHelper.wallSideRight.Contains(binaryAsInt))
        {
            tile = wallSideRight;
        }
        else if (WallByteTypesHelper.wallSideLeft.Contains(binaryAsInt))
        {
            tile = wallSideLeft;
        }
        else if (WallByteTypesHelper.wallBottm.Contains(binaryAsInt))
        {
            tile = wallBottom;
        }
        else if (WallByteTypesHelper.wallFull.Contains(binaryAsInt))
        {
            tile = wallFull;
        }
        else if (WallByteTypesHelper.wallFullEightDirections.Contains(binaryAsInt))
        {
            tile = wallBottom;
        }
        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryValue)
    {
        int binaryAsInt = Convert.ToInt32(binaryValue, 2);
        TileBase tile = null;
        if (WallByteTypesHelper.wallInnerCornerDownLeft.Contains(binaryAsInt))
        {
            tile = wallInnerCornerDownLeft;
            //corners.Add(position + Vector2Int.up + Vector2Int.right);
        }
        else if (WallByteTypesHelper.wallInnerCornerDownRight.Contains(binaryAsInt))
        {
            tile = wallInnerCornerDownRight;
            //corners.Add(position + Vector2Int.up + Vector2Int.left);
        }
        else if (WallByteTypesHelper.wallDiagonalCornerUpRight.Contains(binaryAsInt))
        {
            tile = wallDiagonalCornerUpRight;
            corners.Add(position + Vector2Int.down + Vector2Int.left);
        }
        else if (WallByteTypesHelper.wallDiagonalCornerUpLeft.Contains(binaryAsInt))
        {
            tile = wallDiagonalCornerUpLeft;
            //corners.Add(position);
        }
        else if (WallByteTypesHelper.wallDiagonalCornerDownRight.Contains(binaryAsInt))
        {
            tile = wallDiagonalCornerDownRight;
            //.Add(position);
        }
        else if (WallByteTypesHelper.wallDiagonalCornerDownLeft.Contains(binaryAsInt))
        {
            tile = wallDiagonalCornerDownLeft;
            //corners.Add(position);
        }
        else if (WallByteTypesHelper.wallFullEightDirections.Contains(binaryAsInt))
        {
            tile = wallFull; 
        }

        if (tile != null)
        {
            PaintSingleTile(floorTilemap, tile, position);
        }
    }

    private List<TileBase> createFloorTileBasesList()
    {
        List<TileBase> tileBases = new List<TileBase>();
        tileBases.Add(floorTile_1);
        tileBases.Add(floorTile_2);
        tileBases.Add(floorTile_3);
        tileBases.Add(floorTile_4);
        tileBases.Add(floorTile_5);
        tileBases.Add(floorTile_6);
        tileBases.Add(floorTile_7);
        tileBases.Add(floorTile_8);
        tileBases.Add(floorTile_9);
        tileBases.Add(floorTile_10);
        tileBases.Add(floorTile_11);
        tileBases.Add(floorTile_12);

        return tileBases;
    }
    private List<TileBase> createWallTileTopList()
    {
        List<TileBase> wallTails = new List<TileBase>();
        wallTails.Add(wallTop_1);
        wallTails.Add(wallTop_2);
        wallTails.Add(wallTop_3);
        wallTails.Add(wallTop_4);

        return wallTails;
    }

    private List<TileBase> createCornerTileList()
    {
        List<TileBase> cornerTails = new List<TileBase>();
        cornerTails.Add(corner_web);
        cornerTails.Add(corner_skull);
        cornerTails.Add(corner_bones);
        cornerTails.Add(corner_fire_1);
        cornerTails.Add(corner_fire_2);

        return cornerTails;
    }

}
