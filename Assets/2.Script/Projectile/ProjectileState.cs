using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileState
{
    /// <summary>
    /// The phase of the skill state.
    /// <para>
    /// PREDELAY : Delay before hitbox activation or projectile creation.
    /// HITBOXACTIVE : Phase during hitbox activation.
    /// STOPMOTION : Phase for stiffness effect if any object has been hit by the skill.
    /// MOTIONINPROGRESS : Phase after hitbox deactivation or projectile creation.
    /// POSTDELAY : Delay after the skill motion animation ends.
    /// </para>
    /// </summary>
    protected enum EStatePhase { NONE = -1, PREDELAY, HITBOXACTIVE, STOPMOTION, MOTIONINPROGRESS, POSTDELAY }

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