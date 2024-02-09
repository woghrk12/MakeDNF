using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    #region Variables

    private Dictionary<EEffectList, GameObject> effectDatabase = new();
    private Dictionary<EEffectList, Queue<GameObject>> effectDictionary = new();

    #endregion Variables

    #region Methods

    /// <summary>
    /// Initialize the effect database by getting the effect data from the asset.
    /// </summary>
    public void Init()
    {
        EffectClip[] database = GameManager.Resource.EffectData.GetAllData();

        foreach (EffectClip effectClip in database)
        {
            effectDatabase.Add((EEffectList)effectClip.ID, effectClip.EffectPrefab);
            effectDictionary.Add((EEffectList)effectClip.ID, new());
        }
    }

    /// <summary>
    /// Spawn the effect object from the effect pool at the given index.
    /// </summary>
    /// <param name="effectIndex">The index of the effect to be spawned</param>
    /// <returns>The effect object</returns>
    public GameObject SpawnFromPool(EEffectList effectIndex)
    {
        if (effectDictionary[effectIndex].Count <= 0)
        {
            CreateNewObject(effectIndex);
        }

        GameObject gameObject = effectDictionary[effectIndex].Dequeue();
        gameObject.SetActive(true);

        return gameObject;
    }

    /// <summary>
    /// Return the effect object to the pool at the given index.
    /// </summary>
    /// <param name="effectIndex">The index of the effect to be returned</param>
    /// <param name="gameObject">The game object of the effect to be returned</param>
    public void ReturnToPool(EEffectList effectIndex, GameObject gameObject)
    {
        gameObject.SetActive(false);

        effectDictionary[effectIndex].Enqueue(gameObject);
    }

    #region Helper

    /// <summary>
    /// Create a new effect object and store it in the queue.
    /// </summary>
    /// <param name="effectIndex">The index of the effect to be created</param>
    private void CreateNewObject(EEffectList effectIndex)
    {
        GameObject gameObject = Instantiate(effectDatabase[effectIndex], transform);

        gameObject.name = effectIndex.ToString();
        effectDictionary[effectIndex].Enqueue(gameObject);

        gameObject.SetActive(false);
    }

    #endregion Helper

    #endregion Methods
}
