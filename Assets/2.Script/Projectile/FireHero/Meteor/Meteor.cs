using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Projectile, IAttackable
{
    #region Variables

    private DNFTransform dnfTransform = null;
    private DNFRigidbody dnfRigidbody = null;

    [SerializeField] private float speed = 0f;
    private Vector3 moveDirection = Vector3.zero;
    private float sizeEff = 1f;

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
        dnfRigidbody = GetComponent<DNFRigidbody>();

        AttackHitbox = GetComponent<Hitbox>();
    }

    #endregion Unity Events

    #region Methods

    #region Override    

    public override void Shot(DNFTransform characterTransform, float sizeEff = 1f)
    {
        this.sizeEff = sizeEff;

        // Set projectile transform
        dnfTransform.Position = characterTransform.Position + new Vector3(0f, 6f, 0f);
        dnfTransform.IsLeft = characterTransform.IsLeft;
        dnfTransform.LocalScale = sizeEff;

        // Set projectile direction
        moveDirection = Time.fixedDeltaTime * speed * ((dnfTransform.IsLeft ? Vector3.left : Vector3.right) + Vector3.down).normalized;

        gameObject.SetActive(true);

        StartCoroutine(Activate());
    }

    protected override IEnumerator Activate()
    {
        alreadyHitObjects.Clear();

        while (!dnfRigidbody.IsGround)
        {
            dnfRigidbody.MoveDirection(moveDirection);

            AttackHitbox.CalculateHitbox();

            yield return Utilities.WaitForFixedUpdate;

            CalculateOnHit(GameManager.Room.Monsters);
        }

        GameManager.ObjectPool.SpawnFromPool("Ground_Explosion_FireHero").GetComponent<Projectile>().Shot(dnfTransform, sizeEff);

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
