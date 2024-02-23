using UnityEngine;

/// <summary>
/// The behaviour class when the character jumps.
/// </summary>
public class JumpBehaviour : CharacterBehaviour
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
    /// Hash code value for the jump behaviour.
    /// </summary>
    public override int BehaviourCode => typeof(JumpBehaviour).GetHashCode();

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
        CanJump = false;

        phase = EPhase.PREDELAY;

        character.CanMove = false;

        character.Animator.SetBool(isJumpHash, true);
    }

    public override void OnFixedUpdate()
    {
        if (phase == EPhase.NONE) return;

        AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

        switch (phase)
        {
            case EPhase.PREDELAY:
                if (!animatorStateInfo.IsName("JumpUpReady")) return;
                if (animatorStateInfo.normalizedTime < 1f) return;

                character.Animator.SetBool(isJumpUpHash, true);

                character.DNFRigidbody.AddForce(new Vector3(0f, jumpPower, 0f));

                character.CanMove = true;

                phase = EPhase.JUMPUP;

                break;

            case EPhase.JUMPUP:
                if (character.DNFRigidbody.Velocity.y > 0f) return;

                character.Animator.SetBool(isJumpUpHash, false);
                character.Animator.SetBool(isJumpDownHash, true);

                phase = EPhase.JUMPDOWN;

                break;

            case EPhase.JUMPDOWN:
                if (!character.DNFRigidbody.IsGround) return;

                character.Animator.SetBool(isJumpDownHash, false);

                character.CanMove = false;

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

        character.Animator.SetBool(isJumpHash, false);
        character.Animator.SetBool(isJumpUpHash, false);
        character.Animator.SetBool(isJumpDownHash, false);

        character.CanMove = true;
        character.CanLookBack = true;

        CanJump = true;
    }

    #endregion Override

    #endregion Methods
}
