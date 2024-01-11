using System.Collections.Generic;
using UnityEngine;

public partial class Fireball_2 : Projectile, IAttackable
{
    public enum EState { NONE = -1, SHOT }

    #region Variables

    [SerializeField] private float range = 0f;
    [SerializeField] private float speed = 0f;
    private Vector3 moveDirection = Vector3.zero;

    #endregion Variables

    #region IAttackable Implementation

    public HitboxController AttackHitboxController { set; get; }

    public bool CalculateOnHit(List<IDamagable> targets)
    {
        foreach (IDamagable target in targets)
        {
            if (AttackHitboxController.CheckCollision(target.DamageHitboxController))
            {
                target.OnDamage();
                return true;
            }
        }

        return false;
    }

    #endregion IAttackabe Implementation

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        objectPoolIndex = EObjectPoolList.Fireball_2_FireHero;

        AttackHitboxController = GetComponent<HitboxController>();

        stateList.Add(new Shot(this));
    }

    #endregion Unity Events

    #region Methods

    #region Override 

    public override void Activate(DNFTransform subjectTransform, float sizeEff = 1f)
    {
        // Set projectile transform 
        dnfTransform.Position = subjectTransform.Position;
        dnfTransform.IsLeft = subjectTransform.IsLeft;
        dnfTransform.LocalScale = sizeEff;

        // Set projectile direction
        moveDirection = dnfTransform.IsLeft ? Vector3.left : Vector3.right;

        curState = stateList[(int)EState.SHOT];
        curState.OnStart();

        gameObject.SetActive(true);
    }

    #endregion Override

    #endregion Methods
}
