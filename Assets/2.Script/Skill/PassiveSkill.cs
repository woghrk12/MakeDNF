public abstract class PassiveSkill : Skill
{
    #region Methods

    /// <summary>
    /// Initailize the PassiveSkill class.
    /// </summary>
    /// <param name="character">The character object that possess the skill</param>
    public virtual void Init(Character character)
    {
        this.character = character;
    }

    /// <summary>
    /// Apply the effects of acquiring a passive skill to the character.
    /// </summary>
    public abstract void ApplySkillEffects();

    /// <summary>
    /// Remove the effects of a passive skill from the character.
    /// </summary>
    public abstract void RemoveSkillEffects();

    #endregion Methods
}
