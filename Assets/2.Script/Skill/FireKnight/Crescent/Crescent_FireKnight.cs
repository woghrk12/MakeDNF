using System.Collections.Generic;
using UnityEngine;

public partial class Crescent_FireKnight : Skill, IAttackable
{
    private enum EState { NONE = -1, SLASH }

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

    #region Methods

    #region Override

    public override void Init(Character character, AttackBehaviour attackController)
    {
        base.Init(character, attackController);

        AttackHitboxController = GetComponent<HitboxController>();
        AttackHitboxController.Init(character.DNFTransform);

        skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.CRESCENT);

        stateList.Add(new Slash(character, this));
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return character.CanAttack && !character.IsJump && (activeSkill == null || CancelList.Contains(activeSkill));
    }

    public override void OnStart()
    {
        character.Animator.SetTrigger(skillHash);
        
        SetState((int)EState.SLASH);
    }

    public override void OnComplete()
    {
        curState = null;

        attackController.OnComplete();
    }

    #endregion Override

    #endregion Methods
}
