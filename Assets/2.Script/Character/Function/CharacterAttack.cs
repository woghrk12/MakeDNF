using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    #region Variables

    private Dictionary<EKeyName, Skill> registeredSkillDictionary = new();

    #endregion Variables

    #region Methods

    public void RegisterSkill(EKeyName keyName, Skill skill)
    {
        switch (keyName)
        {
            case EKeyName.BASEATTACK:
            case EKeyName.SKILL1:
            case EKeyName.SKILL2:
            case EKeyName.SKILL3:
            case EKeyName.SKILL4:
                if (registeredSkillDictionary.ContainsKey(keyName))
                {
                    registeredSkillDictionary[keyName] = skill;
                }
                else
                {
                    registeredSkillDictionary.Add(keyName, skill);
                }
                break;

            default:
                throw new System.Exception($"Unable key name. Input key name : {keyName}.");
        }
    }

    public IEnumerator UseSkill(EKeyName keyName)
    {
        yield return registeredSkillDictionary[keyName].ActivateSkill();
    }

    #region Events

    public void OnSkillButtonPressed(EKeyName keyName)
    {
        if (!registeredSkillDictionary.ContainsKey(keyName)) return;
        
        registeredSkillDictionary[keyName].OnPressed();
    }

    public void OnSkillButtonReleased(EKeyName keyName)
    {
        if (!registeredSkillDictionary.ContainsKey(keyName)) return;

        registeredSkillDictionary[keyName].OnReleased();
    }

    #endregion Events

    #endregion Methods
}
