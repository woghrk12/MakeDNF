using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Variables

    private Camera mainCamera = null;

    private Transform cameraTransform = null;
    private Transform targetTransform = null;

    /// <summary>
    /// The offset between the camera position and the target position
    /// </summary>
    [SerializeField] private Vector2 offset = Vector2.zero;

    /// <summary>
    /// The power value for shaking the camera.
    /// </summary>
    private float shakePower = 0f;

    private Vector3 cameraTopRight = Vector3.zero;
    private Vector3 cameraBottomLeft = Vector3.zero;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
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
    /// Set the transform component as the target which the camera focus on. 
    /// </summary>
    /// <param name="transform">The transform component of the target</param>
    public void SetTarget(Transform transform)
    {
        targetTransform = transform;
    }

    public void SetCameraRange(Vector3 cameraTopRight, Vector3 cameraBottomLeft)
    {
        this.cameraTopRight = cameraTopRight;
        this.cameraBottomLeft = cameraBottomLeft;
    }

    /// <summary>
    /// Shake the camera by the given amount of force.
    /// The greater value between the current force and the given force is applied.
    /// </summary>
    /// <param name="shakePower">The value determining how forcefully the camera shakes</param>
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
    /// Calculate the appropriate position for the camera to follow the target 
    /// based on the target's position and the result of camera shaking.
    /// It adjusts the camera's position accordingly and outputs the final position.
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

        float cameraSize = mainCamera.orthographicSize;
        float ratio = (float)mainCamera.pixelWidth / mainCamera.pixelHeight;

        cameraPos.x = Mathf.Clamp(cameraPos.x, cameraBottomLeft.x + cameraSize * ratio, cameraTopRight.x - cameraSize * ratio);
        cameraPos.y = Mathf.Clamp(cameraPos.y, cameraBottomLeft.y + cameraSize, cameraTopRight.y - cameraSize);
        
        cameraTransform.position = cameraPos;
    }

    #endregion Helper

    #endregion Methods
}
