using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourController : MonoBehaviour
{
    #region Variables

    [Header("Variables for controller's behaviour")]
    protected Dictionary<int, GenericBehaviour> behaviourDictionary = new();
    protected GenericBehaviour curBehaviour = null;

    [Header("Unity components")]
    protected Animator animator = null;

    [Header("DNF components")]
    protected DNFTransform dnfTransform = null;
    protected DNFRigidbody dnfRigidbody = null;

    [Header("Basic behaviour of the controller")]
    protected IdleBehaviour idleBehaviour = null;
    protected MoveBehaviour moveBehaviour = null;

    [Header("State flag variables")]
    private bool canMove = true;
    private bool canJump = true;

    #endregion Variables

    #region Properties

    /// <summary>
    /// Animator component of the controller.
    /// </summary>
    public Animator Animator => animator;

    /// <summary>
    /// DNF Transform component of the controller.
    /// </summary>
    public DNFTransform DNFTransform => dnfTransform;

    /// <summary>
    /// DNF Rigidbody component of the controller.
    /// </summary>
    public DNFRigidbody DNFRigidbody => dnfRigidbody;

    /// <summary>
    /// A flag variable that determines whether the controller is allowed to move.
    /// </summary>
    public virtual bool CanMove
    {
        set { canMove = value; }
        get => canMove;
    }

    /// <summary>
    /// A flag variable that determines whether the controller is allowed to jump.
    /// </summary>
    public virtual bool CanJump
    {
        set { canJump = value; }
        get => canJump && dnfRigidbody.IsGround;
    }

    #endregion Properties

    #region Unity Events

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();

        idleBehaviour = GetComponent<IdleBehaviour>();
        moveBehaviour = GetComponent<MoveBehaviour>();

        behaviourDictionary.Add(BehaviourCodeList.IDLE_BEHAVIOUR_CODE, idleBehaviour);

        curBehaviour = idleBehaviour;
    }

    private void Update()
    {
        curBehaviour.OnUpdate();
    }

    private void FixedUpdate()
    {
        curBehaviour.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        curBehaviour.OnLateUpdate();
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Set the current behaviour of the controller to the received behaviour code.
    /// </summary>
    /// <param name="behaviourCode">The hash code of the behaviour to set</param>
    public void SetBehaviour(int behaviourCode)
    {
        if (!behaviourDictionary.ContainsKey(behaviourCode))
        {
            throw new System.Exception($"There is no behaviour matching the given code. Input : {behaviourCode}");
        }

        curBehaviour = behaviourDictionary[behaviourCode];
        curBehaviour.OnStart();
    }

    #endregion Methods
}