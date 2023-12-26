using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundExplosion : Projectile, IAttackable
{
    #region Variables

    private List<IDamagable> alreadyHitObjects = new();

    #endregion Variables

    #region IAttackable

    public Hitbox AttackHitbox { set; get; }

    public bool CalculateOnHit(List<IDamagable> targets)
    {
        int count = 0;

        foreach (IDamagable target in targets)
        {
            if (alreadyHitObjects.Contains(target)) continue;
            if (!AttackHitbox.CheckCollision(target.DamageHitbox)) continue;

            target.OnDamage();

            alreadyHitObjects.Add(target);
            count++;
        }

        return count > 0;
    }

    #endregion IAttackable

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        AttackHitbox = GetComponent<Hitbox>();
    }

    #endregion Unity Events

    #region Methods

    #region Override 

    public override void Shot(DNFTransform dnfTransform, float sizeEff = 1)
    {
        // Set projectile transform
        dnfTransform.Position = dnfTransform.Position;
        dnfTransform.IsLeft = dnfTransform.IsLeft;
        dnfTransform.LocalScale = sizeEff;

        gameObject.SetActive(true);

        StartCoroutine(Activate(dnfTransform, sizeEff));
    }

    protected override IEnumerator Activate(DNFTransform dnfTransform, float sizeEff = 1)
    {
        alreadyHitObjects.Clear();

        float timer = 0f;

        while (timer < 1f)
        {
            AttackHitbox.CalculateHitbox();

            yield return Utilities.WaitForFixedUpdate;

            timer += Time.fixedDeltaTime;

            CalculateOnHit(GameManager.Room.Monsters);
        }

        Clear();
    }

    public override void Clear()
    {
        GameManager.ObjectPool.ReturnToPool(gameObject);
        gameObject.SetActive(false);
    }

    #endregion Override

    #endregion Methods
}
