using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Level
{
    public Cell[] CellData;
    public Cell StartPoint;
    public Cell FinishPoint;
    public List<Waypoint> WaypointList;
    public List<Cell> ObstacleList = new List<Cell>();
    public Vector3 GridPos;
    public Vector2Int GridSize;
    public float CellRadius;
    public int LevelID;
    public string LevelName;

    private float _cellDiameter;

    public Level(Vector3 gridPos, Vector2Int gridSize, float cellRadius, int levelID, string levelName)
    {
        GridPos = gridPos;
        GridSize = gridSize;
        CellRadius = cellRadius;
        _cellDiameter = CellRadius * 2;
        LevelID = levelID;
        LevelName = levelName;
        WaypointList = new List<Waypoint>();
        CreateGrid();
    }
    public void CreateGrid()
    {
        CellData = new Cell[GridSize.x*GridSize.y];
        var worldBottomLeft = GridPos - Vector3.right * GridSize.x/2 - Vector3.forward * GridSize.y/2;

        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.y; j++)
            {
                var pos = worldBottomLeft + Vector3.right * (i * _cellDiameter + CellRadius) + Vector3.forward * (j * _cellDiameter + CellRadius);
                var cell = new Cell(pos, new Vector2Int(i,j));
                CellData[i+j*GridSize.x] = cell;
            }
        }
    }
}
