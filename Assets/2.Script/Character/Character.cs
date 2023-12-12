using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Variables

    private DNFTransform dnfTransform = null;
    private DNFRigidbody dnfRigidbody = null;

    [Header("Character components")]
    private CharacterMove characterMove = null;

    private bool canMove = true;
    private bool canJump = true;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();

        characterMove = GetComponent<CharacterMove>();

        characterMove.Init(dnfRigidbody);
    }

    private void Start()
    {
        // Debug
        Camera.main.GetComponent<CameraFollow>().SetTarget(transform);
        
        GameManager.Input.SetMovementDelegate(Move);

        GameManager.Input.SetButtonDelegate(EKeyName.JUMP, Jump);
    }

    #endregion Unity Events

    #region Methods

    private void Move(Vector3 direction)
    {
        if (!canMove) return;

        characterMove.Move(direction);
    }

    private void Jump()
    {
        if (!canJump) return;

        characterMove.Jump();
    }

    #endregion Methods
}
