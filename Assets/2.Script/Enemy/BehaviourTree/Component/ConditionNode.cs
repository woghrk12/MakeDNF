namespace BehaviourTree
{
    public abstract class ConditionNode : DecoratorNode
    {
        #region Methods

        protected override ENodeState OnUpdate()
        {
            if (ReferenceEquals(ChildNode, null)) return ENodeState.FAILURE;

            if (State == ENodeState.RUNNING || CheckCondition())
            {
                State = ChildNode.Evaluate();

                return State;
            }

            return ENodeState.FAILURE;
        }

        protected abstract bool CheckCondition();

        #endregion Methods
    }
}
