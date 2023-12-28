using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericBehaviour : MonoBehaviour
{
    #region Variables

    protected BehaviourController controller = null;

    protected int behaviourCode = 0;

    #endregion Variables

    #region Properties

    public int BehaviourCode => behaviourCode;

    #endregion Properties

    #region Unity Events

    protected virtual void Awake()
    {
        controller = GetComponent<BehaviourController>();

        behaviourCode = GetType().GetHashCode();
    }

    #endregion Unity Events

    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnComplete() { }
    public virtual void OnCancel() { }
}
