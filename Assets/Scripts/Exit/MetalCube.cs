using System.Collections.Generic;
using UnityEngine;

public class MetalCube : MonoBehaviour, IPoolable
{
    private void DisableCube(Dictionary<string,object> message)
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        EventManager.StartListening("OnNextLevel", DisableCube);
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnNextLevel", DisableCube);
        PoolManager.Instance.PoolObject("metalCube", this);
    }
}
