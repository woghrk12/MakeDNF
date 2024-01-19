using UnityEngine;

/// <summary>
/// The SkillState class represents the various states of a skill within a Skill class.
/// </summary>
public abstract class SkillState
{
    #region Variables

    protected Character character = null;

    [Header("Animation key hash")]
    protected int stateHash = 0;
    protected int cancelHash = 0;

    protected float timer = 0f;

    protected bool isPreDelay = false;
    protected bool isPostDelay = false;

    protected float preDelay = 0f;
    protected float postDelay = 0f;

    #endregion Variables

    #region Constructor

    public SkillState(Character character, Skill stateController) 
    {
        this.character = character;

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