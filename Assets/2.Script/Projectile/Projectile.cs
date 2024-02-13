using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    #region Variables
    
    protected Animator animator = null;

    protected DNFTransform dnfTransform = null;
    protected DNFRigidbody dnfRigidbody = null;

    /// <summary>
    /// The DNFTransform component that create the projectile.
    /// </summary>
    protected DNFTransform spawnerTransform = null;

    /// <summary>
    /// List of hitboxes representing potential targets for the projectile.
    /// </summary>
    protected List<Hitbox> targetList = new();

    protected List<ProjectileState> stateList = new();
    protected ProjectileState curState = null;

    #endregion Variables

    #region Properties

    /// <summary>
    /// A index indicating which queue of the object pool the object will be returned to.
    /// </summary>
    protected abstract EObjectPoolList ObjectPoolIndex { get; }

    #endregion Properties

    #region Unity Events

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();
    }

    private void Update()
    {
        if (curState == null) return;

        curState.OnUpdate();
    }

    private void FixedUpdate()
    {
        if (curState == null) return;

        curState.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        if (curState == null) return;

        curState.OnLateUpdate();
    }

    #endregion Unity Events

    #region Methods

    public void SetState(int index)
    {
        if (index < 0 || index >= stateList.Count)
        {
            throw new System.Exception($"Out of range. GameObject : {gameObject.name}, Input index : {index}");
        }

        curState = stateList[index];
        curState.OnStart();
    }

    /// <summary>
    /// Activates the projectile, initiating its travel and behaviour.
    /// </summary>
    /// <param name="subjectTransform">The DNF transform of the object that created the projectile</param>
    /// <param name="sizeEff">The size ratio of the projectile</param>
    public abstract void Activate(DNFTransform subjectTransform, float sizeEff = 1f);

    /// <summary>
    /// Clears the projectile, marking it as completed and preparing for potential recycling.
    /// </summary>
    public void Complete() 
    {
        curState = null;

        GameManager.ObjectPool.ReturnToPool(ObjectPoolIndex, gameObject);
    }

    #endregion Methods
}
