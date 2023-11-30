using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNFTransform : MonoBehaviour
{
    #region Variables

    [Header("Transform objects for Character Transform")]
    private Transform posTransform = null;
    private Transform yPosTransform = null;
    private Transform scaleTransform = null;

    #endregion Variables

    #region Properties

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

    public float Y
    {
        set
        {
            Vector3 pos = yPosTransform.position;
            pos.y = value;
            yPosTransform.position = pos;
        }
        get => yPosTransform.localPosition.y;
    }

    public float Z
    {
        set
        {
            Vector3 pos = posTransform.position;
            pos.y = value * GlobalDefine.ConvRate;
            posTransform.position = pos;
        }
        get => posTransform.position.y * GlobalDefine.InvConvRate;
    }

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

    public bool HasYObj => yPosTransform != null;

    public Vector3 LocalScale 
    {
        set 
        {
            scaleTransform.localScale = value; 
        } 
        get => scaleTransform.localScale;
    }

    #endregion Properties

    #region Unity Events

    private void Awake()
    {
        posTransform = GetComponent<Transform>();
        yPosTransform = posTransform.GetComponentInChildren<Transform>();
        scaleTransform = yPosTransform.GetComponentInChildren<Transform>();
    }

    #endregion Unity Events
}
