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
    /// STOPMOTION : Phase for stiffness effect if any object has been hit by the skill.
    /// MOTIONINPROGRESS : Phase after hitbox deactivation or projectile creation.
    /// POSTDELAY : Delay after the skill motion animation ends.
    /// </para>
    /// </summary>
    protected enum EStatePhase { NONE = -1, PREDELAY, HITBOXACTIVE, STOPMOTION, MOTIONINPROGRESS, POSTDELAY }

    #region Variables

    protected Character character = null;

    [Header("Animation key hash")]
    protected int attackSpeedHash = 0;
    protected int skillHash = 0;
    protected int continueHash = 0;
    protected int cancelHash = 0;

    [Header("Variables to control the skill phase")]
    protected EStatePhase phase = EStatePhase.NONE;
    protected float preDelay = 0f;
    protected float duration = 0f;
    protected float postDelay = 0f;

    [Header("Variables for stiffness effect")]
    protected float attackSpeed = 0f;
    protected float stiffnessTimer = 0f;
    protected float stiffnessTime = 0.1f;

    #endregion Variables

    #region Constructor

    public SkillState(Character character, Skill stateController) 
    {
        this.character = character;

        attackSpeedHash = Animator.StringToHash(AnimatorKey.Character.ATTACK_SPEED);
        continueHash = Animator.StringToHash(AnimatorKey.Character.IS_CONTINUE_ATTACK);
        cancelHash = Animator.StringToHash(AnimatorKey.Character.END_ATTACK);
    }

    #endregion Constructor

    #region Methods

    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnComplete() { }
    public virtual void OnCancel() { }

    public virtual void OnSkillButtonPressed() { }
    public virtual void OnSkillButtonReleased() { }

    #endregion Methods
}