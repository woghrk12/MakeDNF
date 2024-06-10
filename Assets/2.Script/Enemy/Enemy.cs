using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    #region Variables

    [Header("Unity components")]
    protected Animator animator = null;

    [Header("DNF components")]
    protected DNFTransform dnfTransform = null;
    protected DNFRigidbody dnfRigidbody = null;

    /// <summary>
    /// The current state of the enemy's hitbox.
    /// </summary>
    private EHitboxState hitboxState = EHitboxState.NONE;

    /// <summary>
    /// The component to apply stiffness effect to enemy.
    /// The stiffness effect occur when the enemy is hit.
    /// </summary>
    private StiffnessEffect stiffnessEffect = null;

    /// <summary>
    /// The component to apply outline effect to enemy.
    /// </summary>
    private OutlineEffect outlineEffect = null;

    /// <summary>
    /// The controller component for managing the enemy AI's behaviour tree.
    /// </summary>
    [SerializeField] private BehaviourTree.BehaviourTree behaviourController = null;

    #endregion Variables

    #region Properties

    /// <summary>
    /// The animator component of the enemy.
    /// </summary>
    public Animator Animator => animator;

    public BehaviourTree.BehaviourTree BehaviourController => behaviourController;

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

        // TODO : implement the hit behaviour
        dnfRigidbody.AddForce(knockBackDirection * knockBackPower);
    }

    #endregion IDamagable Implementation

    #region Unity Events

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();

        outlineEffect = GetComponent<OutlineEffect>();
        stiffnessEffect = GetComponent<StiffnessEffect>();

        DefenderHitboxController = GetComponent<HitboxController>();

        DefenderDNFTransform = dnfTransform;
        DefenderHitboxController.Init(dnfTransform);

        behaviourController = behaviourController.Clone();
    }

    private void Start()
    {
        DefenderHitboxController.EnableHitbox(0);
    }

    private void Update()
    {
        behaviourController.Run();
    }

    #endregion Unity Events

    #region Methods

    protected bool CheckAnimationRunning(string name)
    {
        if (animator != null)
        { 
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (!animatorStateInfo.IsName(name)) return false;

            float normalizedTime = animatorStateInfo.normalizedTime;

            return normalizedTime < 1f;
        }

        return false;
    }

    #endregion Methods
}
