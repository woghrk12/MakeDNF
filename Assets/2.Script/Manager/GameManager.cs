using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    #region Variables

    [Header("Core Manager")]
    private static InputManager inputManager = new();
    private static ResourceManager resourceManager = new();

    #endregion Variables

    #region Properties

    public static InputManager Input => inputManager;
    public static ResourceManager Resource => resourceManager;

    #endregion Properties

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        inputManager.Init();
    }

    #endregion Unity Events
}
