using System.Collections;
using UnityEngine;

public partial class BaseAttack_FireHero : Skill
{
    private enum EState { NONE = -1, FIRST, SECOND, THIRD }

    #region Variables

    private Character character = null;

    private bool isContinue = false;

    #endregion Variables

    #region Methods

    #region Override

    public override void Init(Character character)
    {
        this.character = character;

        stateList.Add(new First(this, character));
        stateList.Add(new Second(this, character));
        stateList.Add(new Third(this, character));
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

        isContinue = false;
        activeState = stateList[(int)EState.SECOND];
        yield return activeState.Activate();

        if (!isContinue)
        {
            Clear();
            yield break;
        }

        isContinue = false;
        activeState = stateList[(int)EState.THIRD];
        yield return activeState.Activate();

        Clear();
    }

    public override void OnPressed()
    {
        if (activeState == null) return;

        activeState.OnPressed();
    }

    public override void Clear()
    {
        character.CanMove = true;
        character.CanJump = true;
    }

    #endregion Override

    #endregion Methods
}
