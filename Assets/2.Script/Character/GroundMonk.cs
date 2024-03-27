/// <summary>
/// The class of the controller for Ground Monk.
/// </summary>
public class GroundMonk : Character
{
    #region Unity Events

    protected override void Start()
    {
        base.Start();

        // Register the active skills
        RegisterSkill(EKeyName.BASEATTACK, FindObjectOfType<GroundMonkSkill.BaseAttack>());
        RegisterSkill(EKeyName.SKILL1, FindObjectOfType<GroundMonkSkill.GattlingPunch>());
        RegisterSkill(EKeyName.SKILL2, FindObjectOfType<GroundMonkSkill.Sway>());
        RegisterSkill(EKeyName.SKILL3, FindObjectOfType<GroundMonkSkill.StraightPunch>());
        RegisterSkill(EKeyName.SKILL4, FindObjectOfType<GroundMonkSkill.NuclearPunch>());

        // Register the passive skills
        // TODO : Register the passive skills of the Ground Monk
        // PassiveSkill magicSwordMedley = FindObjectOfType<FireKnightSkill.MagicSwordMedley>();
        // magicSwordMedley.Init(this);
        // magicSwordMedley.ApplySkillEffects();
    }

    #endregion Unity Events
}
