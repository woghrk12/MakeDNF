using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillStat
{
    public string Name = string.Empty;
    public int NeedMana = 0;
    public string SkillDescription = string.Empty;
    public List<Skill> cancelList = new();
}

public abstract class Skill : MonoBehaviour
{
    #region Variables

    [SerializeField] protected SkillStat skillStat;

    protected BehaviourController character = null;
    protected AttackBehaviour attackController = null;

    protected List<SkillState> stateList = new();
    protected SkillState curState = null;

    #endregion Variables

    #region Properties

    public string Name => skillStat.Name;
    public int NeedMana => skillStat.NeedMana;
    public string SkillDescription => skillStat.SkillDescription;
    public List<Skill> CancelList => skillStat.cancelList;

    #endregion Properties

    #region Methods

    public virtual void Init(BehaviourController character, AttackBehaviour attackController)
    {
        this.character = character;
        this.attackController = attackController;
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

        curState.OnCancel();
        curState = stateList[index];
        curState.OnStart();
    }

    #region Events

    public virtual void OnStart() { }
    public virtual void OnComplete() { }
    public virtual void OnCancel() { }

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
    
    public void OnPressed() 
    {
        if (curState == null) return;

        curState.OnPressed();
    }

    public void OnReleased() 
    {
        if (curState == null) return;

        curState.OnReleased();
    }

    #endregion Events

    #endregion Methods
}
