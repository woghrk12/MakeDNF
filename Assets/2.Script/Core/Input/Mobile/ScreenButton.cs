using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenButton : PlayerButton, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    #region Variables

    private Image buttonImage = null;
    private RectTransform buttonTransform = null;

    [Header("Positions for button rect")]
    private Vector2 minPos = Vector2.zero;
    private Vector2 maxPos = Vector2.zero;

    [Header("Colors for button events")]
    private Color normalColor = new Color(1f, 1f, 1f, 1f);
    private Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonTransform = transform as RectTransform;

        minPos = buttonTransform.anchorMin;
        maxPos = buttonTransform.anchorMax;
    }

    #endregion Unity Events

    #region Interface Implements

    #region IPointerDownHandler

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        buttonImage.color = pressedColor;
    }

    #endregion IPointerDownHandler

    #region IDragHandler

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPressed) return;

        Vector2 position = eventData.position;

        // Out of X range of button rect
        if (position.x < minPos.x || position.x > maxPos.x)
        {
            isPressed = false;
            buttonImage.color = normalColor;
            return;
        }

        // Out of Y range of button rect
        if (position.y < minPos.y || position.y > maxPos.y)
        {
            isPressed = false;
            buttonImage.color = normalColor;
            return;
        }
    }

    #endregion IDragHandler

    #region IPointerUpHandler

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        buttonImage.color = normalColor;
    }

    #endregion IPointerUpHandler

    #endregion Interface Implements
}
