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
    private BehaviourTree.BehaviourTreeController behaviourController = null;

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
    }

    private void Start()
    {
        DefenderHitboxController.EnableHitbox(0);

        SetBehaviour();
    }

    private void Update()
    {
        behaviourController.Operate();
    }

    #endregion Unity Events

    #region Methods

    protected virtual void SetBehaviour() { }

    #endregion Methods
}
