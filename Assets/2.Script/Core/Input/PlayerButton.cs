using UnityEngine;

public abstract class PlayerButton : MonoBehaviour
{
    public enum EButtonState { IDLE, DOWN, PRESSED, UP }

    #region Variables
    
    protected bool isPressed = false;
    protected bool onPressed = false;

    protected EButtonState buttonState = EButtonState.IDLE;

    #endregion Variables

    #region Properties

    public EButtonState ButtonState => buttonState;

    #endregion Properties

    #region Unity Events

    protected virtual void Update()
    {
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
    }

    #endregion Unity Events
}
