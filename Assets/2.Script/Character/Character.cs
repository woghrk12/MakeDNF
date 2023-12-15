using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Variables

    private DNFTransform dnfTransform = null;
    private DNFRigidbody dnfRigidbody = null;

    [Header("Character components")]
    private CharacterMove moveController = null;
    private CharacterAttack attackController = null;

    private bool canMove = true;
    private bool canJump = true;
    private bool canAttack = true;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();

        moveController = GetComponent<CharacterMove>();
        attackController = GetComponent<CharacterAttack>();

        moveController.Init(dnfRigidbody);
        attackController.Init(dnfTransform);
    }

    private void Start()
    {
        // Debug
        Camera.main.GetComponent<CameraFollow>().SetTarget(transform);

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
        if (!canMove) return;

        moveController.Move(direction);
    }

    public void OnJumpButtonPressed()
    {
        if (!canJump) return;

        moveController.Jump();
    }

    public void OnAttackButtonPressed()
    {
        attackController.OnSkillButtonPressed(EKeyName.BASEATTACK);
        
        if (!canAttack) return;
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
        
        if (!canAttack) return;
        if (!attackController.CheckCanAttack(keyName)) return;

        attackController.Attack(keyName);
    }

    public void OnSkillButtonReleased(EKeyName keyName)
    {
        attackController.OnSkillButtonReleased(keyName);
    }

    #endregion Event Methods
}
