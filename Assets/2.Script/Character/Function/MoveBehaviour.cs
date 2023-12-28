using System.Collections;
using UnityEngine;

public class MoveBehaviour : GenericBehaviour
{
    #region Variables

    [Header("Scale variables for character movement")]
    [SerializeField] private float xMoveSpeed = 0f;
    [SerializeField] private float zMoveSpeed = 0f;
    [SerializeField] private float jumpPower = 0f;

    [Header("Hash for animation key")]
    private int isWalkHash = 0;
    private int doJumpHash = 0;
    private int isJumpHash = 0;

    #endregion Variables

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        isWalkHash = Animator.StringToHash(AnimatorKey.Character.IS_WALK);
        doJumpHash = Animator.StringToHash(AnimatorKey.Character.DO_JUMP);
        isJumpHash = Animator.StringToHash(AnimatorKey.Character.IS_JUMP);
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

        if (moveDir.x != 0f)
        {
            controller.DNFTransform.IsLeft = moveDir.x < 0f;
        }

        controller.DNFRigidbody.MoveDirection(moveDir);
    }

    /// <summary>
    /// Jump the character.
    /// Perform character jump by modifying the y-value of the local position of Y Pos Transform.
    /// </summary>
    public void Jump()
    {
        controller.Animator.SetTrigger(doJumpHash);
        controller.Animator.SetBool(isJumpHash, true);

        controller.DNFRigidbody.AddForce(new Vector3(0f, jumpPower, 0f));

        StartCoroutine(CheckLanding());
    }

    /// <summary>
    /// Check if the character has landed on the ground.
    /// </summary>
    private IEnumerator CheckLanding()
    {
        while (controller.DNFRigidbody.Velocity.y > 0f)
        {
            yield return Utilities.WaitForFixedUpdate;
        }

        while (!controller.DNFRigidbody.IsGround)
        {
            yield return Utilities.WaitForFixedUpdate;
        }

        controller.Animator.SetBool(isJumpHash, false);
    }

    #endregion Methods
}
