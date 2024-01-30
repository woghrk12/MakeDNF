using UnityEngine;

public partial class SwiftDemonSlash_FireKnight : Skill
{
    private enum EState { NONE = -1, COMBO, FINISH }

    #region Variables

    private int numCombo = 12;

    #endregion Variables

    #region Methods

    #region Override

    public override void Init(Character character, AttackBehaviour attackController)
    {
        base.Init(character, attackController);

        skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.SLASH_COMBO);

        stateList.Add(new Combo(character, this));
        stateList.Add(new Finish(character, this));
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return character.CanAttack && !character.IsJump && (activeSkill == null || CancelList.Contains(activeSkill));
    }

    public override void OnStart()
    {
        character.Animator.SetTrigger(skillHash);

        curState = stateList[(int)EState.COMBO];
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
