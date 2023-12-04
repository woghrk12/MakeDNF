using UnityEngine;

public class KeyboardButton : PlayerButton
{
    #region Variables

    private KeyCode keyCode = KeyCode.None;

    #endregion Variables

    #region Unity Events

    protected override void Update()
    {
        SetButtonState();

        base.Update();
    }

    #endregion Unity Events

    #region Methods

    private void SetButtonState()
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
