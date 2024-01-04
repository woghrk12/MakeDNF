using UnityEngine;

public abstract class PlayerJoystick : MonoBehaviour
{
    public delegate void MoveCharacter(Vector3 direction);

    #region Variables

    /// <summary>
    /// Direction vector in the DNFTransform.
    /// </summary>
    protected Vector3 moveDirection = Vector3.zero;

    [Header("Delegate for directional input")]
    public MoveCharacter MoveCharacterDelegate = null;

    #endregion Variables

    #region Unity Events

    private void Update()
    {
        SetDirection();
    }

    private void FixedUpdate()
    {
        MoveCharacterDelegate?.Invoke(moveDirection);
    }

    #endregion Unity Events

    #region Methods

    #region Abstract

    /// <summary>
    /// Replace the user's directional input with the DNFTransform as the direction. 
    /// </summary>
    protected abstract void SetDirection();

    #endregion Abstract

    #endregion Methods
}