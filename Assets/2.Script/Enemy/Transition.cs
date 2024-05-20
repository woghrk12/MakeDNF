using System;

[Serializable]
public class Transition
{
    #region Variables

    /// <summary>
    /// The decision that triggers this transition.
    /// </summary>
    public Decision Decision = null;

    /// <summary>
    /// The next state to transition to if the decision condition is met.
    /// </summary>
    public EnemyBehaviour NextBehaviour = null;

    #endregion Variables
}
