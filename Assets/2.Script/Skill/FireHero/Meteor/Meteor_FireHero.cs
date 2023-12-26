using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Meteor_FireHero : Skill
{
    private enum EState { NONE = -1, CHARGING, SHOT }

    #region Variables

    private float sizeEff = 1f;

    #endregion Variables

    #region Methods

    #region Override

    public override void Init(Character character)
    {
        base.Init(character);

        stateList.Add(new Charging(this));
        stateList.Add(new Shot(this));
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return activeSkill == null;
    }

    public override IEnumerator Activate()
    {
        character.CanMove = false;
        character.CanJump = false;

        sizeEff = 1f;

        activeState = stateList[(int)EState.CHARGING];
        yield return activeState.Activate();

        activeState = stateList[(int)EState.SHOT];
        yield return activeState.Activate();

        Clear();
    }

    #endregion Override

    #endregion Methods
}
