public partial class ScatterFlame_FireHero : Skill
{
    private enum EState { NONE = -1, SCATTER }

    #region Methods

    #region Override 

    public override void Init(BehaviourController character, AttackBehaviour attackController)
    {
        base.Init(character, attackController);

        stateList.Add(new Scatter(character, this)); 
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return activeSkill == null || CancelList.Contains(activeSkill);
    }

    public override void OnStart()
    {
        character.CanMove = false;
        character.CanJump = false;

        curState = stateList[(int)EState.SCATTER];

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
