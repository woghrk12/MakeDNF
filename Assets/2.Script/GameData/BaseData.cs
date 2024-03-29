using System;
using UnityEngine;

/// <summary>
/// The base class of clip sources.
/// The class has attribute data such as clip ID, name, and path.
/// Need to add [Serializable] attribute when inheriting the BaseClip class for editing in the unity editor.
/// </summary>
[Serializable]
public abstract class BaseClip
{
    #region Variables

    /// <summary>
    /// The ID value of the clip.
    /// </summary>
    public int ID = -1;

    /// <summary>
    /// The name value of the clip.
    /// </summary>
    public string Name = string.Empty;
    
    /// <summary>
    /// The path value from the Resources folder.
    /// </summary>
    public string ResourcesPath = string.Empty;
    
    /// <summary>
    /// The full path value from the Assets folder.
    /// </summary>
    public string FullPath = string.Empty;

    #endregion Variables
}

/// <summary>
/// The base class of database of BaseClip.
/// The class has an clip list, file name, and path.
/// The class has the ability to read and write files
/// </summary>
public abstract class BaseData<T> : ScriptableObject where T : BaseClip
{
    #region Variables

    [SerializeField] protected T[] database = new T[0];

    #endregion Variables

    #region Properties

    /// <summary>
    /// The total number of the data.
    /// </summary>
    public int DataCount => database == null ? 0 : database.Length;

    #endregion Properties

    #region Methods

    public void SetData(int index, T data)
    {
        if (index < 0 || index >= DataCount)
        {
            throw new System.Exception($"Out of range. Data name : {name}, Input index : {index}");
        }

        database[index] = data;
    }

    public T GetData(int index)
    {
        if (index < 0 || index >= DataCount)
        {
            throw new System.Exception($"Out of range. Data name : {name}, Input index : {index}");
        }

        return database[index];
    }

    public T[] GetAllData()
    {
        return database;
    }

    public string[] GetNameList(bool isShowID, string filterWord = "")
    {
        if (database == null) return new string[0];

        string[] retList = new string[DataCount];

        for (int index = 0; index < DataCount; index++)
        {
            if (filterWord != "" && !database[index].Name.ToLower().Contains(filterWord.ToLower())) continue;

            retList[index] = isShowID ? index.ToString() + " : " + database[index].Name : database[index].Name;
        }

        return retList;
    }

    #region Abstract

    public abstract T AddData(string newName);
    public abstract T AddData(T newData);
    public abstract T CopyData(int index);
    public abstract void RemoveData(int index);
    public abstract void ClearData();

    #endregion Abstract

    #endregion Methods
}
