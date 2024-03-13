using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// The transform component of the outline object.
    /// </summary>
    [SerializeField] private Transform outlineTransform = null;

    /// <summary>
    /// The sprite renderer component of the outline object.
    /// </summary>
    [SerializeField] private SpriteRenderer outlineSpriteRenderer = null;

    /// <summary>
    /// The value of local scale for outline effect object.
    /// </summary>
    private float localScaleValue = 1f;

    /// <summary>
    /// The hitbox state of the object with applied outline effect.
    /// </summary>
    private EHitboxState hitboxState = EHitboxState.NONE;

    #endregion Variables

    #region Unity Events

    private void Update()
    {
        if (localScaleValue > 1f)
        {
            localScaleValue -= 0.04f;

            if (localScaleValue < 1f)
            {
                localScaleValue = 1f;
            }

            outlineTransform.localScale = new Vector3(localScaleValue, localScaleValue, 1f);
        }
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Apply the outline effect to the object.
    /// When the hitbox state of the object is NONE state, the outline will disappear.
    /// When the hitbox state of the object is SUPERARMOR state, the outline starts large and gradually shrinks.
    /// And the outline will alternate between yellow and red colors.
    /// When the hitbox state of the object is INVINCIBILITY state, the outline color will be white.
    /// </summary>
    /// <param name="hitboxState">The hitbox state of the object to which the outline effect will be applied</param>
    public void ApplyOutlineEffect(EHitboxState hitboxState)
    {
        if (this.hitboxState == hitboxState) return;

        switch (hitboxState)
        {
            case EHitboxState.NONE:
                outlineTransform.gameObject.SetActive(false);

                break;

            case EHitboxState.SUPERARMOR:
                outlineTransform.gameObject.SetActive(true);

                localScaleValue = 1.4f;

                outlineTransform.localScale = new Vector3(localScaleValue, localScaleValue, 1f);
                outlineSpriteRenderer.material.SetInt("_State", (int)EHitboxState.SUPERARMOR);

                break;

            case EHitboxState.INVINCIBILITY:
                outlineTransform.gameObject.SetActive(true);

                localScaleValue = 1f;

                outlineTransform.localScale = new Vector3(localScaleValue, localScaleValue, 1f);
                outlineSpriteRenderer.material.SetInt("_State", (int)EHitboxState.INVINCIBILITY);

                break;
        }

        this.hitboxState = hitboxState;
    }

    #endregion Methods
}
