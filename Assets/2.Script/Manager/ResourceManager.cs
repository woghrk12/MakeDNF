using UnityEngine;

public class ResourceManager
{
    #region Methods

    public Object Load(string path)
    {
        return Resources.Load(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        Object source = Load(path);

        if (source == null) return null;

        return GameObject.Instantiate(source, parent) as GameObject;
    }

    #endregion Methods
}
