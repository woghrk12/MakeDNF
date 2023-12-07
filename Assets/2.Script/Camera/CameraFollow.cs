using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables

    private Transform cameraTransform = null;
    private Transform target = null;

    [SerializeField] private Vector3 offset = Vector3.zero;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        cameraTransform = transform;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Follow(target);
    }

    #endregion Unity Events

    #region Methods

    public void SetTarget(Transform transform)
    {
        target = transform;
    }

    private void Follow(Transform target)
    {
        Vector3 cameraPos = target.position + offset;
        cameraTransform.position = new Vector3(cameraPos.x, cameraPos.y, cameraTransform.position.z);
    }

    #endregion Methods
}
