using UnityEngine;

public class DNFTransform : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// The cached Transform component of the target object.
    /// </summary>
    private Transform cachedTransform = null;

    [Header("Transform variables")]
    [SerializeField] private Vector3 position = Vector3.zero;
    [SerializeField] private float localScale = 1f;

    [Header("Flag variables")]
    [SerializeField] private bool isLeft = false;
    [SerializeField] private bool isBoundaryOverride = false;

    #endregion Variables

    #region Properties

    /// <summary>
    /// X component of the position in the DNF coordinate.
    /// It changes in proportion to the x-value of the position of the cachedTransform.
    /// </summary>
    public float X
    {
        set
        {
            Room curRoom = GameManager.Room;
            position.x = isBoundaryOverride ? value : Mathf.Clamp(value, curRoom.MinXZPos.x, curRoom.MaxXZPos.x);

            cachedTransform.position = ConvertPosToWorldCoord(position);
        }
        get => position.x;
    }

    /// <summary>
    /// Y component of the position in the DNF coordinate.
    /// It changes in proportion to the y-value of the position of the cahcedTransform.
    /// The value is always non-negative.
    /// </summary>
    public float Y
    {
        set
        {
            position.y = value >= 0f ? value : 0f;

            cachedTransform.position = ConvertPosToWorldCoord(position);
        }
        get => position.y;
    }

    /// <summary>
    /// Z component of the DNFTransform.
    /// It changes in proportion to the screen ratio for the y-value of the position of the cachedTransform.
    /// Refer to the GlobalDefine class. (ConvRate, InvConvRate)
    /// </summary>
    public float Z
    {
        set
        {
            Room curRoom = GameManager.Room;
            position.z = isBoundaryOverride ? value : Mathf.Clamp(value, curRoom.MinXZPos.z, curRoom.MaxXZPos.z);

            cachedTransform.position = ConvertPosToWorldCoord(position);
        }
        get => position.z;
    }

    /// <summary>
    /// The world position in the DNF coordinate.
    /// </summary>
    public Vector3 Position
    {
        set
        {
            X = value.x;
            Z = value.z;
            Y = value.y;
        }
        get => position;
    }

    /// <summary>
    /// The flag indicating whether the direction of the object is facing left. 
    /// Return true if the object is facing left.
    /// </summary>
    public bool IsLeft
    {
        set
        {
            isLeft = value;

            cachedTransform.localScale = new Vector3((isLeft ? -1f : 1f) * localScale, localScale, 1f); 
        }
        get => isLeft;
    }

    /// <summary>
    /// The flag indicating whether the object is allowed to override the map boundary restrictions.
    /// Return true if the object is allowed to move outside the map boundary.
    /// </summary>
    public bool IsBoundaryOverride
    {
        set
        {
            if (!value)
            {
                Room curRoom = GameManager.Room;

                position.x = Mathf.Clamp(position.x, curRoom.MinXZPos.x, curRoom.MaxXZPos.x);
                position.z = Mathf.Clamp(position.z, curRoom.MinXZPos.z, curRoom.MaxXZPos.z);

                cachedTransform.position = ConvertPosToWorldCoord(position);
            }

            isBoundaryOverride = value;
        }
        get => isBoundaryOverride;
    }

    /// <summary>
    /// Scale value of the object.
    /// Modify the x and y values of the localScale of the cachedTransform.
    /// The scale value is always greater than 0.
    /// </summary>
    public float LocalScale
    {
        set
        {
            if (value <= 0f)
            {
                throw new System.Exception($"Local scale value cannot be less than 0.\nInput scale value : {value}");
            }

            localScale = value;

            cachedTransform.localScale = new Vector3((IsLeft ? -1f : 1f) * localScale, localScale, 1f);
        }
        get => localScale;
    }

    #endregion Properties

    #region Unity Events

    private void Awake()
    {
        cachedTransform = GetComponent<Transform>();
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Convert the position in the DNF coordinate into the position in the world coordinate.
    /// </summary>
    /// <param name="dnfPosition">The position value in the DNF coordinate to be converted into the world coordinate</param>
    /// <returns>The position value converted into world coordinate</returns>
    private Vector3 ConvertPosToWorldCoord(Vector3 dnfPosition)
    {
        return new Vector3(dnfPosition.x, dnfPosition.y + dnfPosition.z * GlobalDefine.CONV_RATE, 0f);
    }

    #endregion Methods
}
