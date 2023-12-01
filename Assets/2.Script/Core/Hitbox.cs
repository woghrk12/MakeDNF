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

    #region AABB Collision

    public bool CheckCollision(Hitbox other)
    {
        if (hitboxType == EHitboxType.BOX)
        {
            if (other.hitboxType == EHitboxType.BOX)
            {
                return CheckBB(this, other);
            }

            return CheckBC(this, other);
        }

        if (other.hitboxType == EHitboxType.CIRCLE)
        {
            return CheckCC(this, other);
        }

        return CheckBC(other, this);
    }

    private bool CheckBB(Hitbox fromBox, Hitbox toBox)
    {
        Vector3 fromMinHitboxPos = fromBox.minHitboxPos;
        Vector3 fromMaxHitboxPos = fromBox.maxHitboxPos;
        Vector3 toMinHitboxPos = toBox.minHitboxPos;
        Vector3 toMaxHitboxPos = toBox.maxHitboxPos;

        if (fromMaxHitboxPos.x < toMinHitboxPos.x || fromMinHitboxPos.x > toMaxHitboxPos.x) return false;
        if (fromMaxHitboxPos.z < toMinHitboxPos.z || fromMinHitboxPos.z > toMaxHitboxPos.z) return false;

        return CheckYCollision(fromBox, toBox);
    }

    private bool CheckCC(Hitbox fromCircle, Hitbox toCircle)
    {
        Vector3 fromMinHitboxPos = fromCircle.minHitboxPos;
        Vector3 fromMaxHitboxPos = fromCircle.maxHitboxPos;
        Vector3 toMinHitboxPos = toCircle.minHitboxPos;
        Vector3 toMaxHitboxPos = toCircle.maxHitboxPos;

        Vector3 fromCenter = (fromMinHitboxPos + fromMaxHitboxPos) * 0.5f;
        float fromRadius = fromMaxHitboxPos.x - fromMinHitboxPos.x;
        Vector3 toCenter = (toMinHitboxPos + toMaxHitboxPos) * 0.5f;
        float toRadius = toMaxHitboxPos.x - toMinHitboxPos.x;

        if ((fromRadius + toRadius) * (fromRadius + toRadius) < (fromCenter - toCenter).sqrMagnitude) return false;

        return CheckYCollision(fromCircle, toCircle);
    }

    private bool CheckBC(Hitbox box, Hitbox circle)
    {
        Vector3 boxMinHitboxPos = box.minHitboxPos;
        Vector3 boxMaxHitboxPos = box.maxHitboxPos;
        Vector3 circleMinHitboxPos = circle.minHitboxPos;
        Vector3 circleMaxHitboxPos = circle.maxHitboxPos;

        Vector3 boxCenter = (boxMinHitboxPos + boxMaxHitboxPos) * 0.5f;
        float width = boxMaxHitboxPos.x - boxMinHitboxPos.x;
        float height = boxMaxHitboxPos.z - boxMinHitboxPos.z;
        Vector3 circleCenter = (circleMinHitboxPos + circleMaxHitboxPos) * 0.5f;
        float radius = (circleMaxHitboxPos.x - circleMinHitboxPos.x) * 0.5f;

        int rectNum = (circleCenter.x < boxMinHitboxPos.x ? 0 : (circleCenter.x > boxMaxHitboxPos.x ? 2 : 1))
            + 3 * (circleCenter.z < boxMinHitboxPos.z ? 0 : (circleCenter.z > boxMaxHitboxPos.z ? 2 : 1));

        switch (rectNum)
        {
            case 1:
            case 7:
                float diffZ = boxCenter.z < circleCenter.z ? circleCenter.z - boxCenter.z : boxCenter.z - circleCenter.z;
                if (height * 0.5f + radius < diffZ) return false;
                break;

            case 3:
            case 5:
                float diffX = boxCenter.x < circleCenter.x ? circleCenter.x - boxCenter.x : boxCenter.x - circleCenter.x;
                if (width * 0.5f + radius < diffX) return false;
                break;

            default:
                float cornerX = (rectNum == 0 || rectNum == 6) ? boxMinHitboxPos.x : boxMaxHitboxPos.x;
                float cornerZ = (rectNum == 0 || rectNum == 2) ? boxMinHitboxPos.z : boxMaxHitboxPos.z;
                if (!CheckInsideCircle(new Vector3(cornerX, 0f, cornerZ), circleCenter, radius)) return false;
                break;
        }

        return CheckYCollision(box, circle);
    }

    private bool CheckYCollision(Hitbox from, Hitbox to)
    {
        if (!from.dnfTransform.HasYObj || !to.dnfTransform.HasYObj) return true;

        if (from.maxHitboxPos.y < to.minHitboxPos.y || from.minHitboxPos.y > to.maxHitboxPos.y) return false;

        return true;
    }

    #region Helper Methods

    private bool CheckInsideCircle(Vector3 point, Vector3 center, float radius)
    {
        point.y = 0f;
        center.y = 0f;

        return (center - point).sqrMagnitude < radius * radius;
    }

    #endregion Helper Methods

    #endregion AABB Collision

    #endregion Methods
}
