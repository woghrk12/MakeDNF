using System;
using UnityEngine;

/// <summary>
/// The clip class of Effect.
/// The class has attribute data such as effect prefab, name, and path.
/// </summary>
[Serializable]
public class EffectClip : BaseClip
{
    #region Variables

    public GameObject EffectPrefab = null;

    #endregion Variables

    #region Methods

    public void ReleaseClip()
    {
        if (EffectPrefab == null) return;

        EffectPrefab = null;
    }

    #endregion Methods
}

/// <summary>
/// The database class which containing the effect clips.
/// The class has an effect clip list, effect file name, and path.
/// The class has the ability to read and write files
/// </summary>
public class EffectData : BaseData<EffectClip>
{
    #region Methods

    #region Override

    public override EffectClip AddData(string newName)
    {
        EffectClip newClip = new();
        newClip.ID = DataCount;
        newClip.Name = newName;

        database = ArrayHelper.Add(newClip, database);

        return newClip;
    }

    public override EffectClip AddData(EffectClip newData)
    {
        if (newData == null)
        {
            throw new Exception("Null Exception");
        }

        EffectClip newClip = new();
        newClip.ID = DataCount;
        newClip.Name = newData.Name;
        newClip.Path = newData.Path;
        newClip.EffectPrefab = newData.EffectPrefab;

        database = ArrayHelper.Add(newClip, database);

        return newClip;
    }

    public override EffectClip CopyData(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex >= DataCount)
        {
            throw new Exception($"Out of range. Input index : {dataIndex}");
        }

        EffectClip copiedClip = new();
        copiedClip.ID = DataCount;
        copiedClip.Path = database[dataIndex].Path;
        copiedClip.EffectPrefab = database[dataIndex].EffectPrefab;

        database = ArrayHelper.Add(copiedClip, database);

        return copiedClip;
    }

    public override void RemoveData(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex >= DataCount)
        {
            throw new Exception($"Out of range. Input index : {dataIndex}");
        }

        if (DataCount <= 0) return;

        database = ArrayHelper.Remove(dataIndex, database);

        for (int index = 0; index < DataCount; index++)
        {
            database[index].ID = index;
        }
    }

    public override void ClearData()
    {
        foreach (EffectClip clip in database)
        {
            clip.ReleaseClip();
        }

        database = new EffectClip[0];
    }

    #endregion Override

    #endregion Methods
}
