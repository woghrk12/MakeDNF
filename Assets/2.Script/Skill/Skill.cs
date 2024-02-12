using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class contains information about a skill, such as its name and mana cost.
/// </summary>
[Serializable]
public class SkillStat
{
    /// <summary>
    /// The name of the skill.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// The amount of mana required to use the skill.
    /// </summary>
    public int NeedMana = 0;

    /// <summary>
    /// The description of the skill.
    /// </summary>
    public string SkillDescription = string.Empty;

    /// <summary>
    /// The list of the skills that can be canceld while in use.
    /// </summary>
    public List<Skill> CancelList = new();
}

/// <summary>
/// Enumeration representing different types of skills.
/// </summary>
public enum ESkillType 
{
    /// <summary>
    /// Indicates that the skill type is not specified or valid.
    /// </summary>
    NONE = -1,

    /// <summary>
    /// Represents an active skill that requires user activation or input to be utilized.
    /// </summary>
    ACTIVE,

    /// <summary>
    /// Represents a passive skill that provides ongoing benefits without explicit activation.
    /// </summary>
    PASSIVE
}

public abstract class Skill : MonoBehaviour
{
    #region Variables

    [SerializeField] protected SkillStat skillStat;

    protected Character character = null;

    #endregion Variables

    #region Properties

    /// <summary>
    /// The hash code of the skill.
    /// </summary>
    public abstract int SkillCode { get; }

    /// <summary>
    /// The type of the skill.
    /// </summary>
    public abstract ESkillType SkillType { get; }

    /// <summary>
    /// The name of the skill.
    /// </summary>
    public string Name => skillStat.Name;

    /// <summary>
    /// The amount of mana required to use the skill.
    /// </summary>
    public int NeedMana => skillStat.NeedMana;

    /// <summary>
    /// The description of the skill.
    /// </summary>
    public string SkillDescription => skillStat.SkillDescription;

    #endregion Properties
}
