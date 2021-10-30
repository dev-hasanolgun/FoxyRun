using System;
using UnityEngine;

[Serializable]
public class Waypoint
{
    public Vector3[] Waypoints;
    public int SpawnAmount;
    public float Distance;
    public bool IsActivated;
    public BotBrain BotBrainLevel;
    public enum BotBrain
    {
        EvenDumber, Dumber, Dumb
    }
    
    public Waypoint(Vector3[] waypoints)
    {
        Waypoints = waypoints;
        for (int i = 1; i < Waypoints.Length-1; i++)
        {
            Distance += Vector3.Distance(Waypoints[i - 1], Waypoints[i]);
        }
    }
}
