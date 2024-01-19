public partial class BaseAttack_FireHero : Skill
{
    private enum EState { NONE = -1, FIRST, SECOND, THIRD }

    #region Methods

    #region Override

    public override void Init(Character character, AttackBehaviour attackController)
    {
        base.Init(character, attackController);

        stateList.Add(new First(character, this));
        stateList.Add(new Second(character, this));
        stateList.Add(new Third(character, this));
    }

    public override bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return activeSkill == null;
    }

    public override void OnStart()
    {
        character.CanMove = false;
        character.CanJump = false;

        curState = stateList[(int)EState.FIRST];

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
