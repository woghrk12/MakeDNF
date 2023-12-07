using UnityEngine;

[RequireComponent(typeof(DNFTransform))]
public class DNFRigidbody : MonoBehaviour
{
    #region Variables

    private DNFTransform dnfTransform = null;

    /// <summary>
    /// The gravity applied in all DNFRigidbody components.
    /// </summary>
    private static Vector3 gravity = new Vector3(0f, -9.81f, 0f);

    /// <summary>
    /// Linear velocity of the DNFRigidbody in units per second.
    /// </summary>
    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// Coefficient of drag.
    /// </summary>
    [SerializeField] private float drag = 0.2f;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        // Calculate gravitational acceleration
        gravity *= Time.deltaTime;

        dnfTransform = GetComponent<DNFTransform>();
    }

    private void FixedUpdate()
    {
        // Calculate the velocity of XZ plane
        Vector3 xzVelocity = new Vector3(velocity.x, 0f, velocity.z);

        xzVelocity -= xzVelocity.normalized * drag;

        if (xzVelocity.sqrMagnitude < 0.1f)
        {
            xzVelocity = Vector3.zero;
        }

        velocity.x = xzVelocity.x;
        velocity.z = xzVelocity.z;

        // Calculate the velocity of Y axis
        velocity += gravity;

        if (velocity.y < 0f && dnfTransform.Y <= 0f)
        {
            velocity.y = 0f;
            dnfTransform.Y = 0f;
        }

        // Update position
        dnfTransform.Position += velocity * Time.deltaTime;
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Move the DNFRigidbody in the given direction.
    /// </summary>
    /// <param name="moveDir">The direction to move the DNFRigidbody object</param>
    public void MoveDirection(Vector3 moveDir)
    {
        dnfTransform.Position += moveDir;
    }

    /// <summary>
    /// Apply a force to a given force vector
    /// </summary>
    /// <param name="force">Components of the force in the DNFTransform</param>
    public void AddForce(Vector3 force)
    {
        velocity += force;
    }

    #endregion Methods
}
