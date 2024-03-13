using UnityEngine;

/// <summary>
/// The behaviour class when the character is hit.
/// </summary>
public class HitBehaviour : CharacterBehaviour
{
    #region Variables

    [Header("Variables for hit stiffness effect")]
    private float hitDuration = 0f;
    private float timer = 0f;

    [Header("Animation key hash")]
    private int doHitHash = 0;
    private int isHitHash = 0;

    #endregion Variables

    #region Properties

    /// <summary>
    /// Hash code value for the hit behaviour.
    /// </summary>
    public override int BehaviourCode => typeof(HitBehaviour).GetHashCode();

    #endregion Properties

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

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

        character.DNFRigidbody.Velocity = Vector3.zero;
        character.DNFRigidbody.AddForce(knockBackVector);

        character.SetBehaviour(BehaviourCode);
    }

    public override void OnStart()
    {
        timer = 0f;

        character.CanMove = false;
        character.CanJump = false;

        character.Animator.SetTrigger(doHitHash);
        character.Animator.SetBool(isHitHash, true);
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        
        if (timer < hitDuration) return;

        OnComplete();
    }

    public override void OnComplete()
    {
        character.CanMove = true;
        character.CanJump = true;

        character.Animator.SetBool(isHitHash, false);

        character.SetBehaviour(BehaviourCodeList.IDLE_BEHAVIOUR_CODE);
    }

    public override void OnCancel()
    {
        character.DNFRigidbody.enabled = true;

        character.Animator.SetBool(isHitHash, false);
    }

    #endregion Methods
}
