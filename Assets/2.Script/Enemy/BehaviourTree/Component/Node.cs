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

        #endregion Variables

        #region Properties

        public ENodeState State { protected set; get; } = ENodeState.RUNNING;

        public bool IsStarted { protected set; get; } = false;

        #endregion Properties

        #region Constructor

        public Node(Enemy controller)
        {
            this.controller = controller;
        }

        #endregion Constructor

        #region Methods

        public ENodeState Evaluate()
        {
            if (!IsStarted)
            {
                OnStart();
                IsStarted = true;
            }

            State = OnUpdate();

            if (State == ENodeState.SUCCESS || State == ENodeState.FAILURE)
            {
                OnStop();
                IsStarted = false;
            }

            return State;
        }

        protected abstract void OnStart();

        protected abstract ENodeState OnUpdate();

        protected abstract void OnStop();

        #endregion Methods
    }
}
