using UnityEngine;

public class Character : BehaviourController
{
    #region Variables

    [Header("Character behaviours")]
    private JumpBehaviour jumpBehaviour = null;
    private AttackBehaviour attackBehaviour = null;

    #endregion Variables

    #region Properties

    /// <summary>
    /// A flag variable indicating whether the character is allowed to move.
    /// </summary>
    public virtual bool CanMove
    {
        set { moveBehaviour.CanMove = value; }
        get => moveBehaviour.CanMove;
    }

    /// <summary>
    /// A flag variable indicating whether the character is allowed to change the looking direction.
    /// </summary>
    public virtual bool CanLookBack 
    {
        set { moveBehaviour.CanLookBack = value; }
        get => moveBehaviour.CanLookBack; 
    }

    /// <summary>
    /// A flag variable indicating whether the character is allowed to jump.
    /// </summary>
    public virtual bool CanJump
    {
        set { jumpBehaviour.CanJump = value; }
        get => jumpBehaviour.CanJump && dnfRigidbody.IsGround;
    }

    /// <summary>
    /// A flag variable indicating whether the character is in a jumping state.
    /// </summary>
    public virtual bool IsJump => jumpBehaviour.IsJump;

    /// <summary>
    /// A flag variable indicating whether the character is allowed to attack.
    /// </summary>
    public virtual bool CanAttack
    { 
        set { attackBehaviour.CanAttack = value; }
        get => attackBehaviour.CanAttack;
    }

    #endregion Properties

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        jumpBehaviour = GetComponent<JumpBehaviour>();
        attackBehaviour = GetComponent<AttackBehaviour>();

        behaviourDictionary.Add(BehaviourCodeList.ATTACK_BEHAVIOUR_CODE, attackBehaviour);
    }

    private void Start()
    {
        // Debug
        Camera.main.GetComponent<CameraFollow>().SetTarget(transform);
        attackBehaviour.RegisterSkill(EKeyName.BASEATTACK, FindObjectOfType<BaseAttack_FireKnight>());

        GameManager.Input.AddMovementDelegate(OnJoystickMoved);

        GameManager.Input.AddButtonDownDelegate(EKeyName.JUMP, OnJumpButtonPressed);

        GameManager.Input.AddButtonDownDelegate(EKeyName.BASEATTACK, OnAttackButtonPressed);
        GameManager.Input.AddButtonUpDelegate(EKeyName.BASEATTACK, OnAttackButtonReleased);

        GameManager.Input.AddButtonDownDelegate(EKeyName.SKILL1, () => OnSkillButtonPressed(EKeyName.SKILL1));
        GameManager.Input.AddButtonUpDelegate(EKeyName.SKILL1, () => OnSkillButtonReleased(EKeyName.SKILL1));
        GameManager.Input.AddButtonDownDelegate(EKeyName.SKILL2, () => OnSkillButtonPressed(EKeyName.SKILL2));
        GameManager.Input.AddButtonUpDelegate(EKeyName.SKILL2, () => OnSkillButtonReleased(EKeyName.SKILL2));
        GameManager.Input.AddButtonDownDelegate(EKeyName.SKILL3, () => OnSkillButtonPressed(EKeyName.SKILL3));
        GameManager.Input.AddButtonUpDelegate(EKeyName.SKILL3, () => OnSkillButtonReleased(EKeyName.SKILL3));
        GameManager.Input.AddButtonDownDelegate(EKeyName.SKILL4, () => OnSkillButtonPressed(EKeyName.SKILL4));
        GameManager.Input.AddButtonUpDelegate(EKeyName.SKILL4, () => OnSkillButtonReleased(EKeyName.SKILL4));
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        jumpBehaviour.OnFixedUpdate();
    }

    #endregion Unity Events

    #region Event Methods

    public void OnJoystickMoved(Vector3 direction)
    {
        if (!CanMove) return;

        moveBehaviour.Move(direction);
    }

    public void OnJumpButtonPressed()
    {
        if (!CanJump) return;

        jumpBehaviour.Jump();
    }

    public void OnAttackButtonPressed()
    {
        attackBehaviour.OnSkillButtonPressed(EKeyName.BASEATTACK);

        if (!CanAttack) return;
        if (!attackBehaviour.CheckCanAttack(EKeyName.BASEATTACK)) return;

        attackBehaviour.Attack(EKeyName.BASEATTACK);
    }

    public void OnAttackButtonReleased()
    {
        attackBehaviour.OnSkillButtonReleased(EKeyName.BASEATTACK);
    }

    public void OnSkillButtonPressed(EKeyName keyName)
    {
        attackBehaviour.OnSkillButtonPressed(keyName);

        if (!CanAttack) return;
        if (!attackBehaviour.CheckCanAttack(keyName)) return;

        attackBehaviour.Attack(keyName);
    }

    public void OnSkillButtonReleased(EKeyName keyName)
    {
        attackBehaviour.OnSkillButtonReleased(keyName);
    }

    #endregion Event Methods
}
