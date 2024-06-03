namespace BehaviourTree
{
    public abstract class Condition : Decorator
    {
        #region Methods

        public override ENodeState Evaluate(Blackboard blackboard)
        {
            if (IsChildEmpty) return ENodeState.FAILURE;

            return CheckCondition(blackboard) ? Child.Evaluate(blackboard) : ENodeState.FAILURE;
        }

        protected abstract bool CheckCondition(Blackboard blackboard);

        #endregion Methods
    }
}