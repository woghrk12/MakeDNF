using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Meteor : Projectile, IAttackable
{
    public enum EState { NONE = -1, SHOT, EXPLOSION }

    #region Variables

    [SerializeField] private float speed = 0f;
    private Vector3 moveDirection = Vector3.zero;

    private List<IDamagable> alreadyHitObjects = new();

    #endregion Variables

    #region Properties

    protected override EObjectPoolList ObjectPoolIndex => EObjectPoolList.Meteor_FireHero;

    #endregion Properties

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

    protected override void Awake()
    {
        base.Awake();

        AttackHitboxController = GetComponent<HitboxController>();

        stateList.Add(new Shot(this));
        stateList.Add(new Explosion(this));
    }

    #endregion Unity Events

    #region Methods

    #region Override    

    public override void Activate(DNFTransform subjectTransform, float sizeEff = 1f)
    {
        // Set projectile transform
        dnfTransform.Position = subjectTransform.Position + new Vector3(0f, 6f, 0f);
        dnfTransform.IsLeft = subjectTransform.IsLeft;
        dnfTransform.LocalScale = sizeEff;

        // Set projectile direction
        moveDirection = ((dnfTransform.IsLeft ? Vector3.left : Vector3.right) + Vector3.down).normalized;

        curState = stateList[(int)EState.SHOT];
        curState.OnStart();

        gameObject.SetActive(true);
    }

    #endregion Override

    #endregion Methods
}
