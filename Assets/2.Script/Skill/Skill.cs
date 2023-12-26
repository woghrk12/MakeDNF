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

    protected Character character = null;

    protected Animator characterAnimator = null;

    [SerializeField] protected SkillStat skillStat;

    protected List<SkillState> stateList = new();
    protected SkillState activeState = null;

    #endregion Variables

    #region Properties

    public string Name => skillStat.Name;
    public int NeedMana => skillStat.NeedMana;
    public string SkillDescription => skillStat.SkillDescription;
    public List<Skill> CancelList => skillStat.cancelList;

    #endregion Properties

    #region Methods

    public virtual void Init(Character character)
    {
        this.character = character;
        characterAnimator = character.Animator;
    }

    public abstract bool CheckCanUseSkill(Skill activeSkill = null);
    public abstract IEnumerator Activate();

    public virtual void Cancel() 
    {
        if (activeState == null) return;

        activeState.Cancel();
        activeState = null;
    }

    public virtual void Clear() 
    {
        activeState.Clear();
        activeState = null;

        character.CanMove = true;
        character.CanJump = true;
    }

    #region Events

    public void OnPressed()
    {
        if (activeState == null) return;

        activeState.OnPressed();
    }
    public void OnReleased()
    {
        if (activeState == null) return;

        activeState.OnReleased();
    }

    #endregion Events

    #endregion Methods
}
