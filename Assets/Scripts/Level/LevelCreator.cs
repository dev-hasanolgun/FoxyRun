using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Cell
{
    public Vector3 WorldPos;
    public Vector2Int GridPos;
    public bool IsCellEmpty;

    public Cell(Vector3 worldPos, Vector2Int gridPos, bool isCellEmpty = true)
    {
        WorldPos = worldPos;
        GridPos = gridPos;
        IsCellEmpty = isCellEmpty;
    }
}
public class LevelCreator : MonoBehaviour
{
    [BoxGroup("Grid Settings"), ShowIf("_isCreatingNewLevel")]
    [OnValueChanged("SetGrid")] [VectorRange(0,int.MaxValue,0,int.MaxValue)]
    public Vector2Int GridSize;
    
    [BoxGroup("Grid Settings"), ShowIf("_isCreatingNewLevel")]
    [OnValueChanged("SetGrid")] [MinValue(0.1f)]
    public float CellRadius;
    
    [BoxGroup("Create Level"), HideIf("_isCreatingNewLevel")]
    public LevelDatabase LevelDatabase;
    
    [BoxGroup("Create Level"), HideIf("_isCreatingNewLevel")]
    [OnValueChanged("UpdateProperties")]
    [PropertyRange(0,"_levelLastIndex")]
    [SuppressInvalidAttributeError]
    public int CurrentLevel;
    private int _levelLastIndex;
    
    [BoxGroup("Create Level"), HideIf("_isCreatingNewLevel")]
    [SerializeField]
    private int _levelID;
    
    [BoxGroup("Create Level"), HideIf("_isCreatingNewLevel")]
    [SerializeField] private string _levelName;
    
    [BoxGroup("Create Waypoints"), HideIf("_isCreatingNewLevel")]
    [OnValueChanged("UpdateProperties")] [OnValueChanged("ShowCurrentWaypoints")]
    [PropertyRange(0,"_waypointsLastIndex")]
    public int CurrentWaypoint;
    private int _waypointsLastIndex;

    [BoxGroup("Create Waypoints"), ShowIf("@this._isCreatingNewLevel == false && this._isCreatingWaypoints == true")]
    public int SpawnAmount;

    [BoxGroup("Create Waypoints"), ShowIf("@this._isCreatingNewLevel == false && this._isCreatingWaypoints == true")]
    public Waypoint.BotBrain BotBrainLevel;
    public enum BotBrain
    {
        EvenDumber, Dumber, Dumb
    }
    
    [SerializeField] private bool DisplayGridGizmos;

    private Cell[] _gridMap;
    private List<Vector3> _waypoints = new List<Vector3>();
    private BoxCollider _levelColllider;
    private Cell _currentCell;
    private Cell _startPoint;
    private Cell _finishPoint;
    private Vector2Int _gridSize;
    private float _cellDiameter;
    private bool _isLevelExist;
    private bool _isCreatingNewLevel;
    private bool _isSettingObstacle;
    private bool _isSettingStartPoint;
    private bool _isSettingFinishPoint;
    private bool _isWaypointExist;
    private bool _isCreatingWaypoints;
    
    [BoxGroup("Create Level")]
    [Button("New Level", ButtonSizes.Medium), HideIf("_isCreatingNewLevel")]
    [GUIColor(0,1,0)]
    private void NewLevel()
    {
        _isCreatingNewLevel = true;
        UpdateProperties();
    }
    [BoxGroup("Create Level")]
    [Button("Create Level", ButtonSizes.Medium), ShowIf("_isCreatingNewLevel")]
    [GUIColor(0,1,0)]
    private void CreateLevel()
    {
        LevelDatabase.LevelDB.Add(new Level(transform.position, _gridSize, CellRadius, _levelID,_levelName));
        CurrentLevel += LevelDatabase.LevelDB.Count > 1 ? 1 : 0;
        UpdateProperties();
        _isCreatingNewLevel = false;
        #if UNITY_EDITOR
        EditorUtility.SetDirty(LevelDatabase);
        #endif
    }
    [BoxGroup("Create Level")]
    [Button("Delete Level", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingNewLevel == false")]
    [GUIColor(1,0,0)]
    private void DeleteLevel()
    {
        LevelDatabase.LevelDB.RemoveAt(CurrentLevel);
        CurrentLevel -= 1;
        UpdateProperties();
        ShowCurrentWaypoints();
        #if UNITY_EDITOR
        EditorUtility.SetDirty(LevelDatabase);
        #endif
    }
    [BoxGroup("Create Level")]
    [Button("Cancel", ButtonSizes.Medium), ShowIf("_isCreatingNewLevel")]
    [GUIColor(1,0,0)]
    private void CancelLevelCreation()
    {
        _isCreatingNewLevel = false;
        UpdateProperties();
    }
    [BoxGroup("Create Level/Set Start-Finish Points")]
    [Button("New Start Point", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isSettingStartPoint == false && this._isSettingFinishPoint == false")]
    [GUIColor(0,1,0)]
    private void NewStartPoint()
    {
        _isSettingStartPoint = true;
        UpdateProperties();
    }
    [BoxGroup("Create Level/Set Obstacles")]
    [Button("Set Obstacles", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isSettingObstacle == false")]
    [GUIColor(0,1,0)]
    private void SetObstacles()
    {
        _isSettingObstacle = true;
        UpdateProperties();
    }
    [BoxGroup("Create Level/Set Obstacles")]
    [Button("Cancel", ButtonSizes.Medium), ShowIf("_isSettingObstacle")]
    [GUIColor(1,0,0)]
    private void CancelObstacleSet()
    {
        _isSettingObstacle = false;
        UpdateProperties();
    }
    [BoxGroup("Create Level/Set Start-Finish Points")]
    [Button("New Finish Point", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isSettingFinishPoint == false && this._isSettingStartPoint == false")]
    [GUIColor(0,1,0)]
    private void NewFinishPoint()
    {
        _isSettingFinishPoint = true;
        UpdateProperties();
    }
    [BoxGroup("Create Level/Set Start-Finish Points")]
    [Button("Set Start Point", ButtonSizes.Medium), ShowIf("_isSettingStartPoint")]
    [GUIColor(0,0,1)]
    private void SetStartPoint()
    {
        LevelDatabase.LevelDB[CurrentLevel].StartPoint = _startPoint;
        UpdateProperties();
        _isSettingStartPoint = false;
        #if UNITY_EDITOR
        EditorUtility.SetDirty(LevelDatabase);
        #endif
    }
    [BoxGroup("Create Level/Set Start-Finish Points")]
    [Button("Set Finish Point", ButtonSizes.Medium), ShowIf("_isSettingFinishPoint")]
    [GUIColor(0,0,1)]
    private void SetFinishPoint()
    {
        LevelDatabase.LevelDB[CurrentLevel].FinishPoint = _finishPoint;
        UpdateProperties();
        _isSettingFinishPoint = false;
        #if UNITY_EDITOR
        EditorUtility.SetDirty(LevelDatabase);
        #endif
    }
    [BoxGroup("Create Level/Set Start-Finish Points")]
    [Button("Cancel", ButtonSizes.Medium), ShowIf("@this._isSettingFinishPoint || this._isSettingStartPoint")]
    [GUIColor(1,0,0)]
    private void CancelSettingPoint()
    {
        _isSettingStartPoint = false;
        _isSettingFinishPoint = false;
        UpdateProperties();
    }
    [BoxGroup("Create Waypoints")]
    [Button("New Waypoints", ButtonSizes.Medium), ShowIf("@this._isLevelExist && !this._isCreatingWaypoints == true")]
    [GUIColor(0,1,0)]
    private void NewWaypoints()
    {
        _waypoints.Clear();
        _isCreatingWaypoints = true;
        UpdateProperties();
    }
    [BoxGroup("Create Waypoints")]
    [Button("Save Waypoints", ButtonSizes.Medium), ShowIf("@this._isLevelExist && this._isCreatingWaypoints == true")]
    [GUIColor(0,0,1)]
    private void SaveWaypoints()
    {
        LevelDatabase.LevelDB[CurrentLevel].WaypointList.Add(new Waypoint(_waypoints.ToArray()));
        CurrentWaypoint += LevelDatabase.LevelDB[CurrentLevel].WaypointList.Count > 1 ? 1 : 0;
        LevelDatabase.LevelDB[CurrentLevel].WaypointList[CurrentWaypoint].SpawnAmount = SpawnAmount;
        LevelDatabase.LevelDB[CurrentLevel].WaypointList[CurrentWaypoint].BotBrainLevel = BotBrainLevel;
        UpdateProperties();
        ShowCurrentWaypoints();
        _isCreatingWaypoints = false;
        #if UNITY_EDITOR
        EditorUtility.SetDirty(LevelDatabase);
        #endif
    }
    [BoxGroup("Create Waypoints")]
    [Button("Delete Waypoints", ButtonSizes.Medium), ShowIf("@this._isWaypointExist && !this._isCreatingWaypoints == true")]
    [GUIColor(1,0,0)]
    private void DeleteWaypoints()
    {
        LevelDatabase.LevelDB[CurrentLevel].WaypointList.RemoveAt(CurrentWaypoint);
        CurrentWaypoint -= LevelDatabase.LevelDB[CurrentLevel].WaypointList.Count > 1 ? 1 : 0;
        UpdateProperties();
        ShowCurrentWaypoints();
        #if UNITY_EDITOR
        EditorUtility.SetDirty(LevelDatabase);
        #endif
    }
    [BoxGroup("Create Waypoints")]
    [Button("Cancel", ButtonSizes.Medium), ShowIf("_isCreatingWaypoints")]
    [GUIColor(1,0,0)]
    private void CancelWaypointCreation()
    {
        _isCreatingWaypoints = false;
        UpdateProperties();
    }
    
    private void UpdateProperties()
    {
        _isLevelExist = LevelDatabase.LevelDB.Count != 0;

        if (_isLevelExist)
        {
            _isWaypointExist = LevelDatabase.LevelDB[CurrentLevel].WaypointList.Count != 0;
            
            _levelLastIndex = Mathf.Clamp(LevelDatabase.LevelDB.Count - 1,0,int.MaxValue);
            CurrentLevel = Mathf.Clamp(CurrentLevel, 0, _levelLastIndex);
            _waypointsLastIndex = Mathf.Clamp(LevelDatabase.LevelDB[CurrentLevel].WaypointList.Count-1,0,int.MaxValue);
            CurrentWaypoint = Mathf.Clamp(CurrentWaypoint, 0, _waypointsLastIndex);
        }
        else
        {
            _isWaypointExist = false;
            
            _levelLastIndex = 0;
            CurrentLevel = 0;
            _waypointsLastIndex = 0;
            CurrentWaypoint = 0;
        }
    }

    private void ShowCurrentWaypoints()
    {
        _waypoints.Clear();
        
        if (_isWaypointExist)
        {
            var waypoints = LevelDatabase.LevelDB[CurrentLevel].WaypointList[CurrentWaypoint].Waypoints;

            for (int i = 0; i < waypoints.Length; i++)
            {
                _waypoints.Add(waypoints[i]);
            }

            SpawnAmount = LevelDatabase.LevelDB[CurrentLevel].WaypointList[CurrentWaypoint].SpawnAmount;
            BotBrainLevel = LevelDatabase.LevelDB[CurrentLevel].WaypointList[CurrentWaypoint].BotBrainLevel;
        }
    }
    public void CreateObject()
    {
        var mouseRightClicked = Event.current.type == EventType.MouseDown && Event.current.button == 1;
        var mouseRightDragged = Event.current.type == EventType.MouseDrag && Event.current.button == 1;
        
        var mouseMiddleClicked = Event.current.type == EventType.MouseDown && Event.current.button == 2;
        var mouseMiddleDragged = Event.current.type == EventType.MouseDrag && Event.current.button == 2;

        if (!_isLevelExist || _isCreatingNewLevel) return;

        if (_isCreatingWaypoints)
        {
            if (Event.current.control && (mouseRightClicked || mouseRightDragged))
            {
                Event.current.Use();
                if (!_waypoints.Contains(_currentCell.WorldPos))
                {
                    _waypoints.Add(_currentCell.WorldPos+Vector3.up);
                }
            }
        
            if (Event.current.control && (mouseMiddleClicked || mouseMiddleDragged))
            {
                Event.current.Use();
                _waypoints.Remove(_currentCell.WorldPos+Vector3.up);
            }
        }
        else if (_isSettingObstacle)
        {
            if (Event.current.control && (mouseRightClicked || mouseRightDragged))
            {
                Event.current.Use();
                if (!LevelDatabase.LevelDB[CurrentLevel].ObstacleList.Contains(_currentCell))
                {
                    LevelDatabase.LevelDB[CurrentLevel].ObstacleList.Add(_currentCell);
                }
                #if UNITY_EDITOR
                EditorUtility.SetDirty(LevelDatabase);
                #endif
            }
        
            if (Event.current.control && (mouseMiddleClicked || mouseMiddleDragged))
            {
                Event.current.Use();
                LevelDatabase.LevelDB[CurrentLevel].ObstacleList.Remove(_currentCell);
                #if UNITY_EDITOR
                EditorUtility.SetDirty(LevelDatabase);
                #endif
            }
        }
        else if (_isSettingStartPoint)
        {
            if (Event.current.control && mouseRightClicked)
            {
                Event.current.Use();
                _startPoint = _currentCell;
                #if UNITY_EDITOR
                EditorUtility.SetDirty(LevelDatabase);
                #endif
            }
        }
        else if (_isSettingFinishPoint)
        {
            if (Event.current.control && mouseRightClicked)
            {
                Event.current.Use();
                _finishPoint = _currentCell;
                #if UNITY_EDITOR
                EditorUtility.SetDirty(LevelDatabase);
                #endif
            }
        }
        else
        {
            if (Event.current.control && (mouseRightClicked || mouseRightDragged))
            {
                Event.current.Use();
                LevelDatabase.LevelDB[CurrentLevel].CellData[_currentCell.GridPos.x+_currentCell.GridPos.y*_gridSize.x].IsCellEmpty = false;
                #if UNITY_EDITOR
                EditorUtility.SetDirty(LevelDatabase);
                #endif
            }
        
            if (Event.current.control && (mouseMiddleClicked || mouseMiddleDragged))
            {
                Event.current.Use();
                LevelDatabase.LevelDB[CurrentLevel].CellData[_currentCell.GridPos.x+_currentCell.GridPos.y*_gridSize.x].IsCellEmpty = true;
                #if UNITY_EDITOR
                EditorUtility.SetDirty(LevelDatabase);
                #endif
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        UpdateProperties();
        if (_isCreatingNewLevel || !_isLevelExist)
        {
            Gizmos.DrawWireCube(transform.position,new Vector3(GridSize.x,1,GridSize.y));
            if (_gridMap != null && DisplayGridGizmos) 
            {
                foreach (Cell c in _gridMap) 
                {
                    Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
                    Gizmos.DrawWireCube(c.WorldPos, Vector3.one * (_cellDiameter-.1f));
                }
            }
        }
        else
        {
            _levelID = LevelDatabase.LevelDB[CurrentLevel].LevelID;
            _levelName = LevelDatabase.LevelDB[CurrentLevel].LevelName;
            
            Vector2 mousePosition = Event.current.mousePosition;
            #if UNITY_EDITOR
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _currentCell = CellFromWorldPos(hit.point);
                Gizmos.color = Color.blue;
                if (_isCreatingWaypoints)
                {
                    Gizmos.DrawSphere(_currentCell.WorldPos, _cellDiameter/2f); 
                }
                else if (_isSettingObstacle)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(_currentCell.WorldPos, Vector3.one * (_cellDiameter-.1f));
                }
                else if (_isSettingStartPoint || _isSettingFinishPoint)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(_currentCell.WorldPos, _cellDiameter/2f);
                }
                else
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(_currentCell.WorldPos, Vector3.one * (_cellDiameter-.1f));
                }
            }

            var gridSize = LevelDatabase.LevelDB[CurrentLevel].GridSize;
            Gizmos.DrawWireCube(transform.position,new Vector3(gridSize.x,1,gridSize.y));
            if (LevelDatabase.LevelDB[CurrentLevel].CellData != null && DisplayGridGizmos) 
            {
                foreach (Cell c in LevelDatabase.LevelDB[CurrentLevel].CellData) 
                {
                    Gizmos.color = c.IsCellEmpty?new Color(1f, 1f, 0f, 0.05f):new Color(1f, 0f, 0f, 0.5f);
                    Gizmos.DrawWireCube(c.WorldPos, Vector3.one * (_cellDiameter-.1f));
                }
            }

            if (LevelDatabase.LevelDB[CurrentLevel].ObstacleList.Count > 0 && DisplayGridGizmos)
            {
                foreach (Cell c in LevelDatabase.LevelDB[CurrentLevel].ObstacleList)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(c.WorldPos, Vector3.one * (_cellDiameter-.1f));
                }
            }
            if ((_isSettingStartPoint || _isSettingFinishPoint) && DisplayGridGizmos)
            {
                Gizmos.color = new Color(0f, 1f, 0.78f, 0.7f);
                if (_startPoint != null)
                {
                    Gizmos.DrawSphere(_startPoint.WorldPos, _cellDiameter/2f);
                }

                if (_finishPoint != null)
                {
                    Gizmos.DrawSphere(_finishPoint.WorldPos, _cellDiameter/2f);
                }
            }
            else if (LevelDatabase.LevelDB[CurrentLevel].CellData != null && DisplayGridGizmos) 
            {
                Gizmos.color = new Color(0f, 1f, 0.78f, 0.7f);
                Gizmos.DrawSphere(LevelDatabase.LevelDB[CurrentLevel].StartPoint.WorldPos, _cellDiameter/2f);
                Gizmos.DrawSphere(LevelDatabase.LevelDB[CurrentLevel].FinishPoint.WorldPos, _cellDiameter/2f);
            }
            
            if (_isCreatingWaypoints)
            {
                if (_waypoints.Count > 0 && DisplayGridGizmos) 
                {
                    foreach (Vector3 c in _waypoints) 
                    {
                        Gizmos.color = new Color(1f, 0f, 1f, 0.7f);
                        Gizmos.DrawSphere(c-Vector3.up, _cellDiameter/2f);
                    }
                }
            }
            else if (LevelDatabase.LevelDB[CurrentLevel].WaypointList.Count > 0 && DisplayGridGizmos) 
            {
                foreach (Vector3 c in LevelDatabase.LevelDB[CurrentLevel].WaypointList[CurrentWaypoint].Waypoints) 
                {
                    Gizmos.color = new Color(1f, 0f, 1f, 0.7f);
                    Gizmos.DrawSphere(c-Vector3.up, _cellDiameter/2f);
                }
            }
#endif
        }
    }
    [OnInspectorInit]
    private void SetGrid()
    {
        _cellDiameter = CellRadius*2;
        var x = Mathf.RoundToInt(GridSize.x/_cellDiameter);
        var y = Mathf.RoundToInt(GridSize.y/_cellDiameter);
        _gridSize = new Vector2Int(x,y);
        GenerateGrid();
        _levelColllider = GetComponent<BoxCollider>();
        _levelColllider.center = transform.position;
        _levelColllider.size = new Vector3(GridSize.x, 0, GridSize.y);
        UpdateProperties();
    }
    public void GenerateGrid()
    {
        _gridMap = new Cell[_gridSize.x * _gridSize.y];
        var worldBottomLeft = transform.position - Vector3.right * GridSize.x/2 - Vector3.forward * GridSize.y/2;

        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                var pos = worldBottomLeft + Vector3.right * (i * _cellDiameter + CellRadius) + Vector3.forward * (j * _cellDiameter + CellRadius);
                var cell = new Cell(pos, new Vector2Int(i,j));
                _gridMap[i+j*_gridSize.x] = cell;
            }
        }
    }
    public Cell CellFromWorldPos(Vector3 value)
    {
        var percentX = (value.x + GridSize.x/2f) / GridSize.x;
        var percentY = (value.z + GridSize.y/2f) / GridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        var x = Mathf.RoundToInt((_gridSize.x-1) * percentX);
        var y = Mathf.RoundToInt((_gridSize.y-1) * percentY);
        
        return _gridMap[x+y*_gridSize.x];
    }
    private void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
    }
}
