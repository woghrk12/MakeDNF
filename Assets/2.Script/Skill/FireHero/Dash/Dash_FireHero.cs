using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Dash_FireHero : Skill
{
    private enum EState { NONE = -1, DASH }

    #region Variables

    [SerializeField] private float dashRange = 0f;

    #endregion Variables

    #region Methods

    public override void Init(BehaviourController character, AttackBehaviour attackController)
    {
        base.Init(character, attackController);

        stateList.Add(new Dash(character, this));
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return activeSkill == null;
    }

    public override void OnStart()
    {
        character.CanMove = false;
        character.CanJump = false;

        curState = stateList[(int)EState.DASH];

        curState.OnStart();
    }

    public override void OnComplete()
    {
        curState = null;

        character.CanMove = true;
        character.CanJump = true;

        attackController.OnComplete();
    }

    #endregion Methods
}
