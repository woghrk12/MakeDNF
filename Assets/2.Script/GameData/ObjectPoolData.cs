using System;
using UnityEngine;

/// <summary>
/// The clip class of Object Pool.
/// The class has attribute data such as object prefab, name, and path.
/// </summary>
[Serializable]
public class ObjectPoolClip : BaseClip
{
    #region Variables

    public GameObject ObjectPrefab = null;

    #endregion Variables

    #region Methods

    public void ReleaseClip()
    {
        if (ObjectPrefab == null) return;

        ObjectPrefab = null;
    }

    #endregion Methods
}

/// <summary>
/// The database class which containing the object pool clips.
/// The class has an effect clip list, effect file name, and path.
/// The class has the ability to read and write files
/// </summary>
public class ObjectPoolData : BaseData<ObjectPoolClip>
{
    #region Methods

    #region Override

    public override ObjectPoolClip AddData(string newName)
    {
        ObjectPoolClip newClip = new();
        newClip.ID = DataCount;
        newClip.Name = newName;

        database = ArrayHelper.Add(newClip, database);

        return newClip;
    }

    public override ObjectPoolClip AddData(ObjectPoolClip newData)
    {
        if (newData == null)
        {
            throw new Exception("Null Exception : Input data is null");
        }

        ObjectPoolClip newClip = new();
        newClip.ID = DataCount;
        newClip.Name = newData.Name;
        newClip.Path = newData.Path;
        newClip.ObjectPrefab = newData.ObjectPrefab;

        database = ArrayHelper.Add(newClip, database);

        return newClip;
    }

    public override ObjectPoolClip CopyData(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex >= DataCount)
        {
            throw new Exception($"Out of range. Input index : {dataIndex}");
        }

        ObjectPoolClip copiedClip = new();
        copiedClip.ID = DataCount;
        copiedClip.Path = database[dataIndex].Path;
        copiedClip.ObjectPrefab = database[dataIndex].ObjectPrefab;

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
        foreach (ObjectPoolClip clip in database)
        {
            clip.ReleaseClip();
        }

        database = new ObjectPoolClip[0];
    }

    #endregion Override

    #endregion Methods
}
