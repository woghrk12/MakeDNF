using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenButton : PlayerButton, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    #region Variables

    private Image buttonImage = null;
    private RectTransform buttonTransform = null;

    private Vector2 minPos = Vector2.zero;
    private Vector2 maxPos = Vector2.zero;

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

    #region Interface Methods

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        buttonImage.color = pressedColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPressed) return;

        Vector2 position = eventData.position;

        if (position.x < minPos.x || position.x > maxPos.x)
        {
            isPressed = false;
            buttonImage.color = normalColor;
            return;
        }

        if (position.y < minPos.y || position.y > maxPos.y)
        {
            isPressed = false;
            buttonImage.color = normalColor;
            return;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        buttonImage.color = normalColor;
    }


    #endregion Interface Methods
}
