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

    /// <summary>
    /// Initialize the object pool database by getting the object pool data from the asset.
    /// </summary>
    public void Init()
    {
        ObjectPoolClip[] database = GameManager.Resource.ObjectPoolData.GetAllData();

        foreach (ObjectPoolClip poolClip in database)
        {
            poolDatabase.Add((EObjectPoolList)poolClip.ID, poolClip.ObjectPrefab);
            poolDictionary.Add((EObjectPoolList)poolClip.ID, new());
        }
    }

    /// <summary>
    /// Spawn the instantiated object from the object pool at the given index.
    /// </summary>
    /// <param name="poolIndex">The index of the object to be spawned</param>
    /// <returns>The prefab object</returns>
    public GameObject SpawnFromPool(EObjectPoolList poolIndex)
    {
        if (poolDictionary[poolIndex].Count <= 0)
        {
            CreateNewObject(poolIndex);
        }

        GameObject gameObject = poolDictionary[poolIndex].Dequeue();
        gameObject.SetActive(true);

        return gameObject;
    }

    /// <summary>
    /// Return the instantiated object to the pool at the given index.
    /// </summary>
    /// <param name="poolIndex">The index of the object pool to return the object to</param>
    /// <param name="gameObject">The game object to be returned</param>
    public void ReturnToPool(EObjectPoolList poolIndex, GameObject gameObject)
    {
        gameObject.SetActive(false);

        poolDictionary[poolIndex].Enqueue(gameObject);
    }

    #region Helper 

    /// <summary>
    /// Create a new object and store it in the queue.
    /// </summary>
    /// <param name="poolIndex">The index of the pool to create an object from</param>
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
