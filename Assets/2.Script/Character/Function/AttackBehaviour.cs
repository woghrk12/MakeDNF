using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    #region Variables

    private Character character = null;

    private Animator characterAnimator = null;

    private int isAttackHash = 0;
    private int endAttackHash = 0;

    private Dictionary<EKeyName, Skill> registeredSkillDictionary = new();

    private Coroutine attackCo = null;
    private Skill activeSkill = null;

    #endregion Variables

    #region Methods

    public void Init(Character character)
    {
        this.character = character;
        characterAnimator = character.Animator;

        isAttackHash = Animator.StringToHash(AnimatorKey.Character.IS_ATTACK);
        endAttackHash = Animator.StringToHash(AnimatorKey.Character.END_ATTACK);
    }

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

                skill.Init(character);
                break;

            default:
                throw new System.Exception($"Unable key name. Input key name : {keyName}.");
        }
    }

    public bool CheckCanAttack(EKeyName keyName)
    {
        return registeredSkillDictionary[keyName].CheckCanUseSkill(activeSkill);
    }

    public void Attack(EKeyName keyName)
    {
        activeSkill = registeredSkillDictionary[keyName];
        attackCo = StartCoroutine(UseSkill(activeSkill));
    }

    private IEnumerator UseSkill(Skill skill)
    {
        characterAnimator.SetBool(isAttackHash, true);

        yield return skill.Activate();

        characterAnimator.SetBool(isAttackHash, false);
        characterAnimator.SetTrigger(endAttackHash);

        attackCo = null;
        activeSkill = null;
    }

    #region Events

    public void OnUpdate() { }

    public void OnFixedUpdate() { }
    public void OnLateUpdate() { }


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
