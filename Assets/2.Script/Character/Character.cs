using System.Collections.Generic;
using UnityEngine;

public class Character : BehaviourController, IDamagable
{
    #region Variables

    [Header("Variables for controller's behaviour")]
    private Dictionary<int, GenericBehaviour<Character>> behaviourDictionary = new();
    private GenericBehaviour<Character> curBehaviour = null;

    [Header("Unity components")]
    private Animator animator = null;

    [Header("DNF components")]
    private DNFTransform dnfTransform = null;
    private DNFRigidbody dnfRigidbody = null;

    [Header("Character behaviours")]
    private IdleBehaviour idleBehaviour = null;
    private MoveBehaviour moveBehaviour = null;
    private JumpBehaviour jumpBehaviour = null;
    private AttackBehaviour attackBehaviour = null;
    private HitBehaviour hitBehaviour = null;

    #endregion Variables

    #region Properties

    /// <summary>
    /// The animator component of the character.
    /// </summary>
    public Animator Animator => animator;

    /// <summary>
    /// The DNFTransform component of the character.
    /// </summary>
    public DNFTransform DNFTransform => dnfTransform;

    /// <summary>
    /// The DNFRigidbody component of the character.
    /// </summary>
    public DNFRigidbody DNFRigidbody => dnfRigidbody;

    /// <summary>
    /// The current behaviour code of the character.
    /// </summary>
    public int CurBehaviourCode => curBehaviour.BehaviourCode;

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

    #region IDamagable Implementation

    public DNFTransform DefenseDNFTransform { set; get; }

    public HitboxController DefenseHitboxController { set; get; }

    public void OnDamage(DNFTransform attacker, List<int> damages, float knockBackPower, Vector3 knockBackDirection)
    {
        curBehaviour.OnCancel();

        hitBehaviour.Hit(knockBackPower * 0.2f, knockBackDirection * knockBackPower);
    }

    #endregion IDamagable Implementation

    #region Unity Events

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();

        idleBehaviour = GetComponent<IdleBehaviour>();
        moveBehaviour = GetComponent<MoveBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        attackBehaviour = GetComponent<AttackBehaviour>();
        hitBehaviour = GetComponent<HitBehaviour>();

        behaviourDictionary.Add(BehaviourCodeList.IDLE_BEHAVIOUR_CODE, idleBehaviour);
        behaviourDictionary.Add(BehaviourCodeList.ATTACK_BEHAVIOUR_CODE, attackBehaviour);
        behaviourDictionary.Add(BehaviourCodeList.HIT_BEHAVIOUR_CODE, hitBehaviour);

        DefenseDNFTransform = dnfTransform;
        DefenseHitboxController = GetComponent<HitboxController>();
        DefenseHitboxController.Init(dnfTransform);

        curBehaviour = idleBehaviour;
    }

    protected virtual void Start()
    {
        // Debug
        Camera.main.GetComponent<CameraFollow>().SetTarget(transform);
        attackBehaviour.RegisterSkill(EKeyName.BASEATTACK, FindObjectOfType<BaseAttack_FireKnight>());
        attackBehaviour.RegisterSkill(EKeyName.SKILL1, FindObjectOfType<SwiftDemonSlash_FireKnight>());
        attackBehaviour.RegisterSkill(EKeyName.SKILL2, FindObjectOfType<Crescent_FireKnight>());
        attackBehaviour.RegisterSkill(EKeyName.SKILL3, FindObjectOfType<Dodge_FireKnight>());

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

        CanMove = true;
        CanLookBack = true;
        CanJump = true;
        CanAttack = true;

        DefenseHitboxController.EnableHitbox(0);
    }

    protected virtual void Update()
    {
        curBehaviour.OnUpdate();

        DefenseHitboxController.CalculateHitbox();
    }

    protected virtual void FixedUpdate()
    {
        jumpBehaviour.OnFixedUpdate();

        curBehaviour.OnFixedUpdate();
    }

    protected virtual void LateUpdate()
    {
        curBehaviour.OnLateUpdate();
    }

    #endregion Unity Events

    #region Methods

    #region Override

    /// <summary>
    /// Set the current behaviour of the character to the received behaviour code.
    /// </summary>
    /// <param name="behaviourCode">The hash code of the behaviour to set</param>
    public override void SetBehaviour(int behaviourCode)
    {
        if (!behaviourDictionary.ContainsKey(behaviourCode))
        {
            throw new System.Exception($"There is no behaviour matching the given code. Input : {behaviourCode}");
        }

        curBehaviour = behaviourDictionary[behaviourCode];
        curBehaviour.OnStart();
    }

    #endregion Override

    #region Event

    /// <summary>
    /// The event method called when the player control the joystick.
    /// </summary>
    /// <param name="direction">The direction vector received through the joystick</param>
    public void OnJoystickMoved(Vector3 direction)
    {
        if (!CanMove) return;

        moveBehaviour.Move(direction);
    }

    /// <summary>
    /// The event method called when the player press the jump button.
    /// </summary>
    public void OnJumpButtonPressed()
    {
        if (!CanJump) return;

        jumpBehaviour.Jump();
    }

    /// <summary>
    /// The event method called when the player press the base attack button.
    /// </summary>
    public void OnAttackButtonPressed()
    {
        attackBehaviour.OnSkillButtonPressed(EKeyName.BASEATTACK);

        if (!CanAttack) return;
        if (!attackBehaviour.CheckCanAttack(EKeyName.BASEATTACK)) return;

        attackBehaviour.Attack(EKeyName.BASEATTACK);
    }

    /// <summary>
    /// The event method called when the player release the base attack button.
    /// </summary>
    public void OnAttackButtonReleased()
    {
        attackBehaviour.OnSkillButtonReleased(EKeyName.BASEATTACK);
    }

    /// <summary>
    /// The event method called when the player press the skill button.
    /// </summary>
    /// <param name="keyName">The name of skill button</param>
    public void OnSkillButtonPressed(EKeyName keyName)
    {
        attackBehaviour.OnSkillButtonPressed(keyName);

        if (!CanAttack) return;
        if (!attackBehaviour.CheckCanAttack(keyName)) return;

        attackBehaviour.Attack(keyName);
    }

    /// <summary>
    /// The event method called when the player release the skill button.
    /// </summary>
    /// <param name="keyName">The name of skill button</param>
    public void OnSkillButtonReleased(EKeyName keyName)
    {
        attackBehaviour.OnSkillButtonReleased(keyName);
    }

    #endregion Event 

    #endregion Methods
}
