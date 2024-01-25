using UnityEngine;

public class JumpBehaviour : GenericBehaviour
{
    private enum EPhase { NONE = -1, PREDELAY, JUMPUP, JUMPDOWN, POSTDELAY }

    #region Variables

    [Header("Scale variables for character jump")]
    [SerializeField] private float jumpPower = 0f;

    [Header("State variables")]
    private EPhase phase = EPhase.NONE;

    [Header("Hash for animation key")]
    private int isJumpHash = 0;
    private int isJumpUpHash = 0;
    private int isJumpDownHash = 0;

    #endregion Variables

    #region Properties

    /// <summary>
    /// A flag variable indicating whether the controller is allowed to jump.
    /// </summary>
    public bool CanJump { set; get; }

    /// <summary>
    /// A flag variable indicating whether the controller is in a jumping state.
    /// </summary>
    public bool IsJump => phase != EPhase.NONE;

    #endregion Properties

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        isJumpHash = Animator.StringToHash(AnimatorKey.Character.IS_JUMP);
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
        OnStart();
    }

    #region Override

    public override void OnStart()
    {
        phase = EPhase.PREDELAY;

        controller.CanMove = false;

        controller.Animator.SetBool(isJumpHash, true);
    }

    public override void OnFixedUpdate()
    {
        if (phase == EPhase.NONE) return;

        AnimatorStateInfo animatorStateInfo = controller.Animator.GetCurrentAnimatorStateInfo(0);

        switch (phase)
        {
            case EPhase.PREDELAY:
                if (!animatorStateInfo.IsName("JumpUpReady")) return;
                if (animatorStateInfo.normalizedTime < 1f) return;

                controller.Animator.SetBool(isJumpUpHash, true);
                
                controller.DNFRigidbody.AddForce(new Vector3(0f, jumpPower, 0f));

                controller.CanMove = true;

                phase = EPhase.JUMPUP;

                break;

            case EPhase.JUMPUP:
                if (controller.DNFRigidbody.Velocity.y > 0f) return;

                controller.Animator.SetBool(isJumpUpHash, false);
                controller.Animator.SetBool(isJumpDownHash, true);

                phase = EPhase.JUMPDOWN;

                break;

            case EPhase.JUMPDOWN:
                if (!controller.DNFRigidbody.IsGround) return;

                controller.Animator.SetBool(isJumpDownHash, false);

                controller.CanMove = false;

                phase = EPhase.POSTDELAY;

                break;

            case EPhase.POSTDELAY:
                if (!animatorStateInfo.IsName("JumpDownComplete")) return;
                if (animatorStateInfo.normalizedTime < 1f) return;

                OnComplete();

                break;
        }
    }

    public override void OnComplete()
    {
        phase = EPhase.NONE;

        controller.Animator.SetBool(isJumpHash, false);
        controller.Animator.SetBool(isJumpUpHash, false);
        controller.Animator.SetBool(isJumpDownHash, false);

        controller.CanMove = true;
    }

    #endregion Override

    #endregion Methods
}
