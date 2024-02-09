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

    /// <summary>
    /// Initailize the skill class.
    /// </summary>
    /// <param name="character">The character object that possess the skill</param>
    /// <param name="attackController">The controller object for handling the attack behaviour</param>
    public virtual void Init(Character character, AttackBehaviour attackController)
    {
        this.character = character;
        this.attackController = attackController;

        foreach (Skill skill in skillStat.CancelList)
        {
            CancelList.Add(skill.SkillCode);
        }
    }

    /// <summary>
    /// Check whether the character can use the skill.
    /// </summary>
    /// <param name="activeSkill">The skill currently being used (activated) by the character</param>
    /// <returns>true if the character can use the skill, otherwise false</returns>
    public virtual bool CheckCanUseSkill(Skill activeSkill)
    {
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
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

    /// <summary>
    /// The event method called when the skill is activated.
    /// It serves as an entry point for any logics that need to occur at the beginning.
    /// </summary>
    public virtual void OnStart() { }

    /// <summary>
    /// The event method called when the skill is completed.
    /// </summary>
    public virtual void OnComplete() { }

    /// <summary>
    /// The event method called when the skill is canceled by another skill.
    /// </summary>
    public virtual void OnCancel() 
    {
        curState.OnCancel();

        curState = null;
    }

    /// <summary>
    /// The event method that calls OnUpdate method of the current skill state every frame update.
    /// If the current skill state is null, do not call anything.
    /// </summary>
    public void OnUpdate() 
    {
        if (curState == null) return;

        curState.OnUpdate();
    }

    /// <summary>
    /// The event method that calls OnFixedUpdate method of the current skill state every fixed frame update.
    /// If the current skill state is null, do not call anything.
    /// </summary>
    public void OnFixedUpdate() 
    {
        if (curState == null) return;

        curState.OnFixedUpdate();
    }

    /// <summary>
    /// The event method that calls OnLateUpdate method of the current skill state after every update method has been executed.
    /// If the current skill state is null, do not call anything.
    /// </summary>
    public void OnLateUpdate() 
    {
        if (curState == null) return;

        curState.OnLateUpdate();
    }

    /// <summary>
    /// The event method that calls OnSkillButtonPressed method of the current skill state when the player press the button associated with the skill.
    /// If the current skill state is null, do not call anything.
    /// </summary>
    public void OnSkillButtonPressed() 
    {
        if (curState == null) return;

        curState.OnSkillButtonPressed();
    }

    /// <summary>
    /// The event method that calls OnSkillButtonReleased method of the current skill state when the player release the button associated with the skill.
    /// If the current skill state is null, do not call anything.
    /// </summary>
    public void OnSkillButtonReleased() 
    {
        if (curState == null) return;

        curState.OnSkillButtonReleased();
    }

    #endregion Events

    #endregion Methods
}
