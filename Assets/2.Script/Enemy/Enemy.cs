using UnityEngine;

public class Enemy : MonoBehaviour
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

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        animator = GetComponent<Animator>();

        dnfTransform = GetComponent<DNFTransform>();
        dnfRigidbody = GetComponent<DNFRigidbody>();
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
