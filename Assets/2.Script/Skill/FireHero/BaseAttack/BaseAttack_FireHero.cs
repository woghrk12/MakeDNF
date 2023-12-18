using System.Collections;
using UnityEngine;

public partial class BaseAttack_FireHero : Skill
{
    private enum EState { NONE = -1, FIRST, SECOND, THIRD }

    #region Variables

    private bool isContinue = false;

    #endregion Variables

    #region Methods

    #region Override

    public override void InitSkill(Character character, Animator animator)
    {
        base.InitSkill(character, animator);

        stateList.Add(new First(this));
        stateList.Add(new Second(this));
        stateList.Add(new Third(this));
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return activeSkill == null;
    }

    public override IEnumerator Activate()
    {
        character.CanMove = false;
        character.CanJump = false;

        isContinue = false;

        activeState = stateList[(int)EState.FIRST];
        yield return activeState.Activate();

        if (!isContinue)
        {
            Clear();
            yield break;
        }

        activeState = stateList[(int)EState.SECOND];
        yield return activeState.Activate();

        if (!isContinue)
        {
            Clear();
            yield break;
        }

        activeState = stateList[(int)EState.THIRD];
        yield return activeState.Activate();

        Clear();
    }

    public override void Cancel()
    {
        if (activeState == null) return;

        activeState.Cancel();
        activeState = null;
    }

    public override void Clear()
    {
        activeState.Clear();
        activeState = null;

        character.CanMove = true;
        character.CanJump = true;
    }

    #endregion Override

    #endregion Methods
}
