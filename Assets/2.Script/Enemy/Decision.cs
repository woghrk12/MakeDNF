using UnityEngine;

public abstract class Decision : MonoBehaviour
{
    #region Methods

    /// <summary>
    /// Determines the next state based on the current context.
    /// This function evaluates the conditions and returns the appropriate decision.
    /// </summary>
    /// <returns>The decision made based on the current context</returns>
    public abstract bool Decide(Enemy controller);

    #endregion Methods
}
