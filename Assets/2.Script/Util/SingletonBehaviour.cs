using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : Component
{
    #region Variables

    private static T instance = null;

    #endregion Variables

    #region Properties

    public static T Instance
    {
        get
        {
            if (ReferenceEquals(instance, null))
            {
                instance = FindObjectOfType(typeof(T)) as T;
                
                if (ReferenceEquals(instance, null))
                {
                    instance = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();
                }
            }

            return instance;
        }
    }

    #endregion Properties

    #region Unity Events

    protected virtual void Awake()
    {
        T[] objects = FindObjectsOfType<T>();

        if (objects.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        if (transform.parent != null && transform.root != null)
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion Unity Events
}
