using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Meteor_FireHero : Skill
{
    private enum EState { NONE = -1, CHARGING, SHOT }

    #region Variables

    private Character character = null;

    private float sizeEff = 1f;

    #endregion Variables

    #region Methods

    #region Override

    public override void Init(Character character)
    {
        this.character = character;

        stateList.Add(new Charging(this, character));
        stateList.Add(new Shot(this, character));
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

    public override void OnReleased()
    {
        if (activeState == null) return;

        activeState.OnReleased();
    }

    public override void Clear()
    {
        character.CanMove = true;
        character.CanJump = true;
    }

    #endregion Override

    #endregion Methods
}
