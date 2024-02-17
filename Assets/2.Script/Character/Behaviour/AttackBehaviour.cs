using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The generic behaviour class when the character performs a base attack or uses a skill.
/// </summary>
public class AttackBehaviour : GenericBehaviour<Character>
{
    #region Variables

    /// <summary>
    /// The dictionary for using the skills registered to key slot.
    /// Key : the name of key the user will input.
    /// Value : the skill registered to the key slot.
    /// </summary>
    private Dictionary<EKeyName, ActiveSkill> registeredSkillDictionary = new();

    [Header("Animation key hash")]
    private int isAttackHash = 0;
    private int endAttackHash = 0;

    /// <summary>
    /// The skill currently being used (activated) by the character.
    /// </summary>
    private ActiveSkill curSkill = null;

    #endregion Variables

    #region Properties

    /// <summary>
    /// A flag variable indicating whether the controller is allowed to attack.
    /// </summary>
    public bool CanAttack { set; get; }

    #endregion Properties

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        controller = GetComponent<Character>();

        isAttackHash = Animator.StringToHash(AnimatorKey.Character.IS_ATTACK);
        endAttackHash = Animator.StringToHash(AnimatorKey.Character.END_ATTACK);
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Register the skill to a given key, allowing the skill to be used when the key is pressed.
    /// The skill can only be registered to the skill key.
    /// An error is triggered if any other key is provided such as jump key.
    /// </summary>
    /// <param name="keyName">The key to register the skill</param>
    /// <param name="skill">The skill to register to the key</param>
    public void RegisterSkill(EKeyName keyName, ActiveSkill skill)
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

    /// <summary>
    /// Return whether the skill registered to the given key can be used.
    /// </summary>
    /// <param name="keyName">The skill key to check</param>
    /// <returns>True if the skill registered to the key can be used</returns>
    public bool CheckCanAttack(EKeyName keyName)
    {
        return registeredSkillDictionary[keyName].CheckCanUseSkill(curSkill);
    }

    /// <summary>
    /// Use the skill registered to the given key to initiate an attack.
    /// If there is a currently active skill, cancel the skill.
    /// </summary>
    /// <param name="keyName">The skill key to attack</param>
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

        controller.SetBehaviour(BehaviourCodeList.IDLE_BEHAVIOUR_CODE);
    }

    public override void OnCancel()
    {
        controller.Animator.SetBool(isAttackHash, false);
        controller.Animator.SetTrigger(endAttackHash);

        if (curSkill != null)
        {
            curSkill.OnCancel();
            curSkill = null;
        }
    }

    #endregion Override

    #region Events

    /// <summary>
    /// The event method called when the player press the skill button.
    /// </summary>
    /// <param name="keyName">The name of the button which the player press</param>
    public void OnSkillButtonPressed(EKeyName keyName)
    {
        if (!registeredSkillDictionary.ContainsKey(keyName)) return;
        
        registeredSkillDictionary[keyName].OnSkillButtonPressed();
    }

    /// <summary>
    /// The event method called when the player release the skill button.
    /// </summary>
    /// <param name="keyName">The name of the button which the player release</param>
    public void OnSkillButtonReleased(EKeyName keyName)
    {
        if (!registeredSkillDictionary.ContainsKey(keyName)) return;

        registeredSkillDictionary[keyName].OnSkillButtonReleased();
    }

    #endregion Events

    #endregion Methods
}
