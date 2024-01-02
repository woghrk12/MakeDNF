using UnityEngine;

public class ResourceManager
{
    #region Variables

    private EffectData effectData = null;
    private ObjectPoolData objectPoolData = null;

    #endregion Variables

    #region Properties

    public EffectData EffectData => effectData;
    public ObjectPoolData ObjectPoolData => objectPoolData;

    #endregion Properties

    #region Methods

    public void Init()
    {
        effectData = Load<EffectData>(ResourcePath.EFFECT_DATA);

        if (effectData == null)
        {
            throw new System.Exception($"The path to the effect data is incorrect, or the data is not set. \nInput path : {FilePath.EffectDataPath}");
        }

        objectPoolData = Load<ObjectPoolData>(ResourcePath.OBJECT_POOL_DATA);

        if (objectPoolData == null)
        {
            throw new System.Exception($"The path to the object pool data is incorrect, or the data is not set. \nInput path : {FilePath.OBJECT_POOL_DATA_PATH}");
        }
    }

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject source = Load<GameObject>(path);

        if (source == null) return null;

        return GameObject.Instantiate(source, parent) as GameObject;
    }

    #endregion Methods
}
