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

    /// <summary>
    /// Calculate the hitbox by using the value according to DNF transform.
    /// </summary>
    private void CalculateHitbox()
    {
        Vector3 position = dnfTransform.Position;
        float localScale = dnfTransform.LocalScale;

        position += Offset;

        minHitboxPos = position - new Vector3(Size.x * Pivot.x * localScale, Size.y * Pivot.y * localScale, Size.z * Pivot.z * localScale);
        maxHitboxPos = position + new Vector3(Size.x * (1f - Pivot.x) * localScale, Size.y * (1f - Pivot.y) * localScale, Size.z * (1f - Pivot.z) * localScale);
    }

    #region AABB Collision

    /// <summary>
    /// Check for collisions with the hitbox object passed as a parameter.
    /// Call the appropriate collision detection function based on the shapes of the two hitboxes.
    /// </summary>
    /// <param name="other">The target hitbox for checking collision</param>
    /// <returns>true if a collision occurs</returns>
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

    /// <summary>
    /// Check for collisions between box-shaped hitbox objects in the XZ coordinate.
    /// Compare the maximum and minimum values along each axis.
    /// </summary>
    /// <param name="box1">One of the box-shaped hitbox for checking collisions</param>
    /// <param name="box2">One of the box-shaped hitbox for checking collisions</param>
    /// <returns>true if a collision occurs in XZ coordinate</returns>
    private bool CheckBB(Hitbox box1, Hitbox box2)
    {
        Vector3 fromMinHitboxPos = box1.minHitboxPos;
        Vector3 fromMaxHitboxPos = box1.maxHitboxPos;
        Vector3 toMinHitboxPos = box2.minHitboxPos;
        Vector3 toMaxHitboxPos = box2.maxHitboxPos;

        if (fromMaxHitboxPos.x < toMinHitboxPos.x || fromMinHitboxPos.x > toMaxHitboxPos.x) return false;
        if (fromMaxHitboxPos.z < toMinHitboxPos.z || fromMinHitboxPos.z > toMaxHitboxPos.z) return false;

        return CheckYCollision(box1, box2);
    }

    /// <summary>
    /// Check for collisions between circle-shaped hitbox objects.
    /// Compare the distance between the centers of each circle with the sum of their radius.
    /// </summary>
    /// <param name="circle1">One of the circle-shaped hitbox for checking collisions</param>
    /// <param name="circle2">One of the circle-shaped hitbox for checking collisions</param>
    /// <returns>true if a collision occurs in XZ coordinate</returns>
    private bool CheckCC(Hitbox circle1, Hitbox circle2)
    {
        Vector3 fromMinHitboxPos = circle1.minHitboxPos;
        Vector3 fromMaxHitboxPos = circle1.maxHitboxPos;
        Vector3 toMinHitboxPos = circle2.minHitboxPos;
        Vector3 toMaxHitboxPos = circle2.maxHitboxPos;

        Vector3 fromCenter = (fromMinHitboxPos + fromMaxHitboxPos) * 0.5f;
        float fromRadius = fromMaxHitboxPos.x - fromMinHitboxPos.x;
        Vector3 toCenter = (toMinHitboxPos + toMaxHitboxPos) * 0.5f;
        float toRadius = toMaxHitboxPos.x - toMinHitboxPos.x;

        if ((fromRadius + toRadius) * (fromRadius + toRadius) < (fromCenter - toCenter).sqrMagnitude) return false;

        return CheckYCollision(circle1, circle2);
    }

    /// <summary>
    /// Check for collisions between the box-shaped hitbox and the circle-shaped hitbox.
    /// Divide the space around the box into 9 regions and determine based on the position of the circle.
    /// </summary>
    /// <param name="box">the box-shaped hitbox for checking collisions</param>
    /// <param name="circle">the circle-shaped hitbox for checking collisions</param>
    /// <returns>true if a collision occurs in XZ coordinate</returns>
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

    /// <summary>
    /// Check for collisions between hitbox objects in the Y coordinate.
    /// Compare the range of maximum and minimum y-values.
    /// </summary>
    /// <param name="hitbox1">One of the hitbox objects for checking the collisions</param>
    /// <param name="hitbox2">One of the hitbox objects for checking the collisions</param>
    /// <returns>true if a collision occurs in Y coordinate</returns>
    private bool CheckYCollision(Hitbox hitbox1, Hitbox hitbox2)
    {
        if (!hitbox1.dnfTransform.HasYObj || !hitbox2.dnfTransform.HasYObj) return true;

        if (hitbox1.maxHitboxPos.y < hitbox2.minHitboxPos.y || hitbox1.minHitboxPos.y > hitbox2.maxHitboxPos.y) return false;

        return true;
    }

    #region Helper Methods

    /// <summary>
    /// Check if the point is inside the circle.
    /// Used when comparing a box-shaped hitbox and a circle-shaped hitbox
    /// </summary>
    /// <param name="point">The position for checking whether the point is inside the circle</param>
    /// <param name="center">The center position of the circle</param>
    /// <param name="radius">The radius of the circle</param>
    /// <returns>true if the point is inside the circle</returns>
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
