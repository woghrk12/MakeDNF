using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    #region Variables

    protected DNFTransform dnfTransform = null;
    protected Hitbox hitbox = null;
    protected ParticleSystem projectileEffect = null;

    #endregion Variables

    #region Methods

    public abstract IEnumerator Shot(Vector3 position, bool isLeft, float sizeEff = 1f);

    public abstract void Destroy();

    #endregion Methods
}
