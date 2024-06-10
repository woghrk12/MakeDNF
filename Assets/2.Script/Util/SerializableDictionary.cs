using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    #region Variables

    [SerializeField] private List<K> keyList = new();
    [SerializeField] private List<V> valueList = new();

    #endregion Variables

    #region Implementation ISerializationCallbackReceiver

    public void OnBeforeSerialize()
    {
        keyList.Clear();
        valueList.Clear();

        foreach (KeyValuePair<K, V> pair in this)
        {
            keyList.Add(pair.Key);
            valueList.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();

        if (keyList.Count != valueList.Count)
        {
            throw new Exception($"There are {keyList.Count} keys and {valueList.Count} values after deserialization.");
        }

        int itemCount = keyList.Count <= valueList.Count ? keyList.Count : valueList.Count;

        for (int index = 0; index < itemCount; index++)
        {
            this.Add(keyList[index], valueList[index]);
        }
    }

    #endregion Implementation ISerializationCallbackReceiver
}
