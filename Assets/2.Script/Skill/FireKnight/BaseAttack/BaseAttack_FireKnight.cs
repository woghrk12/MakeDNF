using System.Collections.Generic;
using UnityEngine;

public partial class BaseAttack_FireKnight : Skill, IAttackable
{
    private enum EState { NONE = -1, FIRST, SECOND, THIRD, JUMP }

    #region Variables

    private List<IDamagable> alreadyHitObjects = new();

    #endregion Variables

    #region Properties

    public override int SkillCode => typeof(BaseAttack_FireKnight).GetHashCode();

    #endregion Properties

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

        skillHash = Animator.StringToHash(AnimatorKey.Character.BASE_ATTACK);
        
        stateList.Add(new First(character, this));
        stateList.Add(new Second(character, this));
        stateList.Add(new Third(character, this));
        stateList.Add(new Jump(character, this));
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return character.IsJump ? !character.DNFRigidbody.IsGround && activeSkill == null : activeSkill == null;
    }

    public override void OnStart()
    {
        character.Animator.SetTrigger(skillHash);

        curState = stateList[character.IsJump ? (int)EState.JUMP : (int)EState.FIRST];
        curState.OnStart();
    }

    public override void OnComplete()
    {
        curState = null;

        attackController.OnComplete();
    }

    #endregion Override

    #endregion Methods
}
