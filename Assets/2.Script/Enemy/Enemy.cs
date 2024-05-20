using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    #region Variables

    [Header("Unity components")]
    private Animator animator = null;

    [Header("DNF components")]
    private DNFTransform dnfTransform = null;
    private DNFRigidbody dnfRigidbody = null;

    /// <summary>
    /// The default behaviour to be set when the enemy object is initialized.
    /// </summary>
    [SerializeField] private EnemyBehaviour defaultBehaviour = null;

    /// <summary>
    /// The current state of the enemy.
    /// </summary>
    private EnemyBehaviour curBehaviour = null;

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

    #endregion Variables

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

    private void Awake()
    {
        animator = GetComponent<Animator>();

        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();

        outlineEffect = GetComponent<OutlineEffect>();
        stiffnessEffect = GetComponent<StiffnessEffect>();

        DefenderHitboxController = GetComponent<HitboxController>();

        DefenderDNFTransform = dnfTransform;
        DefenderHitboxController.Init(dnfTransform);
    }

    private void Start()
    {
        DefenderHitboxController.EnableHitbox(0);

        TransitionToState(defaultBehaviour);
    }

    private void Update()
    {
        curBehaviour?.OnUpdate(this);
    }

    private void FixedUpdate()
    {
        curBehaviour?.OnFixedUpdate(this);
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Transition from the current behaviour to the next behaviour.
    /// </summary>
    public void TransitionToState(EnemyBehaviour nextBehaviour)
    {
        curBehaviour?.OnEnd(this);

        curBehaviour = nextBehaviour;
        curBehaviour.OnStart(this);
    }

    #endregion Methods
}
