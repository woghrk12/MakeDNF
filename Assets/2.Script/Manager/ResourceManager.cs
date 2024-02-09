using UnityEngine;

public class ResourceManager
{
    #region Variables

    [Header("Database Variables")]
    private EffectData effectData = null;
    private ObjectPoolData objectPoolData = null;

    #endregion Variables

    #region Properties

    public EffectData EffectData => effectData;
    public ObjectPoolData ObjectPoolData => objectPoolData;

    #endregion Properties

    #region Methods

    /// <summary>
    /// Initialize the resource manager class by preloading the database from the asset folder.
    /// </summary>
    public void Init()
    {
        effectData = Load<EffectData>(ResourcePath.EFFECT_DATA);

        if (effectData == null)
        {
            throw new System.Exception($"The path to the effect data is incorrect, or the data is not set. \nInput path : {FilePath.EFFECT_DATA_PATH}");
        }

        objectPoolData = Load<ObjectPoolData>(ResourcePath.OBJECT_POOL_DATA);

        if (objectPoolData == null)
        {
            throw new System.Exception($"The path to the object pool data is incorrect, or the data is not set. \nInput path : {FilePath.OBJECT_POOL_DATA_PATH}");
        }
    }

    /// <summary>
    /// Load the object of the given type existing in the path.
    /// </summary>
    /// <param name="path">The path from the asset folder</param>
    /// <returns>The loaded object</returns>
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    /// <summary>
    /// Instantiate the object existing in the path.
    /// </summary>
    /// <param name="path">The path from the asset folder</param>
    /// <param name="parent">The parent transform of the instantiated object</param>
    /// <returns>The gameobject object instantiated in the scene</returns>
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject source = Load<GameObject>(path);

        if (source == null) return null;

        return GameObject.Instantiate(source, parent) as GameObject;
    }

    #endregion Methods
}
