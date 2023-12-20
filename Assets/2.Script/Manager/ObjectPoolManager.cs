using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPool
{
    [HideInInspector] public string tag = string.Empty;
    public GameObject prefab = null;
    public int size = 0;

    public void Init()
    {
        tag = prefab.name;
    }
}

public class ObjectPoolManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private ObjectPool[] objectPools = null;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new();

    #endregion Variables

    #region Methods

    public void Init()
    {
        foreach (ObjectPool objectPool in objectPools)
        {
            objectPool.Init();

            poolDictionary.Add(objectPool.tag, new());

            for (int i = 0; i < objectPool.size; i++)
            {
                CreateNewObject(objectPool.tag, objectPool.prefab);
            }
        }
    }

    public GameObject SpawnFromPool(string tag)
    {
        if (poolDictionary[tag].Count <= 0)
        {
            ObjectPool objectPool = Array.Find(objectPools, x => x.tag == tag);
            CreateNewObject(tag, objectPool.prefab);
        }

        GameObject gameObject = poolDictionary[tag].Dequeue();
        gameObject.SetActive(true);

        return gameObject;
    }

    public void ReturnToPool(GameObject gameObject)
    {
        gameObject.SetActive(false);
        poolDictionary[gameObject.name].Enqueue(gameObject);
    }

    private void CreateNewObject(string tag, GameObject prefabObject)
    {
        GameObject gameObject = Instantiate(prefabObject, transform);

        gameObject.name = tag;
        poolDictionary[tag].Enqueue(gameObject);

        gameObject.SetActive(false);
    }

    #endregion Methods
}
