using System.Collections.Generic;
using UnityEngine;

public class MetalDoor : MonoBehaviour, IPoolable
{
    private void DisableDoor(Dictionary<string,object> message)
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        EventManager.StartListening("OnNextLevel", DisableDoor);
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnNextLevel", DisableDoor);
        PoolManager.Instance.PoolObject("metalDoor", this);
    }
}
