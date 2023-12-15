using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct SkillStat
{
    public string Name;
    public int NeedMana;
    public string SkillDescription;
}

public abstract class Skill : MonoBehaviour
{
    #region Variables

    [SerializeField] protected SkillStat skillStat;

    #endregion Variables

    #region Properties

    public string Name => skillStat.Name;
    public int NeedMana => skillStat.NeedMana;
    public string SkillDescription => skillStat.SkillDescription;

    #endregion Properties

    #region Methods

    public virtual void OnPressed() { }
    public virtual void OnReleased() { }

    public virtual bool CheckCanUseSkill() { return true; }

    public abstract IEnumerator ActivateSkill();

    public abstract void Clear();

    #endregion Methods
}
