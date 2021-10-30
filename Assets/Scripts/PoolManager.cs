using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{ 
    public static PoolManager Instance { get; private set; }

    private readonly Dictionary<string, Queue<Component>> _poolDictionary = new Dictionary<string, Queue<Component>>();

    public void PoolObject<T>(string poolName, T objectToPool)
    {
        if (_poolDictionary.TryGetValue(poolName, out _))
        {
            _poolDictionary[poolName].Enqueue(objectToPool as Component);
        }
        else
        {
            _poolDictionary.Add(poolName, new Queue<Component>(new List<Component> {objectToPool as Component}));
        }
    }

    public T GetObjectFromPool<T>(string poolName, T objectToGet) where T : class, IPoolable
    {
        if (_poolDictionary.TryGetValue(poolName, out var thisQueue))
        {
            if (thisQueue.Count == 0)
            {
                return CreateNewObject(objectToGet);
            }

            var obj = thisQueue.Dequeue();
            obj.gameObject.SetActive(true);
            return obj as T;
        }
        return CreateNewObject(objectToGet);
    }
    public bool TryGetObjectFromPool<T>(string poolName, out T objectToGet) where T : class, IPoolable
    {
        if (_poolDictionary.TryGetValue(poolName, out var thisQueue))
        {
            if (thisQueue.Count == 0)
            {
                objectToGet = default;
                return false;
            }
            
            objectToGet = thisQueue.Dequeue() as T;
            return true;
        }

        objectToGet = default;
        return false;
    }

    public bool TryGetWholePool<T>(string poolName, out Queue<T> wholePool) where T : class, IPoolable
    {
        if (_poolDictionary.TryGetValue(poolName, out var thisQueue))
        {
            if (thisQueue.Count == 0)
            {
                wholePool = default;
                return false;
            }
            
            wholePool = thisQueue as Queue<T>;
            return true;
        }

        wholePool = default;
        return false;
    }
    private T CreateNewObject<T>(T objectToCreate) where T : class, IPoolable
    {
        var obj = Instantiate(objectToCreate as Component);
        obj.name = (objectToCreate as Component)?.name ?? "poolObj";
        return obj as T;
    }

    public void RemovePool(string poolName)
    {
        _poolDictionary.Remove(poolName);
    }

    public void ClearPool()
    {
        _poolDictionary.Clear();
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}