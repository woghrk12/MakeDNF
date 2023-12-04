using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenJoystick : PlayerJoystick, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    #region Variables

    private RectTransform background = null;
    private RectTransform handle = null;

    private Vector2 inputDir = Vector2.zero;
    private float radius = 0f;
    private float invRadius = 0f;

    [SerializeField] private float handleRange = 0f;
    [SerializeField, Range(0, 1)] private float minValue = 0f;
    [SerializeField, Range(0, 1)] private float maxValue = 0f;

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

    #region Override Methods

    public override void SetDirection()
    {
        moveDirection.x = inputDir.x > minValue ? 1f : (inputDir.x < -minValue ? -1f : 0f);
        moveDirection.z = inputDir.y > minValue ? 1f : (inputDir.y < -minValue ? -1f : 0f);
    }

    #endregion Override Methods

    #region Interface Methods

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

    #endregion Interface Methods

    #region Methods

    private void ControlHandle(Vector2 eventPos)
    {
        inputDir = (eventPos - (Vector2)background.position) * invRadius;
        HandleInput(inputDir.sqrMagnitude);
        handle.anchoredPosition = radius * handleRange * inputDir;
    }

    private void HandleInput(float sqrMagnitude)
    {
        if (sqrMagnitude > minValue * minValue)
        {
            if (sqrMagnitude > maxValue * maxValue)
            {
                inputDir = inputDir.normalized * maxValue;
            }
        }
        else
        {
            moveDirection = Vector2.zero;
        }
    }

    #endregion Methods
}
