using UnityEngine;

/// <summary>
/// The generic behaviour class when the character is hit.
/// </summary>
public class HitBehaviour : GenericBehaviour<Character>
{
    /// <summary>
    /// The enum for player hit state.
    /// <para>
    /// STIFFNESS : The state where the character pauses briefly upon being hit. 
    /// DURATION : The state for the hit animation and knock back effect.
    /// </para>
    /// </summary>
    private enum EState { NONE = -1, STIFFNESS, DURATION }

    #region Variables

    [Header("Variables for hit stiffness effect")]
    private EState state = EState.NONE;
    private float hitStiffnessTime = 0.08f;
    private float hitDuration = 0f;
    private float timer = 0f;

    [Header("Animation key hash")]
    private int normalSpeedHash = 0;
    private int doHitHash = 0;
    private int isHitHash = 0;

    #endregion Variables

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        controller = GetComponent<Character>();

        normalSpeedHash = Animator.StringToHash(AnimatorKey.Character.NORMAL_SPEED);
        doHitHash = Animator.StringToHash(AnimatorKey.Character.DO_HIT);
        isHitHash = Animator.StringToHash(AnimatorKey.Character.IS_HIT);
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Transition the character into a hit state.
    /// During the hit duration, the character cannot move or jump, and only skills available in the hit state can be used.
    /// The knockBackVector parameter includes the knockback force, so that knockBackVector will be Vector3.zero if there is no knockback effect.
    /// </summary>
    /// <param name="hitDuration">The time it takes for the character to return to the Idle state after being hit</param>
    /// <param name="knockBackVector">The direction for knock back effect</param>
    public void Hit(float hitDuration, Vector3 knockBackVector)
    {
        this.hitDuration = hitDuration;

        controller.DNFRigidbody.Velocity = Vector3.zero;
        controller.DNFRigidbody.AddForce(knockBackVector);

        controller.SetBehaviour(behaviourCode);
    }

    public override void OnStart()
    {
        timer = 0f;

        controller.CanMove = false;
        controller.CanJump = false;

        controller.Animator.SetTrigger(doHitHash);
        controller.Animator.SetBool(isHitHash, true);
        controller.Animator.SetFloat(normalSpeedHash, 0f);

        controller.DNFRigidbody.enabled = false;

        state = EState.STIFFNESS;
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case EState.STIFFNESS:
                if (timer < hitStiffnessTime) return;

                timer = 0f;

                controller.DNFRigidbody.enabled = true;
                controller.Animator.SetFloat(normalSpeedHash, 1f);

                state = EState.DURATION;

                break;

            case EState.DURATION:
                if (timer < hitDuration) return;

                OnComplete();

                break;
        } 
    }

    public override void OnComplete()
    {
        controller.CanMove = true;
        controller.CanJump = true;

        controller.Animator.SetBool(isHitHash, false);
        controller.Animator.SetFloat(normalSpeedHash, 1f);

        controller.SetBehaviour(BehaviourCodeList.IDLE_BEHAVIOUR_CODE);
    }

    public override void OnCancel()
    {
        controller.DNFRigidbody.enabled = true;

        controller.Animator.SetBool(isHitHash, false);
        controller.Animator.SetFloat(normalSpeedHash, 1f);
    }

    #endregion Methods
}
