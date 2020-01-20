using System.Collections.Generic;
using UnityEngine;

public class Grid2
{
    // Grid Data is a List of points in any given cell
    private Dictionary<Vector2, List<Vector2>> gridData;
    private float cellSize;
    private Rect bounds;
    
    public Grid2(float width, float height, float cellSize)
    {
        this.cellSize = cellSize;
        bounds = new Rect(0, 0, width, height);
        gridData = new Dictionary<Vector2, List<Vector2>>();
    }

    /// <summary>
    /// Add a new point to the grid
    /// </summary>
    /// <param name="point">The 2D point to add</param>
    /// <returns>The cell in which the point was added.  A cell return of -1,-1 indicates an error and the point was not added</returns>
    public Vector2 Add(Vector2 point)
    {
        if (!InsideBounds(point)) return new Vector2(-1,-1);
        
        Vector2 cell = PointToCell(point);
        if (!gridData.ContainsKey(cell))
        {
            gridData.Add(cell, new List<Vector2>());
        }
        
        gridData[cell].Add(point);

        return cell;
    }

    /// <summary>
    /// Removes a point from the grid
    /// </summary>
    /// <param name="point">The 2D point to remove</param>
    /// <returns>True if point was remove, False if point wasn't removed or was not found in the grid</returns>
    public bool Remove(Vector2 point)
    {
        if (!Contains(point)) return false;

        Vector2 cell = PointToCell(point);
        return gridData[cell] != null && gridData[PointToCell(point)].Remove(point);
    }

    public List<Vector2> GetPointsInCell(Vector2 cell)
    {
        if (gridData.ContainsKey(cell))
        {
            return gridData[cell];
        }

        return null;
    }

    public int MaxColumn()
    {
        return (int)(bounds.width / cellSize);
    }

    public int MaxRow()
    {
        return (int)(bounds.height / cellSize);
    }

    /// <summary>
    /// Check if point is stored in the grid
    /// </summary>
    /// <param name="point">The 2D point to check</param>
    /// <returns>True if point is stored in the grid, or False if not</returns>
    public bool Contains(Vector2 point)
    {
        if (!InsideBounds(point)) return false;
        
        Vector2 cell = PointToCell(point);
        return gridData.ContainsKey(cell);
    }

    public bool InsideBounds(Vector2 point)
    {
        return bounds.Contains(point);
    }

    /// <summary>
    /// Given a 2D point, determine the Grid cell it would fall into
    /// </summary>
    /// <param name="point">The 2D point</param>
    /// <returns>The grid cell that the point would fall into</returns>
    public Vector2 PointToCell(Vector2 point)
    {
        if (!InsideBounds(point)) return new Vector2(-1,-1);

        var cellX = (int)(point.x / cellSize);
        var cellY = (int)(point.y / cellSize);
        
        return new Vector2(cellX, cellY);
    }
}