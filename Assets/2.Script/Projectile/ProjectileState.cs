using UnityEngine;

public abstract class ProjectileState
{
    /// <summary>
    /// The phase of the projectile state.
    /// <para>
    /// PREDELAY : Delay before hitbox activation.
    /// HITBOXACTIVE : Phase during hitbox activation.
    /// STOPMOTION : Phase for stiffness effect if any object has been hit by the projectile.
    /// MOTIONINPROGRESS : Phase after hitbox deactivation.
    /// POSTDELAY : Delay after the projectile motion animation ends.
    /// </para>
    /// </summary>
    protected enum EStatePhase { NONE = -1, PREDELAY, HITBOXACTIVE, STOPMOTION, MOTIONINPROGRESS, POSTDELAY }

    #region Variables

    [Header("Variables to control the skill phase")]
    protected EStatePhase phase = EStatePhase.NONE;
    protected float preDelay = 0f;
    protected float duration = 0f;
    protected float postDelay = 0f;

    #endregion Variables

    #region Constructor

    public ProjectileState(Projectile stateController) { }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// The event method called when the projectile state is activated.
    /// It serves as an entry point for any logics that need to occur at the beginning.
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
    /// The event method called when the projectile state is completed.
    /// </summary>
    public virtual void OnComplete() { }

    #endregion Methods
}
