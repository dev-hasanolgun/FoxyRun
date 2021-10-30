using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameStateMachine GameStateMachine;
    public LevelDatabase LevelDatabase;
    public GameUI GameUI;
    public CameraController CameraController;
    public Runner RunnerObj;
    public Chaser DumbChaser;
    public Chaser DumberChaser;
    public Chaser EvenDumberChaser;
    public List<Chaser> ActiveChasers = new List<Chaser>();
    public List<Chaser> RespawnList = new List<Chaser>();
    public Trap Trap;
    public List<Trap> TrapList = new List<Trap>();
    public MetalCube MetalCube;
    public MetalDoor MetalDoor;
    public List<Obstacle> Obstacles = new List<Obstacle>();
    public List<Obstacle> ActiveObstacles = new List<Obstacle>();
    public int CurrentLevel = 0;
    public float ChaserCappedSpeed;
    public bool IsChasing;
    
    [HideInInspector] 
    public Runner Runner;
    
    private IEnumerator SpawnChasers(Vector3[] waypoints, Waypoint.BotBrain botBrain, int spawnAmount)
    {
        var targetIndex = 0;
        while (true)
        {
            targetIndex++;
            
            var randomPos = Random.insideUnitCircle*2f;
            var pos = new Vector3(randomPos.x,0,randomPos.y);
            Chaser chaser = null;
            switch (botBrain)
            {
                case Waypoint.BotBrain.Dumb:
                    chaser = PoolManager.Instance.GetObjectFromPool("dumbChaser", DumbChaser);
                    break;
                case Waypoint.BotBrain.Dumber:
                    chaser = PoolManager.Instance.GetObjectFromPool("dumberChaser", DumberChaser);
                    break;
                case Waypoint.BotBrain.EvenDumber:
                    chaser = PoolManager.Instance.GetObjectFromPool("evenDumberChaser", EvenDumberChaser);
                    break;
            }
            
            chaser.transform.position = waypoints[0]+Vector3.up+pos;
            chaser.Runner = Runner;
            chaser.CappedSpeed = ChaserCappedSpeed;
            chaser.Waypoints = waypoints;
            chaser.IsChasing = IsChasing;
            
            if (IsChasing)
            {
                chaser.Animator.SetBool("IsRunning", true);
                chaser.StartCoroutine(chaser.Run());
            }
            
            ActiveChasers.Add(chaser);
            
            if (targetIndex == spawnAmount)
            {
                yield break;
            }

            yield return null;
        }
    }

    public void RespawnChasers() // Respawn dead chasers
    {
        if (PoolManager.Instance.TryGetObjectFromPool<Chaser>("respawnChaser", out var chaser))
        {
            var runnerPos = Runner.transform.position;
            var spawnPos = new Vector3(runnerPos.x - 15f, runnerPos.y, runnerPos.z - 2f);
            var waypoints = new[]{spawnPos, Runner.Target};
            chaser.ResetChaser();
            chaser.transform.position = spawnPos;
            chaser.gameObject.SetActive(true);
            ActiveChasers.Add(chaser);
            chaser.Runner = Runner;
            chaser.CappedSpeed = ChaserCappedSpeed;
            chaser.Waypoints = waypoints;
            chaser.IsChasing = IsChasing;
            chaser.Animator.SetBool("IsRunning", true);
            chaser.StartCoroutine(chaser.Run());
        }
    }
    public void PoolChasers()
    {
        PoolManager.Instance.RemovePool("respawnChaser");
        for (int c = 0; c < ActiveChasers.Count; c++)
        {
            ActiveChasers[c].ResetChaser();
            ActiveChasers[c].gameObject.SetActive(false);
        }
    }
    public void PoolTraps()
    {
        for (int i = 0; i < TrapList.Count; i++)
        {
            TrapList[i].MeshRenderer.material.color = Color.white;
            TrapList[i].Collider.isTrigger = false;
            TrapList[i].Rb.isKinematic = true;
            TrapList[i].gameObject.SetActive(false);
            PoolManager.Instance.PoolObject("trap", TrapList[i]);
        }
    }
    public void PoolObstacles()
    {
        for (int i = 0; i < ActiveObstacles.Count; i++)
        {
            ActiveObstacles[i].gameObject.SetActive(false);
            PoolManager.Instance.PoolObject("obstacle", ActiveObstacles[i]);
        }
    }

    private void CreateObstacles()
    {
        var currentLevel = LevelDatabase.LevelDB[CurrentLevel];
        var obstacleList = currentLevel.ObstacleList;
        var randomIndex = Random.Range(0, Obstacles.Count);
        for (int i = 0; i < obstacleList.Count; i++)
        {
            var obstacle = PoolManager.Instance.GetObjectFromPool("obstacle", Obstacles[randomIndex]);
            obstacle.transform.position = obstacleList[i].WorldPos+Vector3.up;
            ActiveObstacles.Add(obstacle);
        }
    }

    private void SetExitDoor()
    {
        var currentLevel = LevelDatabase.LevelDB[CurrentLevel];
        var cellData = currentLevel.CellData;
        var finishPoint = currentLevel.FinishPoint;
        
        var doorCell = cellData[finishPoint.GridPos.x-5 + finishPoint.GridPos.y * currentLevel.GridSize.x];
        
        var c = 2;
        var currentCell = cellData[doorCell.GridPos.x + (doorCell.GridPos.y - c) * currentLevel.GridSize.x];
        while (!currentCell.IsCellEmpty)
        {
            var cubeR = PoolManager.Instance.GetObjectFromPool("metalCube", MetalCube);
            cubeR.transform.position = currentCell.WorldPos + Vector3.up;
            c++;
            currentCell = cellData[doorCell.GridPos.x + (doorCell.GridPos.y - c) * currentLevel.GridSize.x];
        }

        c = 2;
        currentCell = cellData[doorCell.GridPos.x + (doorCell.GridPos.y + c) * currentLevel.GridSize.x];
        while (!currentCell.IsCellEmpty)
        {
            var cubeR = PoolManager.Instance.GetObjectFromPool("metalCube", MetalCube);
            cubeR.transform.position = currentCell.WorldPos + Vector3.up;
            c++;
            currentCell = cellData[doorCell.GridPos.x + (doorCell.GridPos.y + c) * currentLevel.GridSize.x];
        }
        var door = PoolManager.Instance.GetObjectFromPool("metalDoor", MetalDoor);
        door.transform.position = doorCell.WorldPos;
    }
    public void InitializeLevel()
    {
        CurrentLevel = HelperFunctions.ReverseClampToInt(CurrentLevel, 0, LevelDatabase.LevelDB.Count-1);
        var currentLevel = LevelDatabase.LevelDB[CurrentLevel];
        var cellData = currentLevel.CellData; 
        for (int i = 0; i < cellData.Length; i++)
        {
            if (!cellData[i].IsCellEmpty)
            {
                var trap = PoolManager.Instance.GetObjectFromPool("trap", Trap);
                trap.transform.SetParent(transform);
                trap.transform.position = cellData[i].WorldPos;
                TrapList.Add(trap);
            }
        }
        var waypointList = currentLevel.WaypointList;
        for (int i = 0; i < waypointList.Count; i++)
        {
            waypointList[i].IsActivated = false;
        }
        
        CreateObstacles();
        SetExitDoor();
        
        var startPoint = currentLevel.StartPoint;
        var finishPoint = currentLevel.FinishPoint;
    
        Runner = PoolManager.Instance.GetObjectFromPool("runner", RunnerObj);
        Runner.transform.position = startPoint.WorldPos + Vector3.up;
        Runner.Target = finishPoint.WorldPos;

        CameraController.Runner = Runner;
        
        StartSpawning();
    }

    public void StartSpawning()
    {
        var waypointList = LevelDatabase.LevelDB[CurrentLevel].WaypointList;
        
        for (int i = 0; i < waypointList.Count; i++)
        {
            var waypoints = waypointList[i].Waypoints;
            var distance = Vector3.Distance(Runner.transform.position, waypoints[waypoints.Length - 2]);
            var deltaDistance = waypointList[i].Distance / ChaserCappedSpeed * Runner.Speed -1f;
            if (waypointList[i].IsActivated) continue;
            
            if (distance < deltaDistance || waypoints.Length == 2)
            {
                StartCoroutine(SpawnChasers(waypoints,waypointList[i].BotBrainLevel, waypointList[i].SpawnAmount));
                waypointList[i].IsActivated = true;
            }
        }
    }
    private void Start()
    {
        GameStateMachine = new GameStateMachine(this);
        GameStateMachine.SetState(new TutorialState(GameStateMachine));
    }
    private void Update()
    {
        GameStateMachine.CurrentState.Tick();
    }
}