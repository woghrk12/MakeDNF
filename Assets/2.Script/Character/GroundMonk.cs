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
        // TODO : Register the active skills of the Ground Monk
        //RegisterSkill(EKeyName.BASEATTACK, FindObjectOfType<FireKnightSkill.BaseAttack>());

        // Register the passive skills
        // TODO : Register the passive skills of the Ground Monk
        // PassiveSkill magicSwordMedley = FindObjectOfType<FireKnightSkill.MagicSwordMedley>();
        // magicSwordMedley.Init(this);
        // magicSwordMedley.ApplySkillEffects();
    }

    #endregion Unity Events
}