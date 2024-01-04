using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenJoystick : PlayerJoystick, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    #region Variables

    [Header("Rect Transform for calculating the directional input")]
    private RectTransform background = null;
    private RectTransform handle = null;

    private Vector2 inputDir = Vector2.zero;
    private float radius = 0f;
    private float invRadius = 0f;

    [SerializeField, Range(0, 1)] private float minHandleRange = 0f;
    [SerializeField, Range(0, 1)] private float maxHandleRange = 0f;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        background = transform.GetChild(0) as RectTransform;
        handle = transform.GetChild(1) as RectTransform;

        radius = background.sizeDelta.x * 0.5f;
        invRadius = 1 / radius;
    }

    #endregion Unity Events

    #region Methods

    #region Override

    /// <summary>
    /// Replace the user's directional input with the DNFTransform as the direction. 
    /// Move in the direction in which the joystick handler moved.
    /// </summary>
    protected override void SetDirection()
    {
        moveDirection.x = inputDir.x >= minHandleRange ? 1f : (inputDir.x <= -minHandleRange ? -1f : 0f);
        moveDirection.z = inputDir.y >= minHandleRange ? 1f : (inputDir.y <= -minHandleRange ? -1f : 0f);
    }

    #endregion Override

    #region Helper

    /// <summary>
    /// Move the joystick handle based on user input position
    /// </summary>
    /// <param name="eventPos">The touch point of the player input</param>
    private void ControlHandle(Vector2 eventPos)
    {
        inputDir = (eventPos - (Vector2)background.position) * invRadius;
        inputDir = HandleInput(inputDir);
        handle.anchoredPosition = radius * inputDir;
    }

    /// <summary>
    /// Clamp the player's input within the joystick range.
    /// </summary>
    private Vector2 HandleInput(Vector2 inputDir)
    {
        float sqrMagnitude = inputDir.sqrMagnitude;

        if (sqrMagnitude > minHandleRange * minHandleRange)
        {
            inputDir = inputDir.normalized * maxHandleRange;
        }
        else if (sqrMagnitude < maxHandleRange * maxHandleRange)
        {
            inputDir = inputDir.normalized * minHandleRange;
            moveDirection = Vector2.zero;
        }

        return inputDir;
    }

    #endregion Helper

    #endregion Methods

    #region Interface Implements

    public void OnPointerDown(PointerEventData eventData)
    {
        ControlHandle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ControlHandle(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputDir = Vector3.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    #endregion Interface Implements
}
