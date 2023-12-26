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

    public abstract void Init(Character character);
    public abstract bool CheckCanUseSkill(Skill activeSkill = null);
    public abstract IEnumerator Activate();

    public virtual void Cancel() { }

    public virtual void Clear() { }

    #region Events

    public virtual void OnPressed() { }
    public virtual void OnReleased() { }

    #endregion Events

    #endregion Methods
}
