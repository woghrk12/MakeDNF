using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    #region Variables

    protected DNFTransform dnfTransform = null;
    protected Hitbox hitbox = null;
    
    protected List<Hitbox> targetList = new();

    #endregion Variables

    #region Unity Events

    protected virtual void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();
        hitbox = GetComponent<Hitbox>();
    }

    private void LateUpdate()
    {
        CalculateOnHit(targetList);
    }

    #endregion Unity Events

    #region Methods

    public abstract void Shot(Vector3 startPos, bool isLeft, float sizeEff = 1f);
    public virtual void Cancel() { }
    public virtual void Clear() { }

    protected abstract IEnumerator Activate(Vector3 startPos, bool isLeft, float sizeEff = 1f);
    protected abstract void CalculateOnHit(List<Hitbox> targets);

    #endregion Methods
}
