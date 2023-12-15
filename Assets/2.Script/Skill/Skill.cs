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

    protected Animator animator = null;

    [SerializeField] protected SkillStat skillStat;

    #endregion Variables

    #region Properties

    public string Name => skillStat.Name;
    public int NeedMana => skillStat.NeedMana;
    public string SkillDescription => skillStat.SkillDescription;
    public List<Skill> CancelList => skillStat.cancelList;

    #endregion Properties

    #region Methods

    public virtual void InitSkill(Character character, Animator animator)
    {
        this.character = character;
        this.animator = animator;
    }

    public virtual void OnPressed() { }
    public virtual void OnReleased() { }

    public abstract bool CheckCanUseSkill(Skill activeSkill = null);

    public abstract IEnumerator ActivateSkill();

    public virtual void Clear() { }

    #endregion Methods
}
