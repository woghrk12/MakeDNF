namespace BehaviourTree
{
    public abstract class ConditionNode : DecoratorNode
    {
        #region Methods

        protected override ENodeState OnUpdate()
        {
            if (ReferenceEquals(ChildNode, null)) return ENodeState.FAILURE;

            if (ChildNode.State == ENodeState.RUNNING) return ENodeState.RUNNING;

            if (CheckCondition())
            {
                return ChildNode.Evaluate();
            }

            return ENodeState.FAILURE;
        }

        protected abstract bool CheckCondition();

        #endregion Methods
    }
}
