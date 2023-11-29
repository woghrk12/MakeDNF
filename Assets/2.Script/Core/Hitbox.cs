using UnityEngine;

public enum EHitboxType { NONE = -1, BOX, CIRCLE, END }

[RequireComponent(typeof(DNFTransform))]
public class Hitbox : MonoBehaviour
{
    #region Variables

    private DNFTransform dnfTransform = null;

    [SerializeField] private EHitboxType hitboxType = EHitboxType.NONE;

    [Header("Variables for the hitbox shape")]
    [HideInInspector] public float SizeX = 0f;
    [HideInInspector] public float SizeY = 0f;
    [HideInInspector] public float SizeZ = 0f;
    [HideInInspector] public float OffsetX = 0f;
    [HideInInspector] public float OffsetY = 0f;
    [HideInInspector] public float OffsetZ = 0f;
    [HideInInspector, Range(0f, 1f)] public float PivotX = 0f;
    [HideInInspector, Range(0f, 1f)] public float PivotY = 0f;
    [HideInInspector, Range(0f, 1f)] public float PivotZ = 0f;
    
    private Vector3 maxHitboxPos = Vector3.zero;
    private Vector3 minHitboxPos = Vector3.zero;

    #endregion Variable

    public Vector3 MaxHitboxPos => maxHitboxPos;
    public Vector3 MinHitboxPos => minHitboxPos;

    #region Unity Events

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();
    }

    private void OnValidate()
    {
        if (SizeX < 0f) SizeX = 0f;
        if (SizeY < 0f) SizeY = 0f;
        if (SizeZ < 0f) SizeZ = 0f;
    }

    #endregion Unity Events

    #region Methods

    public void CalculateHitbox()
    {
        Vector3 position = dnfTransform.Position;

        position += new Vector3(OffsetX, OffsetY, OffsetZ);

        minHitboxPos = position - new Vector3(SizeX * PivotX, SizeY * PivotY, SizeZ * PivotZ);
        maxHitboxPos = position + new Vector3(SizeX * (1f - PivotX), SizeY * (1f - PivotY), SizeZ * (1f - PivotZ));
    }

    #endregion Methods
}
