using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DNFRigidbody))]
public class CharacterMove : MonoBehaviour
{
    #region Variables

    private Animator animator = null;
    private Transform characterTransform = null;

    private DNFRigidbody dnfRigidbody = null;
    
    [SerializeField] private float xMoveSpeed = 0f;
    [SerializeField] private float zMoveSpeed = 0f;
    [SerializeField] private float jumpPower = 0f;

    [Header("Hash for animation key")]
    private int isWalkHash = 0;
    private int doJumpHash = 0;
    private int isJumpHash = 0;

    private bool isLeft = false;

    #endregion Variables

    #region Properties

    public bool IsLeft
    {
        set
        {
            isLeft = value;
            characterTransform.localScale = new Vector3(isLeft ? -1f : 1f, 1f, 1f);
        }
        get => isLeft;
    }

    #endregion Properties

    #region Methods

    public void Init(DNFRigidbody dnfRigidbody)
    {
        animator = GetComponent<Animator>();
        characterTransform = transform;

        isWalkHash = Animator.StringToHash(AnimatorKey.Character.IS_WALK);
        doJumpHash = Animator.StringToHash(AnimatorKey.Character.DO_JUMP);
        isJumpHash = Animator.StringToHash(AnimatorKey.Character.IS_JUMP);

        this.dnfRigidbody = dnfRigidbody;
    }

    #region Move

    public void Move(Vector3 moveDir)
    {
        animator.SetBool(isWalkHash, moveDir != Vector3.zero);

        moveDir.x *= xMoveSpeed * Time.fixedDeltaTime;
        moveDir.z *= zMoveSpeed * Time.fixedDeltaTime;

        if (moveDir.x != 0f)
        {
            IsLeft = moveDir.x < 0f;
        }

        dnfRigidbody.MoveDirection(moveDir);
    }

    #endregion Move

    #region Jump

    public void Jump()
    {
        if (!dnfRigidbody.IsGround) return;

        animator.SetTrigger(doJumpHash);
        animator.SetBool(isJumpHash, true);

        dnfRigidbody.AddForce(new Vector3(0f, jumpPower, 0f));

        StartCoroutine(CheckLanding());
    }

    private IEnumerator CheckLanding()
    {
        while (dnfRigidbody.Velocity.y > 0f)
        {
            yield return Utilities.WaitForFixedUpdate;
        }

        while (!dnfRigidbody.IsGround)
        {
            yield return Utilities.WaitForFixedUpdate;
        }

        animator.SetBool(isJumpHash, false);
    }

    #endregion Jump

    #endregion Methods
}
