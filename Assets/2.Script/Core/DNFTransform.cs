using UnityEngine;

public class DNFTransform : MonoBehaviour
{
    #region Variables

    [Header("Transform objects for Character Transform")]
    private Transform posTransform = null;
    private Transform yPosTransform = null;
    private Transform scaleTransform = null;

    [Header("Flag variables")]
    private bool isLeft = false;
    private bool isBoundaryOverride = false;

    #endregion Variables

    #region Properties

    /// <summary>
    /// X component of the DNFTransform.
    /// It changes in proportion to the x-value of the position in the posTransform.
    /// </summary>
    public float X
    {
        set
        {
            Vector3 pos = posTransform.position;

            Room curRoom = GameManager.Room;
            pos.x = isBoundaryOverride ? value : Mathf.Clamp(value, curRoom.MinXZPos.x, curRoom.MaxXZPos.x);

            posTransform.position = pos;
        }
        get => posTransform.position.x;
    }

    /// <summary>
    /// Y component of the DNFTransform.
    /// It changes in proportion to the y-value of the local position in the yPosTransform.
    /// </summary>
    public float Y
    {
        set
        {
            if (!HasYObj) return;

            Vector3 pos = yPosTransform.localPosition;
            pos.y = value;
            yPosTransform.localPosition = pos;
        }
        get => HasYObj ? yPosTransform.localPosition.y : 0f;
    }

    /// <summary>
    /// Z component of the DNFTransform.
    /// It changes in proportion to the screen ratio for the y-value of the position in the posTransform.
    /// Refer to the GlobalDefine class. (ConvRate, InvConvRate)
    /// </summary>
    public float Z
    {
        set
        {
            Vector3 pos = posTransform.position;

            Room curRoom = GameManager.Room;
            pos.y = isBoundaryOverride 
                ? value * GlobalDefine.CONV_RATE 
                : Mathf.Clamp(value * GlobalDefine.CONV_RATE, curRoom.MinXZPos.z, curRoom.MaxXZPos.z);
            
            posTransform.position = pos;
        }
        get => posTransform.position.y * GlobalDefine.INV_CONV_RATE;
    }

    /// <summary>
    /// The world position of DNFTransform.
    /// If yPosTransform do not exists, the y-value of DNFTransform will be fixed at 0.
    /// </summary>
    public Vector3 Position
    {
        set
        {
            X = value.x;
            Z = value.z;

            if (HasYObj)
            {
                Y = value.y;
            }
        }
        get => new Vector3(X, HasYObj ? Y : 0f, Z);
    }

    /// <summary>
    /// Return whether yPosTransform exists or not.
    /// </summary>
    public bool HasYObj => yPosTransform != null;

    /// <summary>
    /// Return whether scaleTransform exists or not.
    /// </summary>
    public bool HasScaleObj => scaleTransform != null;

    /// <summary>
    /// The flag controls whether the direction of the object is facing left. 
    /// Return true if the object is facing left.
    /// </summary>
    public bool IsLeft
    {
        set
        {
            isLeft = value;
            posTransform.localScale = new Vector3(isLeft ? -1f : 1f, 1f, 1f);
        }
        get => isLeft;
    }

    /// <summary>
    /// The flag controls whether the object is allowed to override the map boundary restrictions.
    /// Return true if the object is allowed to move outside the map boundary.
    /// </summary>
    public bool IsBoundaryOverride
    {
        set
        {
            if (!value)
            {
                Vector3 pos = posTransform.position;

                Room curRoom = GameManager.Room;

                pos.x = Mathf.Clamp(pos.x, curRoom.MinXZPos.x, curRoom.MaxXZPos.x);
                pos.y = Mathf.Clamp(pos.y, curRoom.MinXZPos.z, curRoom.MaxXZPos.z);

                posTransform.position = pos;
            }

            isBoundaryOverride = value;
        }
        get => isBoundaryOverride;
    }

    /// <summary>
    /// Scale value of the object.
    /// Modify the x and y values of the localScale in the scaleTransform.
    /// </summary>
    public float LocalScale
    {
        set
        {
            if (!HasScaleObj) return;

            scaleTransform.localScale = new Vector3(value, value, 1f);
        }
        get => HasScaleObj ? scaleTransform.localScale.x : 1f;
    }

    #endregion Properties

    #region Unity Events

    private void Awake()
    {
        posTransform = transform;
        yPosTransform = posTransform.childCount > 0 ? posTransform.GetChild(0) : null;
        scaleTransform = yPosTransform != null && yPosTransform.childCount > 0 ? yPosTransform.GetChild(0) : null;
    }

    #endregion Unity Events
}
