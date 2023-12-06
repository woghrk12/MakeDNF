using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    #region Variables

    private PlayerJoystick playerJoystick = null;
    private Dictionary<EKeyName, PlayerButton> playerButtonDictionary = new();

    #endregion Variables

    #region Properties

    /// <summary>
    /// Joystick class handling user's directional input.
    /// The returned direction vector is based on the DNFTransform.
    /// </summary>
    public PlayerJoystick PlayerJoystick => playerJoystick;
    
    /// <summary>
    /// The dictionary containing player buttons. 
    /// Key : the role of the button. (Base Attack, Jump, etc)
    /// </summary>
    public Dictionary<EKeyName, PlayerButton> PlayerButtonDictionary => playerButtonDictionary;

    #endregion Properties

    public void Init()
    {
        Transform inputSystem = GameManager.Resource.Instantiate(
#if UNITY_EDITOR || UNITY_STANDALONE
            ResourcePath.KEYBOARD_INPUT_SYSTEM
#else
            ResourcePath.SCREEN_INPUT_SYSTEM
#endif
            , GameManager.Instance.transform).transform;

        // Initialize the player joystick
        playerJoystick = inputSystem.GetChild(0).GetComponent<PlayerJoystick>();
        
        // Initialize the player buttons
        int childCount = inputSystem.childCount;
        for (int i = 1; i < childCount; i++)
        {
            if (!inputSystem.GetChild(i).TryGetComponent(out PlayerButton button))
            {
                throw new System.Exception($"There is no button component. Name : {inputSystem.GetChild(i).name}");
            }

            if (button.KeyName == EKeyName.NONE)
            {
                throw new System.Exception($"Button is not setting. Name : {button.name}");
            }

            if (playerButtonDictionary.ContainsKey(button.KeyName))
            {
                throw new System.Exception($"Button is already exist. Previous : {playerButtonDictionary[button.KeyName].name} / New : {button.name}");
            }

            playerButtonDictionary.Add(button.KeyName, button);   
        }
    }
}
