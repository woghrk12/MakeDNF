using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    #region Variables

    protected List<Hitbox> targetList = new();

    #endregion Variables

    #region Methods

    public abstract void Shot(DNFTransform characterTransform, float sizeEff = 1f);
    public virtual void Cancel() { }
    public virtual void Clear() { }

    protected abstract IEnumerator Activate();

    #endregion Methods
}
