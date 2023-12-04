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

    private void Update()
    {
        SetButtonState();
    }

    #endregion Unity Events

    #region Abstract Methods

    public abstract void SetButtonState();

    #endregion Abstract Methods
}
