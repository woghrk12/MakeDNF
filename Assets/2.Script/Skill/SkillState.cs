using UnityEngine;

/// <summary>
/// The SkillState class represents the various states of a skill within a Skill class.
/// </summary>
public abstract class SkillState
{
    /// <summary>
    /// The phase of the skill state.
    /// <para>
    /// PREDELAY : Delay before hitbox activation or projectile creation.
    /// HITBOXACTIVE : Phase during hitbox activation.
    /// MOTIONINPROGRESS : Phase after hitbox deactivation or projectile creation.
    /// POSTDELAY : Delay after the skill motion animation ends.
    /// </para>
    /// </summary>
    protected enum EStatePhase { NONE = -1, PREDELAY, HITBOXACTIVE, MOTIONINPROGRESS, POSTDELAY }

    #region Variables

    protected Character character = null;

    [Header("Animation key hash")]
    protected int skillHash = 0;
    protected int continueHash = 0;
    protected int cancelHash = 0;

    [Header("Variables to control the skill phase")]
    protected EStatePhase phase = EStatePhase.NONE;
    protected float preDelay = 0f;
    protected float duration = 0f;
    protected float postDelay = 0f;

    #endregion Variables

    #region Constructor

    public SkillState(Character character, Skill stateController) 
    {
        this.character = character;

        continueHash = Animator.StringToHash(AnimatorKey.Character.IS_CONTINUE_ATTACK);
        cancelHash = Animator.StringToHash(AnimatorKey.Character.END_ATTACK);
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// The event method called when the skill state is activated.
    /// It serves as an entry point for any logics that need to occur at the beginning.
    /// </summary>
    public virtual void OnStart() { }

    /// <summary>
    /// The event method called every frame update.
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// The event method called every fixed frame update.
    /// </summary>
    public virtual void OnFixedUpdate() { }

    /// <summary>
    /// The event method called after every update method has been executed.
    /// </summary>
    public virtual void OnLateUpdate() { }

    /// <summary>
    /// The event method called when the skill state is completed.
    /// </summary>
    public virtual void OnComplete() { }

    /// <summary>
    /// The event method called when the skill is canceled by another skill.
    /// </summary>
    public virtual void OnCancel() { }

    #endregion Methods
}