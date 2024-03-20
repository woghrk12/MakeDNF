using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Variables

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
    /// Set the transform component as the target which the camera focus on. 
    /// </summary>
    /// <param name="transform">The transform component of the target</param>
    public void SetTarget(Transform transform)
    {
        targetTransform = transform;
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

        cameraTransform.position = cameraPos;
    }

    #endregion Helper

    #endregion Methods
}
