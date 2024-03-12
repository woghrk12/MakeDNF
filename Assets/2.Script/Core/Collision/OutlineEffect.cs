using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private Transform outlineTransform = null;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private SpriteRenderer outlineSpriteRenderer = null;

    /// <summary>
    /// 
    /// </summary>
    private float localScaleValue = 1f;

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

    public void SetOutlineEffect(EHitboxState hitboxState)
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
