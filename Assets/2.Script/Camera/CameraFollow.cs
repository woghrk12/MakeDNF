using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables

    private Transform cameraTransform = null;
    private Transform target = null;

    [SerializeField] private Vector2 offset = Vector2.zero;

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
        Vector3 targetPos = new Vector3(target.position.x + offset.x, target.position.y + offset.y, cameraTransform.position.z);
        cameraTransform.position = targetPos;
    }

    #endregion Methods
}
