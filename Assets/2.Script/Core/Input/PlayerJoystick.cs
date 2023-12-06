using UnityEngine;

public abstract class PlayerJoystick : MonoBehaviour
{
    #region Variables

    protected Vector3 moveDirection = Vector3.zero;

    #endregion Variables

    #region Properties

    public Vector3 MoveDirection => moveDirection;

    #endregion Properties

    #region Unity Events

    private void Update()
    {
        SetDirection();
    }

    #endregion Unity Events

    #region Abstract Methods

    protected abstract void SetDirection();

    #endregion Abstract Methods
}