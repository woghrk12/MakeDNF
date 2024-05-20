using UnityEngine;

public class EnemyBehaviour : ScriptableObject
{
    #region Variables

    /// <summary>
    /// Transitions from current state to other states.
    /// </summary>
    [SerializeField] private Transition[] transitions = null;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Checks if any transition conditions are met.
    /// If there is a state that satisfies the transition condition, transition to the state.
    /// </summary>
    public void CheckTransitions(Enemy controller)
    {
        foreach (Transition transition in transitions)
        {
            if (transition.Decision.Decide(controller))
            {
                controller.TransitionToState(transition.NextBehaviour);
                break;
            }
        }
    }

    #region Events

    /// <summary>
    /// The event method called when the behaviour is enabled.
    /// </summary>
    public virtual void OnStart(Enemy controller) { }

    /// <summary>
    /// The event method called every update frame.
    /// </summary>
    public virtual void OnUpdate(Enemy controller) { }

    /// <summary>
    /// The event method called every fixed update frame.
    /// </summary>
    public virtual void OnFixedUpdate(Enemy controller) { }

    /// <summary>
    /// The event method called when the behaviour is disabled
    /// </summary>
    public virtual void OnEnd(Enemy controller) { }

    #endregion Events

    #endregion Methods
}
