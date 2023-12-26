using System.Collections;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    #region Variables

    private Animator characterAnimator = null;

    private DNFTransform characterTransform = null;
    private DNFRigidbody characterRigidbody = null;
    
    [Header("Scale variables for character movement")]
    [SerializeField] private float xMoveSpeed = 0f;
    [SerializeField] private float zMoveSpeed = 0f;
    [SerializeField] private float jumpPower = 0f;

    [Header("Hash for animation key")]
    private int isWalkHash = 0;
    private int doJumpHash = 0;
    private int isJumpHash = 0;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Initialize the variables and components for character movement.
    /// </summary>
    public void Init(Character character)
    {
        characterAnimator = character.Animator;

        characterTransform = character.DNFTransform;
        characterRigidbody = character.DNFRigidbody;

        isWalkHash = Animator.StringToHash(AnimatorKey.Character.IS_WALK);
        doJumpHash = Animator.StringToHash(AnimatorKey.Character.DO_JUMP);
        isJumpHash = Animator.StringToHash(AnimatorKey.Character.IS_JUMP);
    }

    /// <summary>
    /// Move the character in the given direction in the DNF coordinate system.
    /// Perform character movement by modifying the x-value and z-value of the position of Pos Transform. 
    /// </summary>
    /// <param name="moveDir">The direction to move character</param>
    public void Move(Vector3 moveDir)
    {
        characterAnimator.SetBool(isWalkHash, moveDir != Vector3.zero);

        moveDir.x *= xMoveSpeed * Time.fixedDeltaTime;
        moveDir.z *= zMoveSpeed * Time.fixedDeltaTime;

        if (moveDir.x != 0f)
        {
            characterTransform.IsLeft = moveDir.x < 0f;
        }

        characterRigidbody.MoveDirection(moveDir);
    }

    /// <summary>
    /// Jump the character.
    /// Perform character jump by modifying the y-value of the local position of Y Pos Transform.
    /// </summary>
    public void Jump()
    {
        characterAnimator.SetTrigger(doJumpHash);
        characterAnimator.SetBool(isJumpHash, true);

        characterRigidbody.AddForce(new Vector3(0f, jumpPower, 0f));

        StartCoroutine(CheckLanding());
    }

    /// <summary>
    /// Check if the character has landed on the ground.
    /// </summary>
    private IEnumerator CheckLanding()
    {
        while (characterRigidbody.Velocity.y > 0f)
        {
            yield return Utilities.WaitForFixedUpdate;
        }

        while (!characterRigidbody.IsGround)
        {
            yield return Utilities.WaitForFixedUpdate;
        }

        characterAnimator.SetBool(isJumpHash, false);
    }

    #endregion Methods
}
