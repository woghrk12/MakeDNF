using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    #region Variables

    private Dictionary<EObjectPoolList, GameObject> poolDatabase = new();
    private Dictionary<EObjectPoolList, Queue<GameObject>> poolDictionary = new();

    #endregion Variables

    #region Methods

    public void Init()
    {
        ObjectPoolClip[] database = GameManager.Resource.ObjectPoolData.GetAllData();

        foreach (ObjectPoolClip poolClip in database)
        {
            poolDatabase.Add((EObjectPoolList)poolClip.ID, poolClip.ObjectPrefab);
            poolDictionary.Add((EObjectPoolList)poolClip.ID, new());
        }
    }

    public GameObject SpawnFromPool(EObjectPoolList poolIndex)
    {
        if (poolDictionary[poolIndex].Count <= 0)
        {
            CreateNewObject(poolIndex);
        }

        GameObject gameObject = poolDictionary[poolIndex].Dequeue();

        return gameObject;
    }

    public void ReturnToPool(EObjectPoolList poolIndex, GameObject gameObject)
    {
        poolDictionary[poolIndex].Enqueue(gameObject);
    }

    #region Helper 

    private void CreateNewObject(EObjectPoolList poolIndex)
    {
        GameObject gameObject = Instantiate(poolDatabase[poolIndex], transform);

        gameObject.name = poolIndex.ToString();
        poolDictionary[poolIndex].Enqueue(gameObject);

        gameObject.SetActive(false);
    }

    #endregion Helper

    #endregion Methods
}
