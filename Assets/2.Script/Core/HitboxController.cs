using System;
using System.Collections.Generic;
using UnityEngine;

public enum EHitboxType { BOX, CIRCLE }

/// <summary>
/// Interface representing an subject capable of performing attacks.
/// </summary>
public interface IAttackable
{
    /// <summary>
    /// DNFTransform component of the attacking subject.
    /// </summary>
    public DNFTransform AttackerDNFTransform { set;  get; }

    /// <summary>
    /// Hitbox controller component for representing the attack range.
    /// </summary>
    public HitboxController AttackerHitboxController{ set; get; }
    
    /// <summary>
    /// Check if the targets' hitbox is within the attack range.
    /// </summary>
    /// <param name="targets">The list of objects that can be hit</param>
    /// <returns>True if there is at least one target that has been hit</returns>
    public bool CalculateOnHit(List<IDamagable> targets);
}

/// <summary>
/// Interface representing an object capable of receiving damage.
/// </summary>
public interface IDamagable
{
    /// <summary>
    /// DNFTransform component of the defending object.
    /// </summary>
    public DNFTransform DefenderDNFTransform { set;  get; }

    /// <summary>
    /// Hitbox controller component used to check whether the object has been hit.
    /// </summary>
    public HitboxController DefenderHitboxController { set; get; }

    /// <summary>
    /// The event method called when the object is hit.
    /// </summary>
    /// <param name="attacker">The DNFTransform component of the attacker</param>
    /// <param name="damages">The list of damamges</param>
    /// <param name="knockBackPower">The power value for the knock back effect</param>
    /// <param name="knockBackDirection">The normalized direction value for the knock back effect</param>
    public void OnDamage(DNFTransform attacker, List<int> damages, float knockBackPower, Vector3 knockBackDirection);
}

[Serializable]
public class Hitbox
{
    #region Variables

    private DNFTransform dnfTransform = null;

    [SerializeField] private EHitboxType hitboxType = EHitboxType.BOX;
    [SerializeField] private Vector3 size = Vector3.zero;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Vector3 pivot = Vector3.zero;

    /// <summary>
    /// The maximum values for each component of the hitbox.
    /// If the hitbox shape is a circle, the z-value has no effect.
    /// </summary>
    private Vector3 maxHitboxPos = Vector3.zero;

    /// <summary>
    /// The minimum values for each component of the hitbox.
    /// If the hitbox shape is a circle, the z-value has no effect.
    /// </summary>
    private Vector3 minHitboxPos = Vector3.zero;

    #endregion Variables

#if UNITY_EDITOR

    #region Properties

    /// <summary>
    /// DNF transform component used to calculate the hitbox.
    /// </summary>
    public DNFTransform DNFTransform => dnfTransform;

    /// <summary>
    /// The shape of the hitbox on the XZ plane.
    /// The shape of the hitbox is either box-shaped or circle-shaped.
    /// </summary>
    public EHitboxType HitboxType
    {
        set 
        {
            hitboxType = value; 
        }
        get => hitboxType;
    }

    /// <summary>
    /// The component determining the size of the hitbox.
    /// Each component of the size is a non-negative value.
    /// </summary>
    public Vector3 Size
    {
        set
        {
            Vector3 input = value;

            if (input.x < 0f) input.x = 0f;
            if (input.y < 0f) input.y = 0f;
            if (input.z < 0f) input.z = 0f;

            size = input;
        }
        get => size;
    }

    /// <summary>
    /// The component determining how far the hitbox is from the object's position.
    /// </summary>
    public Vector3 Offset
    {
        set { offset = value; }
        get => offset;
    }

    /// <summary>
    /// The component determining the ratio of the center point of the hitbox.
    /// Each component of the pivot will be clamped between 0 and 1.
    /// </summary>
    public Vector3 Pivot
    {
        set
        {
            Vector3 input = value;

            input.x = Mathf.Clamp01(input.x);
            input.y = Mathf.Clamp01(input.y);
            input.z = Mathf.Clamp01(input.z);

            pivot = input;
        }
        get => pivot;
    }

    #endregion Properties

#endif

    #region Methods

    /// <summary>
    /// Initialize the hitbox.
    /// </summary>
    public void Init(DNFTransform dnfTransform)
    {
        this.dnfTransform = dnfTransform;
    }

    /// <summary>
    /// Calculate the hitbox by using the value according to DNF transform.
    /// </summary>
    public void CalculateHitbox()
    {
        Vector3 position = dnfTransform.Position;
        float localScale = dnfTransform.LocalScale;

        Vector3 minHitboxPos = Offset - localScale * new Vector3(Size.x * Pivot.x, Size.y * Pivot.y, Size.z * Pivot.z);
        Vector3 maxHitboxPos = Offset + localScale * new Vector3(Size.x * (1f - Pivot.x), Size.y * (1f - Pivot.y), Size.z * (1f - Pivot.z));

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
    /// Call the appropriate collision detection function based on the shapes of the two hitboxes.
    /// </summary>
    /// <returns>true if a collision occurs</returns>
    public bool CheckCollision(Hitbox other)
    {
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

    /// <summary>
    /// Check for collisions between box-shaped hitbox objects in the XZ coordinate.
    /// Compare the maximum and minimum values along each axis.
    /// </summary>
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
    /// <returns>true if a collision occurs in Y coordinate</returns>
    private bool CheckYCollision(Hitbox hitbox1, Hitbox hitbox2)
    {
        if (!hitbox1.dnfTransform.HasYObj || !hitbox2.dnfTransform.HasYObj) return true;

        if (hitbox1.maxHitboxPos.y < hitbox2.minHitboxPos.y || hitbox1.minHitboxPos.y > hitbox2.maxHitboxPos.y) return false;

        return true;
    }

    #region Helper

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

public class HitboxController : MonoBehaviour
{
    #region Variables

    [SerializeField, HideInInspector] private Hitbox[] hitboxes = new Hitbox[0];
    private Hitbox activeHitbox = null;

    #endregion Variables

    #region Properties

    /// <summary>
    /// Determine whether the current hitbox is activated.
    /// Return true if the hitbox currently activated, and false otherwise.
    /// </summary>
    public bool IsHitboxActivated => activeHitbox != null;

#if UNITY_EDITOR

    /// <summary>
    /// The array of the hitboxes for editing and debugging.
    /// </summary>
    public Hitbox[] Hitboxes
    {
        set 
        {
            hitboxes = value; 
        }
        get => hitboxes;
    }

    /// <summary>
    /// The currently active hitbox component.
    /// </summary>
    public Hitbox ActiveHitbox => activeHitbox;

#endif

    #endregion Properties

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

        foreach (Hitbox hitbox in hitboxes)
        {
            hitbox.Init(dnfTransform);
        }
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

        activeHitbox = hitboxes[index];
        activeHitbox.CalculateHitbox();
    }

    /// <summary>
    /// Disable the active hitbox.
    /// </summary>
    public void DisableHitbox()
    {
        if (!IsHitboxActivated) return;

        activeHitbox = null;
    }

    /// <summary>
    /// Calculate the active hitbox range.
    /// </summary>
    public void CalculateHitbox()
    {
        if (!IsHitboxActivated) return;

        activeHitbox.CalculateHitbox();
    }

    /// <summary>
    /// Check whether the collision occur.
    /// </summary>
    /// <param name="other">The other hitbox controller for collision checks</param>
    /// <returns>True if both controller's active hitbox is not null and a collision has occurred</returns>
    public bool CheckCollision(HitboxController other)
    {
        return activeHitbox != null 
            && other.activeHitbox != null
            && activeHitbox.CheckCollision(other.activeHitbox);
    }

    #endregion Methods
}
