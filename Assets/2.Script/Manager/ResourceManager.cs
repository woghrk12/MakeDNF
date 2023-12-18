using UnityEngine;

public class ResourceManager
{
    #region Methods

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
