using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class of the controller for character, which is the parent class of other player classes.
/// </summary>
public class Character : MonoBehaviour, IDamagable
{
    #region Variables

    [Header("Variables for controller's behaviour")]
    private Dictionary<int, CharacterBehaviour> behaviourDictionary = new();
    private CharacterBehaviour curBehaviour = null;

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

    /// <summary>
    /// The dictionary containing the visual effect applied to the character.
    /// </summary>
    private Dictionary<EEffectList, VFX> vfxDictionary = new();

    /// <summary>
    /// The current state of the character's hitbox.
    /// </summary>
    private EHitboxState hitboxState = EHitboxState.NONE;

    /// <summary>
    /// The component to apply stiffness effect to character.
    /// The stiffness effect occur when the character successfully attack the target or is hit.
    /// </summary>
    private StiffnessEffect stiffnessEffect = null;

    /// <summary>
    /// The component to apply outline effect to character.
    /// </summary>
    private OutlineEffect outlineEffect = null;

    /// <summary>
    /// The event triggered when a character successfully executes an attack.
    /// </summary>
    public event Action<DNFTransform, EAttackType> AttackEvent = null;

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

    public DNFTransform DefenderDNFTransform { set; get; }

    public HitboxController DefenderHitboxController { set; get; }

    public EHitboxState HitboxState
    {
        set
        {
            hitboxState = value;

            outlineEffect.ApplyOutlineEffect(hitboxState);
        }
        get => hitboxState;
    }

    public void OnDamage(DNFTransform attacker, List<int> damages, float knockBackPower, Vector3 knockBackDirection)
    {
        stiffnessEffect.ApplyStiffnessEffect();

        if (HitboxState == EHitboxState.SUPERARMOR) return;

        curBehaviour.OnCancel();

        hitBehaviour.Hit(knockBackPower * 0.2f, knockBackDirection * knockBackPower);
    }

    #endregion IDamagable Implementation

    #region Unity Events

    private void Awake()
    {
        animator = GetComponent<Animator>();

        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();

        idleBehaviour = GetComponent<IdleBehaviour>();
        moveBehaviour = GetComponent<MoveBehaviour>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
        attackBehaviour = GetComponent<AttackBehaviour>();
        hitBehaviour = GetComponent<HitBehaviour>();

        behaviourDictionary.Add(idleBehaviour.BehaviourCode, idleBehaviour);
        behaviourDictionary.Add(moveBehaviour.BehaviourCode, moveBehaviour);
        behaviourDictionary.Add(jumpBehaviour.BehaviourCode, jumpBehaviour);
        behaviourDictionary.Add(attackBehaviour.BehaviourCode, attackBehaviour);
        behaviourDictionary.Add(hitBehaviour.BehaviourCode, hitBehaviour);

        DefenderDNFTransform = dnfTransform;
        DefenderHitboxController = GetComponent<HitboxController>();
        
        DefenderHitboxController.Init(dnfTransform);

        stiffnessEffect = GetComponent<StiffnessEffect>();
        outlineEffect = GetComponent<OutlineEffect>();
    }

    protected virtual void Start()
    {
        SetBehaviour(BehaviourCodeList.IDLE_BEHAVIOUR_CODE);

        // Camera Debug
        GameManager.Camera.SetTarget(transform);
        
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

        DefenderHitboxController.EnableHitbox(0);
    }

    private void Update()
    {
        curBehaviour.OnUpdate();

        DefenderHitboxController.CalculateHitbox();
    }

    private void FixedUpdate()
    {
        jumpBehaviour.OnFixedUpdate();

        curBehaviour.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        curBehaviour.OnLateUpdate();
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Set the current behaviour of the character to the received behaviour code.
    /// </summary>
    /// <param name="behaviourCode">The hash code of the behaviour to set</param>
    public void SetBehaviour(int behaviourCode)
    {
        if (!behaviourDictionary.ContainsKey(behaviourCode))
        {
            throw new Exception($"There is no behaviour matching the given code. Input : {behaviourCode}");
        }

        curBehaviour = behaviourDictionary[behaviourCode];
        curBehaviour.OnStart();
    }

    /// <summary>
    /// Add an effect to the character at the given index.
    /// If an effect already exists, it removes the existing effect and adds the new ones.
    /// </summary>
    /// <param name="effectIndex">The index of effect to be added</param>
    /// <param name="isAttackEffect"></param>
    public void AddEffect(EEffectList effectIndex, bool isAttackEffect = false)
    {
        if (vfxDictionary.TryGetValue(effectIndex, out VFX oldVfx))
        {
            oldVfx.ReturnEffect();
            vfxDictionary.Remove(effectIndex);
        }

        VFX vfx = GameManager.Effect.SpawnFromPool(effectIndex).GetComponent<VFX>();
        vfx.InitEffect(dnfTransform);

        if (isAttackEffect)
        {
            float attackSpeed = animator.GetFloat(AnimatorKey.Character.ATTACK_SPEED);
            vfx.SetMotionSpeed(attackSpeed);
        }

        vfxDictionary.Add(effectIndex, vfx);
    }

    /// <summary>
    /// Remove the effect corresponding to the given index.
    /// If there is no effect at the specified index for the character, nothing happens.
    /// </summary>
    /// <param name="effectIndex">The index of effect to be removed</param>
    public void RemoveEffect(EEffectList effectIndex)
    {
        if (!vfxDictionary.TryGetValue(effectIndex, out VFX vfx)) return;

        vfx.ReturnEffect();

        vfxDictionary.Remove(effectIndex);
    }

    public void RegisterSkill(EKeyName keyName, ActiveSkill activeSkill)
        => attackBehaviour.RegisterSkill(keyName, activeSkill);

    #region Event

    /// <summary>
    /// The event method called when the character successfully attack the target.
    /// </summary>
    /// <param name="targetTransform">The DNFTransform of the target hit by the character</param>
    /// <param name="attackType">The type of attack</param>
    /// <param name="hitType">The type of collision</param>
    public void OnAttack(DNFTransform targetTransform, EAttackType attackType, EHitType hitType)
    {
        if (hitType == EHitType.DIRECT)
        {
            stiffnessEffect.ApplyStiffnessEffect();

            foreach (KeyValuePair<EEffectList, VFX> vfx in vfxDictionary)
            {
                vfx.Value.StopEffect();
            }
        }

        AttackEvent?.Invoke(targetTransform, attackType);
    }

    /// <summary>
    /// The event method called when the player control the joystick.
    /// </summary>
    /// <param name="direction">The direction vector received through the joystick</param>
    private void OnJoystickMoved(Vector3 direction)
    {
        if (!CanMove) return;

        moveBehaviour.Move(direction);
    }

    /// <summary>
    /// The event method called when the player press the jump button.
    /// </summary>
    private void OnJumpButtonPressed()
    {
        if (!CanJump) return;

        jumpBehaviour.Jump();
    }

    /// <summary>
    /// The event method called when the player press the base attack button.
    /// </summary>
    private void OnAttackButtonPressed()
    {
        attackBehaviour.OnSkillButtonPressed(EKeyName.BASEATTACK);

        if (!CanAttack) return;
        if (!attackBehaviour.CheckCanAttack(EKeyName.BASEATTACK)) return;

        attackBehaviour.Attack(EKeyName.BASEATTACK);
    }

    /// <summary>
    /// The event method called when the player release the base attack button.
    /// </summary>
    private void OnAttackButtonReleased()
    {
        attackBehaviour.OnSkillButtonReleased(EKeyName.BASEATTACK);
    }

    /// <summary>
    /// The event method called when the player press the skill button.
    /// </summary>
    /// <param name="keyName">The name of skill button</param>
    private void OnSkillButtonPressed(EKeyName keyName)
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
    private void OnSkillButtonReleased(EKeyName keyName)
    {
        attackBehaviour.OnSkillButtonReleased(keyName);
    }

    #endregion Event 

    #endregion Methods
}
