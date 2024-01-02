using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundExplosion : Projectile, IAttackable
{
    #region Variables

    private DNFTransform dnfTransform = null;

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

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();

        AttackHitbox = GetComponent<Hitbox>();
    }

    #endregion Unity Events

    #region Methods

    #region Override 

    public override void Shot(DNFTransform characterTransform, float sizeEff = 1f)
    {
        // Set projectile transform
        dnfTransform.Position = characterTransform.Position;
        dnfTransform.IsLeft = characterTransform.IsLeft;
        dnfTransform.LocalScale = sizeEff;

        gameObject.SetActive(true);

        StartCoroutine(Activate());
    }

    protected override IEnumerator Activate()
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
        GameManager.ObjectPool.ReturnToPool(EObjectPoolList.Ground_Explosion_FireHero, gameObject);
        gameObject.SetActive(false);
    }

    #endregion Override

    #endregion Methods
}
