using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourController : MonoBehaviour
{
    #region Variables

    protected Dictionary<int, GenericBehaviour> behaviourDictionary = new();
    protected GenericBehaviour curBehaviour = null;

    protected Animator animator = null;

    protected DNFTransform dnfTransform = null;
    protected DNFRigidbody dnfRigidbody = null;

    protected IdleBehaviour idleBehaviour = null;
    protected MoveBehaviour moveBehaviour = null;

    private bool canMove = true;
    private bool canJump = true;

    #endregion Variables

    #region Properties

    public Animator Animator => animator;

    public DNFTransform DNFTransform => dnfTransform;
    public DNFRigidbody DNFRigidbody => dnfRigidbody;

    public virtual bool CanMove
    {
        set { canMove = value; }
        get => canMove;
    }

    public virtual bool CanJump
    {
        set { canJump = value; }
        get => canJump && !dnfRigidbody.IsGround;
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

        behaviourDictionary.Add(BehaviourCodeList.idleBehaviourCode, idleBehaviour);

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