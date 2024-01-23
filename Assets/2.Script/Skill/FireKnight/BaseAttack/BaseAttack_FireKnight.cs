using System.Collections.Generic;
using UnityEngine;

public partial class BaseAttack_FireKnight : Skill, IAttackable
{
    private enum EState { NONE = -1, FIRST, SECOND, THIRD }

    #region Variables

    private List<IDamagable> alreadyHitObjects = new();
    [SerializeField] private UnityEngine.UI.Text text;
    #endregion Variables

    #region IAttackable Implementation

    public HitboxController AttackHitboxController { set; get; }

    public bool CalculateOnHit(List<IDamagable> targets)
    {
        return true;
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
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return activeSkill == null; 
    }

    public override void OnStart()
    {
        character.CanMove = false;
        character.CanJump = false;

        character.Animator.SetTrigger(skillHash);

        curState = stateList[(int)EState.FIRST];
        curState.OnStart();
    }

    public override void OnComplete()
    {
        curState = null;

        character.CanMove = true;
        character.CanJump = true;

        attackController.OnComplete();
    }

    #endregion Override

    #endregion Methods
}
