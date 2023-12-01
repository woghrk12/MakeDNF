using UnityEngine;

public enum EHitboxType { NONE = -1, BOX, CIRCLE }

[RequireComponent(typeof(DNFTransform))]
public class Hitbox : MonoBehaviour
{
    #region Variables

    private DNFTransform dnfTransform = null;

    [SerializeField] private EHitboxType hitboxType = EHitboxType.NONE;

    [Header("Variables for the hitbox shape")]
    [HideInInspector] public Vector3 Size = new Vector3(1f, 1f, 1f);
    [HideInInspector] public Vector3 Offset = Vector3.zero;
    [HideInInspector] public Vector3 Pivot = Vector3.zero;

    private Vector3 maxHitboxPos = Vector3.zero;
    private Vector3 minHitboxPos = Vector3.zero;

    #endregion Variable

    #region Properties

    public EHitboxType HitboxType => hitboxType;
    public Vector3 MaxHitboxPos => maxHitboxPos;
    public Vector3 MinHitboxPos => minHitboxPos;

    #endregion Properties

    #region Unity Events

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();
    }

    private void Update()
    {
        CalculateHitbox();
    }

    #endregion Unity Events

    #region Methods

    public void CalculateHitbox()
    {
        Vector3 position = dnfTransform.Position;
        float localScale = dnfTransform.LocalScale;

        position += Offset;

        minHitboxPos = position - new Vector3(Size.x * Pivot.x * localScale, Size.y * Pivot.y * localScale, Size.z * Pivot.z * localScale);
        maxHitboxPos = position + new Vector3(Size.x * (1f - Pivot.x) * localScale, Size.y * (1f - Pivot.y) * localScale, Size.z * (1f - Pivot.z) * localScale);
    }

    #endregion Methods
}
