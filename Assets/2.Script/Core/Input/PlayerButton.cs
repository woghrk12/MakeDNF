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
    public ButtonDown buttonDownDelegate = null;
    public ButtonUp buttonUpDelegate = null;
    
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
                buttonDownDelegate?.Invoke();
            }

            onPressed = true;
        }
        else
        {
            if (onPressed)
            {
                buttonUpDelegate?.Invoke();
            }

            onPressed = false;
        }
    }

    #endregion Unity Events
}
