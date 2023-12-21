using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Projectile, IAttackable
{
    #region Variables

    private DNFRigidbody dnfRigidbody = null;

    [SerializeField] private float duration = 0f;
    [SerializeField] private float speed = 0f;
    private Vector3 moveDirection = Vector3.zero;

    #endregion Variables

    #region IAttackable

    public Hitbox AttackHitbox { set; get; }

    public bool CalculateOnHit(List<IDamagable> targets)
    {
        foreach (IDamagable target in targets)
        {
            if (AttackHitbox.CheckCollision(target.DamageHitbox))
            {
                target.OnDamage();
                return true;
            }
        }

        return false;
    }

    #endregion IAttackabe

    #region Methods

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        AttackHitbox = GetComponent<Hitbox>();
        dnfRigidbody = GetComponent<DNFRigidbody>();
    }

    #endregion Unity Events

    #region Override 

    public override void Shot(Vector3 startPos, bool isLeft, float sizeEff = 1)
    {
        // Set projectile transform
        dnfTransform.Position = startPos;
        dnfTransform.IsLeft = isLeft;
        dnfTransform.LocalScale = sizeEff;

        // Set projectile direction
        moveDirection = Time.fixedDeltaTime * speed * (isLeft ? Vector3.left : Vector3.right);

        gameObject.SetActive(true);

        StartCoroutine(Activate(startPos, isLeft, sizeEff));
    }

    public override void Clear()
    {
        GameManager.ObjectPool.ReturnToPool(gameObject);
        gameObject.SetActive(false);
    }

    protected override IEnumerator Activate(Vector3 startPos, bool isLeft, float sizeEff = 1f)
    {
        float timer = 0f;
        while (timer < duration)
        {
            dnfRigidbody.MoveDirection(moveDirection);
            
            AttackHitbox.CalculateHitbox();
            
            yield return Utilities.WaitForFixedUpdate;
            
            timer += Time.fixedDeltaTime;

            if (CalculateOnHit(GameManager.Room.Monsters))
            {
                // Spawn hit effect

                break;
            }
        }

        Clear();
    }

    #endregion Override

    #endregion Methods
}
