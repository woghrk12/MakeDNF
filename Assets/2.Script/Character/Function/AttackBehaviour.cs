using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : GenericBehaviour
{
    #region Variables

    private Dictionary<EKeyName, Skill> registeredSkillDictionary = new();

    private int isAttackHash = 0;
    private int endAttackHash = 0;

    private Skill curSkill = null;

    #endregion Variables

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        isAttackHash = Animator.StringToHash(AnimatorKey.Character.IS_ATTACK);
        endAttackHash = Animator.StringToHash(AnimatorKey.Character.END_ATTACK);
    }

    #endregion Unity Events

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

                skill.Init(controller, this);
                break;

            default:
                throw new System.Exception($"Unable key name. Input key name : {keyName}.");
        }
    }

    public bool CheckCanAttack(EKeyName keyName)
    {
        return registeredSkillDictionary[keyName].CheckCanUseSkill(curSkill);
    }

    public void Attack(EKeyName keyName)
    {
        curSkill?.OnCancel();

        curSkill = registeredSkillDictionary[keyName];

        controller.SetBehaviour(behaviourCode);
    }

    #region Override

    public override void OnStart()
    {
        controller.Animator.SetBool(isAttackHash, true);

        curSkill.OnStart();
    }

    public override void OnUpdate()
    {
        curSkill.OnUpdate();
    }

    public override void OnFixedUpdate()
    {
        curSkill.OnFixedUpdate();
    }

    public override void OnLateUpdate()
    {
        curSkill.OnLateUpdate();
    }

    public override void OnComplete()
    {
        controller.Animator.SetBool(isAttackHash, false);
        controller.Animator.SetTrigger(endAttackHash);

        curSkill = null;

        controller.SetBehaviour(BehaviourCodeList.idleBehaviourCode);
    }

    public override void OnCancel()
    {
        controller.Animator.SetBool(isAttackHash, false);

        curSkill = null;
    }

    #endregion Override

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
