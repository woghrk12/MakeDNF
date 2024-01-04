using UnityEngine;

public abstract class PlayerButton : MonoBehaviour
{
    public delegate void ButtonDown();
    public delegate void ButtonUp();

    #region Variables

    [SerializeField] private EKeyName keyName = EKeyName.NONE;

    /// <summary>
    /// Whether the button is pressed during this frame.
    /// </summary>
    protected bool isPressed = false;
    
    /// <summary>
    /// Whether the button is in a pressed state.
    /// </summary>
    protected bool onPressed = false;

    [Header("Delegates for button events")]
    public ButtonDown ButtonDownDelegate = null;
    public ButtonUp ButtonUpDelegate = null;
    
    #endregion Variables

    #region Properties

    /// <summary>
    /// Indicate the function interacting with the button.
    /// </summary>
    public EKeyName KeyName => keyName;

    #endregion Properties

    #region Unity Events

    protected virtual void Update()
    {
        if (isPressed)
        {
            if (!onPressed)
            {
                ButtonDownDelegate?.Invoke();
            }

            onPressed = true;
        }
        else
        {
            if (onPressed)
            {
                ButtonUpDelegate?.Invoke();
            }

            onPressed = false;
        }
    }

    #endregion Unity Events
}
