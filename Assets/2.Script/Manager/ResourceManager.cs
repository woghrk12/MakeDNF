using UnityEngine;

public class ResourceManager
{
    #region Methods

    public Object Load(string path)
    {
        return Resources.Load(path);
    }

    public GameObject Instantiate(string path)
    {
        Object source = Load(path);

        if (source == null) return null;

        return GameObject.Instantiate(source) as GameObject;
    }

    #endregion Methods
}
