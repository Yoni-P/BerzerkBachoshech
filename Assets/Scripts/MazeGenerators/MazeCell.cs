using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A class representing a single cell in the maze
 */
public class MazeCell
{
    // The type of cell
    public String cellType;
    // The coordinates of the cell in maze space
    public Vector3Int cellCorrdinates { get; }
    // The four borders defined for this cell
    public Dictionary<Vector2Int, Border> borders { get; }
    
    private static Grid _cellGrid;
    
    // The upper border of the cell
    public Border upperBorder
    { 
        get => borders[Vector2Int.up];
        set => borders[Vector2Int.up] = value;
    }
    
    // The right border of the cell
    public Border rightBorder
    { 
        get => borders[Vector2Int.right];
        set => borders[Vector2Int.right] = value;
    }
    
    // The bottom border of the cell
    public Border lowerBorder
    { 
        get => borders[Vector2Int.down];
        set => borders[Vector2Int.down] = value;
    }
    
    // The left border of the cell
    public Border leftBorder
    { 
        get => borders[Vector2Int.left];
        set => borders[Vector2Int.left] = value;
    }

    /**
     * Constructor for creating a MazeCell instance
     */
    public MazeCell(Vector3Int cellCoordinates)
    {
        _cellGrid = GameManager.Instance.MazeCellGrid;
        cellCorrdinates = cellCoordinates;

        borders = new Dictionary<Vector2Int, Border>();
        borders.Add(Vector2Int.up, Border.UNDEFINED);
        borders.Add(Vector2Int.right, Border.UNDEFINED);
        borders.Add(Vector2Int.down, Border.UNDEFINED);
        borders.Add(Vector2Int.left, Border.UNDEFINED);
    }

    /**
     * A method for receiving the center of the maze cell in world coordinates
     */
    public Vector3 MazeCellCenterToWorld()
    {
        return _cellGrid.GetCellCenterWorld(new Vector3Int(-cellCorrdinates.x, cellCorrdinates.y));
    }
}
