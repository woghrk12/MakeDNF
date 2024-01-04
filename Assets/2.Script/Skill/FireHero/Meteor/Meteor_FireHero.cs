public partial class Meteor_FireHero : Skill
{
    private enum EState { NONE = -1, CHARGING, SHOT }

    #region Variables

    private float sizeEff = 1f;

    #endregion Variables

    #region Methods

    #region Override

    public override void Init(BehaviourController character, AttackBehaviour attackController)
    {
        base.Init(character, attackController);

        stateList.Add(new Charging(character, this));
        stateList.Add(new Shot(character, this));
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return activeSkill == null || CancelList.Contains(activeSkill);
    }

    public override void OnStart()
    {
        character.CanMove = false;
        character.CanJump = false;

        sizeEff = 1f;

        curState = stateList[(int)EState.CHARGING];

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
