namespace BehaviourTree
{
    public abstract class Condition : Decorator
    {
        #region Constructor

        public Condition(Enemy controller) : base(controller) { }

        #endregion Constructor

        #region Methods

        public override ENodeState Evaluate(Blackboard blackboard)
        {
            if (IsChildEmpty) return ENodeState.FAILURE;

            return (Child.State == ENodeState.RUNNING || CheckCondition(blackboard)) ? Child.Evaluate(blackboard) : ENodeState.FAILURE;
        }

        protected abstract bool CheckCondition(Blackboard blackboard);

        #endregion Methods
    }
}