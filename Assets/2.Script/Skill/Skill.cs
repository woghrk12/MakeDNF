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

public abstract class Skill : MonoBehaviour
{
    #region Variables

    [SerializeField] protected SkillStat skillStat;

    protected Character character = null;
    protected AttackBehaviour attackController = null;

    protected int skillHash = 0;

    protected List<SkillState> stateList = new();
    protected SkillState curState = null;

    #endregion Variables

    #region Properties

    /// <summary>
    /// The hash code of the skill.
    /// </summary>
    public abstract int SkillCode { get; }

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

    /// <summary>
    /// The list of the skills that can be canceld while in use.
    /// </summary>
    public List<int> CancelList = new();

    #endregion Properties

    #region Methods

    public virtual void Init(Character character, AttackBehaviour attackController)
    {
        this.character = character;
        this.attackController = attackController;

        foreach (Skill skill in skillStat.CancelList)
        {
            CancelList.Add(skill.SkillCode);
        }
    }

    public virtual bool CheckCanUseSkill(Skill activeSkill = null)
    {
        return true;
    }

    public void SetState(int index)
    {
        if (index < 0 || index >= stateList.Count)
        {
            throw new Exception($"Out of range. GameObject : {gameObject.name}, Input index : {index}");
        }

        curState = stateList[index];
        curState.OnStart();
    }

    #region Events

    public virtual void OnStart() { }
    public virtual void OnComplete() { }
    public virtual void OnCancel() 
    {
        curState.OnCancel();

        curState = null;
    }

    public void OnUpdate() 
    {
        if (curState == null) return;

        curState.OnUpdate();
    }

    public void OnFixedUpdate() 
    {
        if (curState == null) return;

        curState.OnFixedUpdate();
    }

    public void OnLateUpdate() 
    {
        if (curState == null) return;

        curState.OnLateUpdate();
    }
    
    public void OnSkillButtonPressed() 
    {
        if (curState == null) return;

        curState.OnSkillButtonPressed();
    }

    public void OnSkillButtonReleased() 
    {
        if (curState == null) return;

        curState.OnSkillButtonReleased();
    }

    #endregion Events

    #endregion Methods
}
