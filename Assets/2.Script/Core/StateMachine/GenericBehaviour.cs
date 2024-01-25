using UnityEngine;

public abstract class GenericBehaviour<T> : MonoBehaviour where T : BehaviourController
{
    #region Variables

    protected T controller = null;

    protected int behaviourCode = 0;

    #endregion Variables

    #region Properties

    /// <summary>
    /// Hash code value for the behaviour.
    /// </summary>
    public int BehaviourCode => behaviourCode;

    #endregion Properties

    #region Unity Events

    protected virtual void Awake()
    {
        behaviourCode = GetType().GetHashCode();
    }

    #endregion Unity Events

    #region Methods

    #region Virtual

    /// <summary>
    /// The event method called when the behaviour is started.
    /// It serves as an entry point for any behaviours that need to occur at the beginning.
    /// </summary>
    public virtual void OnStart() { }

    /// <summary>
    /// The event method called every frame update.
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// The event method called every fixed frame update.
    /// </summary>
    public virtual void OnFixedUpdate() { }

    /// <summary>
    /// The event method called after every update method has been executed.
    /// </summary>
    public virtual void OnLateUpdate() { }

    /// <summary>
    /// The event method called when the behaviour is completed.
    /// </summary>
    public virtual void OnComplete() { }

    /// <summary>
    /// The event method called when the behaviour is canceled by another behaviour.
    /// </summary>
    public virtual void OnCancel() { }

    #endregion Virtual

    #endregion Methods
}
