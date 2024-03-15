using UnityEngine;

public class SortedSprite : MonoBehaviour
{
    #region Variables 

    private SpriteSorter sortingController = null;

    [SerializeField] private DNFTransform dnfTransform = null;
    [SerializeField] private SpriteRenderer[] spriteRenderers = null;
    [SerializeField] private ESortingType sortingType = ESortingType.NONE;

    #endregion Variables

    #region Unity Events

    private void Start()
    {
#if UNITY_EDITOR
        
        if (sortingType == ESortingType.NONE)
        {
            Debug.LogWarning($"Sorting type is not set. GameObject : {gameObject.name}");
        }

#endif
        sortingController = FindObjectOfType<SpriteSorter>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sortingOrder = sortingController.GetSortingOrder(dnfTransform);
        }
    }

    private void Update()
    {
        if (sortingType != ESortingType.UPDATE) return;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sortingOrder = sortingController.GetSortingOrder(dnfTransform);
        }
    }

    #endregion Unity Events

}
