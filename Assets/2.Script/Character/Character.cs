using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Variables

    private Animator animator = null;

    private DNFTransform dnfTransform = null;
    private DNFRigidbody dnfRigidbody = null;

    [Header("Character components")]
    private CharacterMove moveController = null;
    private CharacterAttack attackController = null;

    public bool CanMove = true;
    public bool CanJump = true;
    public bool CanAttack = true;

    #endregion Variables

    #region Properties

    public Animator Animator => animator;

    public DNFTransform DNFTransform => dnfTransform;
    public DNFRigidbody DNFRigidbody => dnfRigidbody;

    #endregion Properties

    #region Unity Events

    private void Awake()
    {
        animator = GetComponent<Animator>();

        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();

        moveController = GetComponent<CharacterMove>();
        attackController = GetComponent<CharacterAttack>();

        moveController.Init(this);
        attackController.Init(this);
    }

    private void Start()
    {
        // Debug
        Camera.main.GetComponent<CameraFollow>().SetTarget(transform);
        attackController.RegisterSkill(EKeyName.BASEATTACK, FindObjectOfType<BaseAttack_FireHero>());
        attackController.RegisterSkill(EKeyName.SKILL1, FindObjectOfType<Meteor_FireHero>());
        attackController.RegisterSkill(EKeyName.SKILL2, FindObjectOfType<ScatterFlame_FireHero>());

        GameManager.Input.SetMovementDelegate(OnJoystickMoved);

        GameManager.Input.SetButtonDelegate(EKeyName.JUMP, OnJumpButtonPressed);

        GameManager.Input.SetButtonDelegate(EKeyName.BASEATTACK, OnAttackButtonPressed, OnAttackButtonReleased);

        GameManager.Input.SetButtonDelegate(EKeyName.SKILL1,
            () => OnSkillButtonPressed(EKeyName.SKILL1),
            () => OnSkillButtonReleased(EKeyName.SKILL1));
        GameManager.Input.SetButtonDelegate(EKeyName.SKILL2,
            () => OnSkillButtonPressed(EKeyName.SKILL2),
            () => OnSkillButtonReleased(EKeyName.SKILL2));
        GameManager.Input.SetButtonDelegate(EKeyName.SKILL3,
            () => OnSkillButtonPressed(EKeyName.SKILL3),
            () => OnSkillButtonReleased(EKeyName.SKILL3));
        GameManager.Input.SetButtonDelegate(EKeyName.SKILL4,
            () => OnSkillButtonPressed(EKeyName.SKILL4),
            () => OnSkillButtonReleased(EKeyName.SKILL4));
    }

    #endregion Unity Events

    #region Event Methods

    public void OnJoystickMoved(Vector3 direction)
    {
        if (!CanMove) return;

        moveController.Move(direction);
    }

    public void OnJumpButtonPressed()
    {
        if (!CanJump) return;
        if (!dnfRigidbody.IsGround) return;

        moveController.Jump();
    }

    public void OnAttackButtonPressed()
    {
        attackController.OnSkillButtonPressed(EKeyName.BASEATTACK);
        
        if (!CanAttack) return;
        if (!attackController.CheckCanAttack(EKeyName.BASEATTACK)) return;

        attackController.Attack(EKeyName.BASEATTACK);
    }

    public void OnAttackButtonReleased()
    {
        attackController.OnSkillButtonReleased(EKeyName.BASEATTACK);
    }

    public void OnSkillButtonPressed(EKeyName keyName)
    {
        attackController.OnSkillButtonPressed(keyName);
        
        if (!CanAttack) return;
        if (!attackController.CheckCanAttack(keyName)) return;

        attackController.Attack(keyName);
    }

    public void OnSkillButtonReleased(EKeyName keyName)
    {
        attackController.OnSkillButtonReleased(keyName);
    }

    #endregion Event Methods
}
