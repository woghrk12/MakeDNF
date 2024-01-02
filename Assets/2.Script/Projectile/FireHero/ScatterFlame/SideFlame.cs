using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideFlame : Projectile, IAttackable
{
    #region Variables

    private DNFTransform dnfTransform = null;

    private List<IDamagable> alreadyHitObjects = new();

    private float preDelay = 0f;
    private float duration = 0f;
    private float postDelay = 0f;

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

        preDelay = Time.fixedDeltaTime * 1f * 4f;
        duration = Time.fixedDeltaTime * 12f * 4f;
        postDelay = Time.fixedDeltaTime * 2f * 4f;
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
        AttackHitbox.CalculateHitbox();

        yield return Utilities.WaitForSeconds(preDelay);

        float timer = 0f;
        while (timer < duration)
        {
            yield return Utilities.WaitForFixedUpdate;

            timer += Time.fixedDeltaTime;

            CalculateOnHit(GameManager.Room.Monsters);
        }

        yield return Utilities.WaitForSeconds(postDelay);

        Clear();
    }

    public override void Clear()
    {
        GameManager.ObjectPool.ReturnToPool(EObjectPoolList.Side_Flame_FireHero, gameObject);
        gameObject.SetActive(false);
    }

    #endregion Override

    #endregion Methods
}
