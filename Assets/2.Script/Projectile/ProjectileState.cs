using System.Collections;
using System.Collections.Generic;
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

    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnComplete() { }

    #endregion Methods
}