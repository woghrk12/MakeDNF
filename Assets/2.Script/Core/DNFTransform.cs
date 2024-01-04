using UnityEngine;

public class DNFTransform : MonoBehaviour
{
    #region Variables

    [Header("Transform objects for Character Transform")]
    private Transform posTransform = null;
    private Transform yPosTransform = null;
    private Transform scaleTransform = null;

    private bool isLeft = false;

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
            pos.x = value;
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
            pos.y = value * GlobalDefine.CONV_RATE;
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
    /// The direction the object is facing. Return true if the object is facing left.
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

        if (posTransform.childCount > 0)
        { 
            yPosTransform = posTransform.GetChild(0);
        }

        if (yPosTransform != null)
        { 
            scaleTransform = yPosTransform.GetChild(0);
        }
    }

    #endregion Unity Events
}
