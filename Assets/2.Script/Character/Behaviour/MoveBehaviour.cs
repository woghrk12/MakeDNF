using UnityEngine;

/// <summary>
/// The generic behaviour class when the character moves.
/// MoveBehaviour is not overriden by other actions, and is only controlled through a flag variables.
/// </summary>
public class MoveBehaviour : GenericBehaviour<Character>
{
    #region Variables

    [Header("Scale variables for character movement")]
    [SerializeField] private float xMoveSpeed = 0f;
    [SerializeField] private float zMoveSpeed = 0f;

    [Header("Hash for animation key")]
    private int isWalkHash = 0;

    #endregion Variables

    #region Properties

    /// <summary>
    /// A flag variable indicating whether the controller is allowed to move.
    /// </summary>
    public bool CanMove { set; get; }

    /// <summary>
    /// A flag variable indicating whether the controller is allowed to change the look direction.
    /// </summary>
    public bool CanLookBack { set; get; }

    #endregion Properties

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        controller = GetComponent<Character>();

        isWalkHash = Animator.StringToHash(AnimatorKey.Character.IS_WALK);
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Move the character in the given direction in the DNF coordinate system.
    /// Perform character movement by modifying the x-value and z-value of the position of Pos Transform. 
    /// </summary>
    /// <param name="moveDir">The direction to move character</param>
    public void Move(Vector3 moveDir)
    {
        controller.Animator.SetBool(isWalkHash, moveDir != Vector3.zero);

        moveDir.x *= xMoveSpeed * Time.fixedDeltaTime;
        moveDir.z *= zMoveSpeed * Time.fixedDeltaTime;

        if (CanLookBack && moveDir.x != 0f)
        {
            controller.DNFTransform.IsLeft = moveDir.x < 0f;
        }

        controller.DNFRigidbody.MoveDirection(moveDir);
    }

    #endregion Methods
}
