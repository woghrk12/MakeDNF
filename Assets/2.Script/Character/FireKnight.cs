
/// <summary>
/// The class of the controller for Fire Knight.
/// </summary>
public class FireKnight : Character
{
    protected override void Start()
    {
        base.Start();

        // Register the active skills
        RegisterSkill(EKeyName.BASEATTACK, FindObjectOfType<FireKnightSkill.BaseAttack>());
        RegisterSkill(EKeyName.SKILL1, FindObjectOfType<FireKnightSkill.SwiftDemonSlash>());
        RegisterSkill(EKeyName.SKILL2, FindObjectOfType<FireKnightSkill.Crescent>());
        RegisterSkill(EKeyName.SKILL3, FindObjectOfType<FireKnightSkill.Dodge>());
        RegisterSkill(EKeyName.SKILL4, FindObjectOfType<FireKnightSkill.BladeWaltz>());

        // Register the passive skills
        PassiveSkill magicSwordMedley = FindObjectOfType<FireKnightSkill.MagicSwordMedley>();
        magicSwordMedley.Init(this);
        magicSwordMedley.ApplySkillEffects();
    }
}
