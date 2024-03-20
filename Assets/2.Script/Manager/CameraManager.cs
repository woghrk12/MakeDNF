using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Variables

    private Transform cameraTransform = null;
    private Transform targetTransform = null;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private Vector2 offset = Vector2.zero;

    /// <summary>
    /// 
    /// </summary>
    private float shakePower = 0f;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        cameraTransform = transform;
    }

    private void LateUpdate()
    {
        if (targetTransform == null) return;
        
        if (shakePower > 0f)
        {
            shakePower -= Time.deltaTime;
        }

        HandleCameraPosition();
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transform"></param>
    public void SetTarget(Transform transform)
    {
        targetTransform = transform;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shakePower"></param>
    public void ShakeCamera(float shakePower)
    {
        float power = shakePower * 0.1f;

        if (this.shakePower < power)
        {
            this.shakePower = power;
        }
    }

    #region Helper

    /// <summary>
    /// 
    /// </summary>
    private void HandleCameraPosition()
    {
        Vector3 targetPos = targetTransform.position;
        Vector3 cameraPos = new Vector3(targetPos.x + offset.x, targetPos.y + offset.y, cameraTransform.position.z);

        if (shakePower > 0f)
        { 
            Vector3 shakeOffset = Random.insideUnitCircle * shakePower;
            cameraPos += shakeOffset;
        }

        cameraTransform.position = cameraPos;
    }

    #endregion Helper

    #endregion Methods
}
