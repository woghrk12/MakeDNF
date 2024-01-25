using UnityEngine;

/// <summary>
/// The abstract class for state machine controller.
/// Need to add the structure variable for maintaining each state.
/// </summary>
public abstract class BehaviourController : MonoBehaviour
{
    #region Methods

    /// <summary>
    /// Set the current behaviour of the controller to the received behaviour code.
    /// </summary>
    /// <param name="behaviourCode">The hash code of the behaviour to set</param>
    public abstract void SetBehaviour(int behaviourCode);

    #endregion Methods
}