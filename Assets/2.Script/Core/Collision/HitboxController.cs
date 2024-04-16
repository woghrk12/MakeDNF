using System;
using UnityEngine;

public enum EHitboxType { BOX, CIRCLE }

[Serializable]
public class Hitbox
{
    #region Variables

    /// <summary>
    /// The shape of the hitbox on the XZ plane.
    /// The shape of the hitbox is either box-shaped or circle-shaped.
    /// </summary>
    [SerializeField] private EHitboxType hitboxType = EHitboxType.BOX;

    /// <summary>
    /// The component determining the size of the hitbox.
    /// Each component of the size is a non-negative value.
    /// </summary>
    [SerializeField] private Vector3 size = Vector3.zero;

    /// <summary>
    /// The component determining how far the hitbox is from the object's position.
    /// </summary>
    [SerializeField] private Vector3 offset = Vector3.zero;

    /// <summary>
    /// The component determining the ratio of the center point of the hitbox.
    /// Each component of the pivot will be clamped between 0 and 1.
    /// </summary>
    [SerializeField] private Vector3 pivot = Vector3.zero;

    #endregion Variables

    #region Property

    public EHitboxType HitboxType => hitboxType;

    public Vector3 Size => size;

    public Vector3 Offset => offset;

    public Vector3 Pivot => pivot;

    #endregion Property
}

public partial class HitboxController : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// DNF transform component used to calculate the hitbox.
    /// </summary>
    private DNFTransform dnfTransform = null;

    [SerializeField] private Hitbox[] hitboxes = new Hitbox[0];

    /// <summary>
    /// The index of currently active hitbox.
    /// If the index is less than 0, the current hitbox is turned off and does not cause any collision. 
    /// </summary>
    [SerializeField] private int activeHitboxIndex = -1;

    /// <summary>
    /// The shape of the hitbox on the XZ plane.
    /// The shape of the hitbox is either box-shaped or circle-shaped.
    /// </summary>
    [SerializeField] private EHitboxType hitboxType = EHitboxType.BOX;

    /// <summary>
    /// The maximum values for each component of the hitbox.
    /// If the hitbox shape is a circle, the z-value has no effect.
    /// </summary>
    [SerializeField] private Vector3 minHitboxPos = Vector3.zero;

    /// <summary>
    /// The minimum values for each component of the hitbox.
    /// If the hitbox shape is a circle, the z-value has no effect.
    /// </summary>
    [SerializeField] private Vector3 maxHitboxPos = Vector3.zero;

    #endregion Variables

    #region Properties

    /// <summary>
    /// Determine whether the current hitbox is activated.
    /// Return true if the hitbox currently activated, and false otherwise.
    /// </summary>
    public bool IsHitboxActivated => activeHitboxIndex >= 0;

    #endregion Properties

    #region Unity Events

    private void Update()
    {
        if (!IsHitboxActivated) return;

        CalculateHitbox();
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Initialize the hitbox controller component.
    /// Use the given DNFTransform component to calculate the shape of the hitboxes.
    /// </summary>
    /// <param name="dnfTransform">DNFTransform component to be used when calculating the shape of hitboxes</param>
    public void Init(DNFTransform dnfTransform)
    {
        if (hitboxes.Length <= 0)
        {
            throw new Exception($"There is no hitbox. GameObject : {gameObject.name}");
        }

        this.dnfTransform = dnfTransform;
    }

    /// <summary>
    /// Enable the hitbox corresponding to the received index.
    /// </summary>
    /// <param name="index">The index of the hitbox to be activated</param>
    public void EnableHitbox(int index)
    {
        if (index < 0 || index >= hitboxes.Length)
        {
            throw new Exception($"Out of range. GameObject : {gameObject.name}. Input index : {index}");
        }

        activeHitboxIndex = index;

        hitboxType = hitboxes[activeHitboxIndex].HitboxType;

        CalculateHitbox();
    }

    /// <summary>
    /// Disable the currently active hitbox.
    /// </summary>
    public void DisableHitbox()
    {
        if (!IsHitboxActivated) return;

        activeHitboxIndex = -1;
        minHitboxPos = Vector3.zero;
        maxHitboxPos = Vector3.zero;
    }

    /// <summary>
    /// Check whether the collision occur.
    /// Call the appropriate collision detection function based on the shapes of the two hitboxes.
    /// </summary>
    /// <param name="other">The other hitbox controller for collision checks</param>
    /// <returns>True if both controller's active hitbox is not null and a collision has occurred</returns>
    public bool CheckCollision(HitboxController other)
    {
        if (!IsHitboxActivated || !other.IsHitboxActivated) return false;

        if (hitboxType == EHitboxType.BOX)
        {
            if (other.hitboxType == EHitboxType.BOX)
            {
                return CheckBB(this, other);
            }
            else
            {
                return CheckBC(this, other);
            }
        }
        else
        {
            if (other.hitboxType == EHitboxType.BOX)
            {
                return CheckBC(other, this);
            }
            else
            {
                return CheckCC(this, other);
            }
        }
    }

    #region Helper

    /// <summary>
    /// Calculate the currently active hitbox range.
    /// </summary>
    private void CalculateHitbox()
    {
        if (!IsHitboxActivated) return;

        Vector3 position = dnfTransform.Position;
        float localScale = dnfTransform.LocalScale;

        Hitbox activeHitbox = hitboxes[activeHitboxIndex];
        Vector3 size = activeHitbox.Size;
        Vector3 offset = activeHitbox.Offset;
        Vector3 pivot = activeHitbox.Pivot;

        Vector3 minHitboxPos = offset - localScale * new Vector3(size.x * pivot.x, size.y * pivot.y, size.z * pivot.z);
        Vector3 maxHitboxPos = offset + localScale * new Vector3(size.x * (1f - pivot.x), size.y * (1f - pivot.y), size.z * (1f - pivot.z));

        if (dnfTransform.IsLeft)
        {
            this.minHitboxPos = position + new Vector3(-maxHitboxPos.x, minHitboxPos.y, minHitboxPos.z);
            this.maxHitboxPos = position + new Vector3(-minHitboxPos.x, maxHitboxPos.y, maxHitboxPos.z);
        }
        else
        {
            this.minHitboxPos = position + minHitboxPos;
            this.maxHitboxPos = position + maxHitboxPos;
        }
    }

    /// <summary>
    /// Check for collisions between box-shaped hitbox objects in the XZ coordinate.
    /// Compare the maximum and minimum values along each axis.
    /// </summary>
    /// <returns>True if a collision occurs in XZ coordinate</returns>
    private bool CheckBB(HitboxController box1, HitboxController box2)
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
    /// <returns>True if a collision occurs in XZ coordinate</returns>
    private bool CheckCC(HitboxController circle1, HitboxController circle2)
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
    /// <returns>True if a collision occurs in XZ coordinate</returns>
    private bool CheckBC(HitboxController box, HitboxController circle)
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
    /// <returns>True if a collision occurs in Y coordinate</returns>
    private bool CheckYCollision(HitboxController hitbox1, HitboxController hitbox2)
    {
        if (hitbox1.maxHitboxPos.y < hitbox2.minHitboxPos.y || hitbox1.minHitboxPos.y > hitbox2.maxHitboxPos.y) return false;

        return true;
    }

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
        // Only performed on the XZ planes
        point.y = 0f;
        center.y = 0f;

        return (center - point).sqrMagnitude < radius * radius;
    }

    #endregion Helper

    #endregion Methods
}
