using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ScatterFlame_FireHero : Skill
{
    private enum EState { NONE = -1, SCATTER }

    #region Methods

    #region Override 

    public override void Init(Character character, Animator animator)
    {
        base.Init(character, animator);

        stateList.Add(new Scatter(this));
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return activeSkill == null;
    }

    public override IEnumerator Activate()
    {
        character.CanMove = false;
        character.CanJump = false;

        activeState = stateList[(int)EState.SCATTER];
        yield return activeState.Activate();

        Clear();
    }

    #endregion Override

    #endregion Methods
}
