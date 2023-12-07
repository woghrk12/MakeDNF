using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Variables

    private DNFTransform dnfTransform = null;

    [Header("Character components")]
    private CharacterMove characterMove = null;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();

        characterMove = GetComponent<CharacterMove>();

        characterMove.Init(dnfTransform);
    }

    private void Start()
    {
        GameManager.Input.SetMovementDelegate(characterMove.Move);
    }

    #endregion Unity Events
}
