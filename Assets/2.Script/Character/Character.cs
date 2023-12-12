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
    private CharacterAttack acttackController = null; 

    private bool canMove = true;
    private bool canJump = true;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();
        
        moveController = GetComponent<CharacterMove>();
        acttackController = GetComponent<CharacterAttack>();

        moveController.Init(dnfRigidbody);
    }

    private void Start()
    {
        // Debug
        Camera.main.GetComponent<CameraFollow>().SetTarget(transform);

        GameManager.Input.SetMovementDelegate(OnJoystickMoved);

        GameManager.Input.SetButtonDelegate(EKeyName.JUMP, OnJumpButtonPressed);
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


    #endregion Event Methods
}
