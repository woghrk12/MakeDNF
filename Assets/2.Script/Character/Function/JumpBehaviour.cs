using UnityEngine;

public class JumpBehaviour : GenericBehaviour
{
    #region Variables

    [Header("Scale variables for character jump")]
    [SerializeField] private float jumpPower = 0f;


    [Header("State variables")]
    private bool isPreDelay = false;
    private bool isJumpUp = false;
    private bool isJumpDown = false;
    private bool isPostDelay = false;

    [Header("Time variables for delay")]
    private float timer = 0f;
    private float preDelay = 0f;
    private float postDelay = 0f;

    [Header("Hash for animation key")]
    private int doJumpHash = 0;
    private int isJumpUpHash = 0;
    private int isJumpDownHash = 0;

    #endregion Variables

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        preDelay = Time.deltaTime * 3f * 5f;
        postDelay = Time.deltaTime * 4f * 5f;

        doJumpHash = Animator.StringToHash(AnimatorKey.Character.DO_JUMP);
        isJumpUpHash = Animator.StringToHash(AnimatorKey.Character.IS_JUMP_UP);
        isJumpDownHash = Animator.StringToHash(AnimatorKey.Character.IS_JUMP_DOWN);
    }

    #endregion Unity Events

    #region Methods
    
    /// <summary>
    /// Jump the character.
    /// Perform character jump by modifying the y-value of the local position of Y Pos Transform.
    /// </summary>
    public void Jump()
    {
        controller.SetBehaviour(behaviourCode);
    }

    #region Override

    public override void OnStart()
    {
        // Initialize the variables
        timer = 0f;
        isPreDelay = true;
        isJumpUp = false;
        isJumpDown = false;
        isPostDelay = false;

        controller.CanMove = false;

        controller.Animator.SetTrigger(doJumpHash);
    }

    public override void OnFixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (isPreDelay)
        {
            if (timer < preDelay) return;

            isJumpUp = true;
            
            controller.Animator.SetBool(isJumpUpHash, true);
            
            controller.DNFRigidbody.AddForce(new Vector3(0f, jumpPower, 0f));

            controller.CanMove = true;

            isPreDelay = false;
            timer = 0f;
        }
        else if (isPostDelay)
        {
            if (timer < postDelay) return;
            
            OnComplete();
        }
        else
        {
            if (isJumpUp)
            {
                if (controller.DNFRigidbody.Velocity.y > 0f) return;

                controller.Animator.SetBool(isJumpUpHash, false);
                controller.Animator.SetBool(isJumpDownHash, true);

                isJumpUp = false;
                isJumpDown = true;
            }
            else if (isJumpDown)
            {
                if (!controller.DNFRigidbody.IsGround) return;

                controller.Animator.SetBool(isJumpDownHash, false);

                controller.CanMove = false;

                isPostDelay = true;
                timer = 0f;
            }
        }
    }

    public override void OnComplete()
    {
        timer = 0f;
        isPreDelay = false;
        isJumpUp = false;
        isJumpDown = false;
        isPostDelay = false;

        controller.Animator.ResetTrigger(doJumpHash);
        controller.Animator.SetBool(isJumpUpHash, false);
        controller.Animator.SetBool(isJumpDownHash, false);

        controller.CanMove = true;

        controller.SetBehaviour(BehaviourCodeList.IDLE_BEHAVIOUR_CODE);
    }

    #endregion Override

    #endregion Methods
}
