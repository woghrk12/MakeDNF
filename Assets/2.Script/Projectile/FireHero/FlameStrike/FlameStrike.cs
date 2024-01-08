using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameStrike : Projectile, IAttackable
{
    #region Variables

    private DNFTransform dnfTransform = null;

    private List<IDamagable> alreadyHitObjects = new();

    #endregion Variables

    #region IAttackable Implementation

    public HitboxController AttackHitboxController { set; get; }

    public bool CalculateOnHit(List<IDamagable> targets)
    {
        int count = 0;

        foreach (IDamagable target in targets)
        {
            if (alreadyHitObjects.Contains(target)) continue;
            if (!AttackHitboxController.CheckCollision(target.DamageHitboxController)) continue;

            target.OnDamage();

            alreadyHitObjects.Add(target);
            count++;
        }

        return count > 0;
    }

    #endregion IAttackable Implementation

    #region Unity Events

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();

        AttackHitboxController = GetComponent<HitboxController>();
    }

    #endregion Unity Events

    #region Methods

    #region Override

    public override void Shot(DNFTransform subjectTransform, float sizeEff = 1)
    {
        // Set projectile transform
        dnfTransform.Position = subjectTransform.Position;

        gameObject.SetActive(true);

        StartCoroutine(Activate());
    }

    protected override IEnumerator Activate()
    {
        alreadyHitObjects.Clear();

        float timer = 0f;

        while (timer < 1f)
        {
            AttackHitboxController.CalculateHitbox();

            yield return Utilities.WaitForFixedUpdate;

            timer += Time.fixedDeltaTime;

            CalculateOnHit(GameManager.Room.Monsters);
        }

        Clear();
    }

    public override void Clear()
    {
        GameManager.ObjectPool.ReturnToPool(EObjectPoolList.Flame_Strike_FireHero, gameObject);
        gameObject.SetActive(false);
    }

    #endregion Override

    #endregion Methods
}
