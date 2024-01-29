using System.Collections.Generic;

public partial class Slash_2_FireKnight : Projectile, IAttackable
{
    public enum EState { NONE = -1, SHOT }

    #region Variables

    private List<IDamagable> alreadyHitObjects = new();

    #endregion Variables

    #region Properties

    protected override EObjectPoolList ObjectPoolIndex => EObjectPoolList.Slash_2_FireKnight;

    #endregion Properties

    #region IAttackable Implementation

    public HitboxController AttackHitboxController { set; get;}

    public bool CalculateOnHit(List<IDamagable> targets)
    {
        int count = 0;

        foreach (IDamagable target in targets)
        {
            if (alreadyHitObjects.Contains(target)) continue;
            if (AttackHitboxController.CheckCollision(target.DamageHitboxController))
            {
                target.OnDamage();
                alreadyHitObjects.Add(target);
                count++;
            }
        }

        return count > 0;
    }

    #endregion IAttackable Implementation

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        AttackHitboxController = GetComponent<HitboxController>();
        AttackHitboxController.Init(dnfTransform);

        stateList.Add(new Shot(this));
    }

    #endregion Unity Events

    #region Methods

    #region Override

    public override void Activate(DNFTransform subjectTransform, float sizeEff = 1)
    {
        // Set projectile transform
        dnfTransform.Position = subjectTransform.Position;
        dnfTransform.IsLeft = subjectTransform.IsLeft;
        dnfTransform.LocalScale = sizeEff;

        AttackHitboxController.CalculateHitbox();

        curState = stateList[(int)EState.SHOT];
        curState.OnStart();
    }

    #endregion Override

    #endregion Methods
}
