using UnityEngine;

public class KeyboardButton : PlayerButton
{
    #region Variables

    private KeyCode keyCode = KeyCode.None;

    #endregion Variables

    public override void SetButtonState()
    {
        if (Input.GetKeyDown(keyCode))
        {
            isPressed = true;
        }
        if (Input.GetKeyUp(keyCode))
        {
            isPressed = false;
        }

        if (isPressed)
        {
            buttonState = onPressed ? EButtonState.PRESSED : EButtonState.DOWN;
            onPressed = true;
        }
        else
        {
            buttonState = onPressed ? EButtonState.UP : EButtonState.IDLE;
            onPressed = false;
        }
        Debug.Log(buttonState);
    }
}
