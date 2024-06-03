namespace BehaviourTree
{
    public enum ENodeState
    {
        NONE = -1,
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public abstract class Node
    {
        #region Variables

        protected Enemy controller = null;

        protected ENodeState state = ENodeState.NONE;

        #endregion Variables

        #region Constructor

        public Node(Enemy controller)
        {
            this.controller = controller;
        }

        #endregion Constructor

        #region Methods

        public abstract ENodeState Evaluate(Blackboard blackboard);

        #endregion Methods
    }
}
