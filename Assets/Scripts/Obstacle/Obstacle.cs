using UnityEngine;

public class Obstacle : MonoBehaviour, IPoolable
{
    public float Speed;
    
    private void Update()
    {
        transform.Rotate(Vector3.up,Time.deltaTime*Speed);
    }
}
