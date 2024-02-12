using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveSkill : Skill
{
    #region Variables

    #endregion Variables

    #region Properties

    public override ESkillType SkillType => ESkillType.PASSIVE;

    #endregion Properties

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

    #endregion Methods
}
