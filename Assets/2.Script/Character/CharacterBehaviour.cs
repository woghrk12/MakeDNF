using UnityEngine;

[RequireComponent(typeof(Character))]
public abstract class CharacterBehaviour : MonoBehaviour
{
    #region Variables

    protected Character character = null;

    #endregion Variables

    #region Properties

    /// <summary>
    /// Hash code value for the behaviour.
    /// </summary>
    public abstract int BehaviourCode { get; }

    #endregion Properties

    #region Unity Events

    protected virtual void Awake()
    {
        character = GetComponent<Character>();
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

    /// <summary>
    /// The event method called when the player control the joystick.
    /// </summary>
    /// <param name="direction">The direction vector in the DNF coordinate received through the joystick</param>
    public virtual void OnJoystickMoved(Vector3 direction) { }

    /// <summary>
    /// The event method called when the player press the button.
    /// </summary>
    /// <param name="keyName">The enum element indicating the button which player press</param>
    public virtual void OnButtonPressed(EKeyName keyName) { }

    /// <summary>
    /// The event method called when the player release the button.
    /// </summary>
    /// <param name="keyName">The enum element indicating the button which player release</param>
    public virtual void OnButtonReleased(EKeyName keyName) { }

    #endregion Virtual

    #endregion Methods
}
