using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    #region Variables

    private Dictionary<EEffectList, GameObject> effectDatabase = new();
    private Dictionary<EEffectList, Queue<GameObject>> effectDictionary = new();

    #endregion Variables

    #region Methods

    public void Init()
    {
        EffectClip[] database = GameManager.Resource.EffectData.GetAllData();

        foreach (EffectClip effectClip in database)
        {
            effectDatabase.Add((EEffectList)effectClip.ID, effectClip.EffectPrefab);
            effectDictionary.Add((EEffectList)effectClip.ID, new());
        }
    }

    public GameObject SpawnEffect(EEffectList effectIndex)
    {
        if (effectDictionary[effectIndex].Count <= 0)
        {
            CreateNewObject(effectIndex);
        }

        GameObject gameObject = effectDictionary[effectIndex].Dequeue();

        return gameObject;
    }

    public void ReturnEffect(EEffectList effectIndex, GameObject gameObject)
    {
        effectDictionary[effectIndex].Enqueue(gameObject);
    }

    #region Helper

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
