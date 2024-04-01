using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveSkill : Skill
{
    #region Variables

    protected AttackBehaviour attackController = null;

    /// <summary>
    /// The key player need to input for additional skill manipulation.
    /// </summary>
    protected EKeyName keyName = EKeyName.NONE;

    /// <summary>
    /// The animator key for character skill usage motion.
    /// </summary>
    protected int skillHash = 0;

    /// <summary>
    /// The list containing various states of the skill.
    /// </summary>
    protected List<SkillState> stateList = new();

    /// <summary>
    /// The current skill state.
    /// </summary>
    protected SkillState curState = null;

    /// <summary>
    /// The list of the skills that can be canceld while in use.
    /// </summary>
    [HideInInspector] public List<int> CancelList = new();

    #endregion Variables

    #region Methods

    #region Virtual

    /// <summary>
    /// Initailize the ActiveSkill class.
    /// </summary>
    /// <param name="character">The character object that possess the skill</param>
    /// <param name="attackController">The controller object for handling the attack behaviour</param>
    /// <param name="keyName">The key player need to input for additional skill manipulation</param>
    public virtual void Init(Character character, AttackBehaviour attackController, EKeyName keyName)
    {
        this.character = character;
        this.attackController = attackController;
        this.keyName = keyName;

        foreach (Skill skill in skillStat.CancelList)
        {
            CancelList.Add(skill.SkillCode);
        }
    }

    /// <summary>
    /// Check whether the character can use the skill.
    /// </summary>
    /// <param name="curSkill">The skill currently being used (activated) by the character</param>
    /// <returns>true if the character can use the skill, otherwise false</returns>
    public virtual bool CheckCanUseSkill(ActiveSkill curSkill)
    {
        return true;
    }

    /// <summary>
    /// Sets the current state of the skill to the one corresponding to the given index among multiple states in the skill.
    /// </summary>
    /// <param name="index">The index of the state to set as the current state</param>
    public void SetState(int index)
    {
        if (index < 0 || index >= stateList.Count)
        {
            throw new Exception($"Out of range. GameObject : {gameObject.name}, Input index : {index}");
        }

        curState = stateList[index];
        curState.OnStart();
    }

    /// <summary>
    /// The event method called when the skill is activated.
    /// It serves as an entry point for any logics that need to occur at the beginning.
    /// </summary>
    public virtual void OnStart() { }

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

    #endregion Virtual

    #endregion Methods
}
