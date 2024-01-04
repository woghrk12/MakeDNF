using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// List of hitboxes representing potential targets for the projectile.
    /// </summary>
    protected List<Hitbox> targetList = new();

    #endregion Variables

    #region Methods

    #region Abstract

    /// <summary>
    /// Activates the projectile, initiating its travel and behaviour.
    /// </summary>
    /// <param name="subjectTransform">The DNF transform of the object that created the projectile</param>
    /// <param name="sizeEff">The size ratio of the projectile</param>
    public abstract void Shot(DNFTransform subjectTransform, float sizeEff = 1f);

    /// <summary>
    /// Activates the projectile, enabling it to interact with the environment and targets.
    /// </summary>
    protected abstract IEnumerator Activate();

    #endregion Abstract

    #region Virtual

    /// <summary>
    /// Cancel the activation of the projectile, interrupting its current behaviour.
    /// </summary>
    public virtual void Cancel() { }

    /// <summary>
    /// Clears the projectile, marking it as completed and preparing for potential recycling.
    /// </summary>
    public virtual void Clear() { }

    #endregion Virtual

    #endregion Methods
}
