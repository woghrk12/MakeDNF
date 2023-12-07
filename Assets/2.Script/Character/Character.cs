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
        
        GameManager.Input.SetMovementDelegate(characterMove.Move);

        GameManager.Input.SetButtonDelegate(EKeyName.JUMP, characterMove.Jump);
    }

    #endregion Unity Events
}
