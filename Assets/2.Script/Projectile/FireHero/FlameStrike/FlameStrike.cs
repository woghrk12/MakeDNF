using System.Collections.Generic;

public partial class FlameStrike : Projectile, IAttackable
{
    public enum EState { NONE = -1, SHOT }

    #region Variables

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

    protected override void Awake()
    {
        base.Awake();

        objectPoolIndex = EObjectPoolList.Flame_Strike_FireHero;

        AttackHitboxController = GetComponent<HitboxController>();

        stateList.Add(new Shot(this));
    }

    #endregion Unity Events

    #region Methods

    #region Override

    public override void Activate(DNFTransform subjectTransform, float sizeEff = 1)
    {
        // Set projectile transform
        dnfTransform.Position = subjectTransform.Position;

        curState = stateList[(int)EState.SHOT];
        curState.OnStart();

        gameObject.SetActive(true);
    }

    #endregion Override

    #endregion Methods
}
