using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball_2 : Projectile, IAttackable
{
    #region Variables

    private DNFTransform dnfTransform = null;
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

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();
        
        AttackHitbox = GetComponent<Hitbox>();
    }

    #endregion Unity Events

    #region Override 

    public override void Shot(DNFTransform characterTransform, float sizeEff = 1f)
    {
        // Set projectile transform 
        dnfTransform.Position = characterTransform.Position;
        dnfTransform.IsLeft = characterTransform.IsLeft;
        dnfTransform.LocalScale = sizeEff;

        // Set projectile direction
        moveDirection = Time.fixedDeltaTime * speed * (dnfTransform.IsLeft ? Vector3.left : Vector3.right);

        gameObject.SetActive(true);

        StartCoroutine(Activate());
    }

    public override void Clear()
    {
        GameManager.ObjectPool.ReturnToPool(EObjectPoolList.Fireball_2_FireHero, gameObject);
        gameObject.SetActive(false);
    }

    protected override IEnumerator Activate()
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
