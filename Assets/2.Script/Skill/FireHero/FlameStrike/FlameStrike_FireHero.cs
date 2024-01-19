using UnityEngine;

public partial class FlameStrike_FireHero : Skill
{
    private enum EState { NONE = -1, AIM, SHOT }

    #region Variables

    [SerializeField] private float aimSpeed = 0f;
    [SerializeField] private DNFTransform aimTransform = null;

    #endregion Variables

    #region Methods

    #region Override 

    public override void Init(Character character, AttackBehaviour attackController)
    {
        base.Init(character, attackController);

        stateList.Add(new Aim(character, this));
        stateList.Add(new Shot(character, this));

        aimTransform.gameObject.SetActive(false);
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return activeSkill == null || CancelList.Contains(activeSkill);
    }

    public override void OnStart()
    {
        character.CanMove = false;
        character.CanJump = false;

        curState = stateList[(int)EState.AIM];

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
