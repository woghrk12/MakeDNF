using UnityEngine;

public class KeyboardButton : PlayerButton
{
    #region Variables

    [SerializeField] private KeyCode keyCode = KeyCode.None;

    #endregion Variables

    #region Unity Events

    protected override void Update()
    {
        HandleInputFlag();

        base.Update();
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Handle the flag variable based on the player's button input status.
    /// </summary>
    private void HandleInputFlag()
    {
        if (Input.GetKeyDown(keyCode))
        {
            isPressed = true;
        }
        if (Input.GetKeyUp(keyCode))
        {
            isPressed = false;
        }
    }

    #endregion Methods
}
