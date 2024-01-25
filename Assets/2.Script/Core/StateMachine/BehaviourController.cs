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

    protected virtual void Update()
    {
        curBehaviour.OnUpdate();
    }

    protected virtual void FixedUpdate()
    {
        curBehaviour.OnFixedUpdate();
    }

    protected virtual void LateUpdate()
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