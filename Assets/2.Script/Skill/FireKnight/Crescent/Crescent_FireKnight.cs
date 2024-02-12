using System.Collections.Generic;
using UnityEngine;

public partial class Crescent_FireKnight : ActiveSkill, IAttackable
{
    private enum EState { NONE = -1, SLASH }

    #region Variables

    /// <summary>
    /// The list of objects hit after the skill is activated.
    /// </summary>
    private List<IDamagable> alreadyHitObjects = new();

    #endregion Variables

    #region Properties 

    public override int SkillCode => typeof(Crescent_FireKnight).GetHashCode();

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
                target.OnDamage(character.DNFTransform, null, 3f, character.DNFTransform.IsLeft ? Vector3.left : Vector3.right);
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

    public override bool CheckCanUseSkill(Skill activeSkill)
    {
        if (!character.CanAttack) return false;
        
        if (character.IsJump) return false;
        
        if (character.CurBehaviourCode == BehaviourCodeList.HIT_BEHAVIOUR_CODE) return false;

        if (activeSkill != null && !CancelList.Contains(activeSkill.SkillCode)) return false;

        return true;
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
