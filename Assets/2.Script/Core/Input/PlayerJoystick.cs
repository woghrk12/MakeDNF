using UnityEngine;

public abstract class PlayerJoystick : MonoBehaviour
{
    #region Variables

    protected Vector3 moveDirection = Vector3.zero;

    #endregion Variables

    #region Properties

    /// <summary>
    /// Direction vector in the DNFTransform.
    /// </summary>
    public Vector3 MoveDirection => moveDirection;

    #endregion Properties

    #region Unity Events

    private void Update()
    {
        SetDirection();
    }

    #endregion Unity Events

    #region Abstract Methods

    /// <summary>
    /// Replace the user's directional input with the DNFTransform as the direction. 
    /// </summary>
    protected abstract void SetDirection();

    #endregion Abstract Methods
}