using UnityEngine;

public class KeyboardJoystick : PlayerJoystick
{
    #region Variables

    [Header("KeyCodes for directional input")]
    [SerializeField] private KeyCode upKeyCode = KeyCode.None;
    [SerializeField] private KeyCode downKeyCode = KeyCode.None;
    [SerializeField] private KeyCode leftKeyCode = KeyCode.None;
    [SerializeField] private KeyCode rightKeyCode = KeyCode.None;

    private int lastPressedHorizontal = 0;
    private int lastPressedVertical = 0;

    #endregion Variables

    #region Override Methods

    /// <summary>
    /// Replace the user's directional input with the DNFTransform as the direction. 
    /// Move in the direction of the last pressed keyboard input.
    /// </summary>
    protected override void SetDirection()
    {
        bool isLeft = Input.GetKey(leftKeyCode);
        bool isRight = Input.GetKey(rightKeyCode);
        bool isUp = Input.GetKey(upKeyCode);
        bool isDown = Input.GetKey(downKeyCode);

        // Horizontal movement
        if (isLeft && isRight)
        {
            moveDirection.x = lastPressedHorizontal * -1;
        }
        else if (isLeft)
        {
            moveDirection.x = -1;
            lastPressedHorizontal = -1;
        }
        else if (isRight)
        {
            moveDirection.x = 1;
            lastPressedHorizontal = 1;
        }
        else
        {
            moveDirection.x = 0;
            lastPressedHorizontal = 0;
        }

        // Vertical movement
        if (isUp && isDown)
        {
            moveDirection.z = lastPressedVertical * -1;
        }
        else if (isDown)
        {
            moveDirection.z = -1;
            lastPressedVertical = -1;
        }
        else if (isUp)
        {
            moveDirection.z = 1;
            lastPressedVertical = 1;
        }
        else
        {
            moveDirection.z = 0;
            lastPressedVertical = 0;
        }
    }

    #endregion Override Methods
}
