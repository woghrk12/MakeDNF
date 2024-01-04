using UnityEngine;

public class KeyboardJoystick : PlayerJoystick
{
    #region Variables

    [Header("KeyCodes for directional input")]
    private KeyCode upKeyCode = KeyCode.UpArrow;
    private KeyCode downKeyCode = KeyCode.DownArrow;
    private KeyCode leftKeyCode = KeyCode.LeftArrow;
    private KeyCode rightKeyCode = KeyCode.RightArrow;

    /// <summary>
    /// The last input direction of horizontal movement.
    /// </summary>
    private int lastPressedHorizontal = 0;
    
    /// <summary>
    /// The last input direction of vertical movement.
    /// </summary>
    private int lastPressedVertical = 0;

    #endregion Variables

    #region Methods

    #region Override

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

    #endregion Override

    #endregion Methods
}
