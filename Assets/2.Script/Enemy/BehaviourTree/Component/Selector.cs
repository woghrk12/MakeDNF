namespace BehaviourTree
{
    public class Selector : Composite
    {
        #region Methods

        public override ENodeState Evaluate(Blackboard blackboard)
        {
            if (ChildCount == 0) return ENodeState.FAILURE;

            foreach (Node child in ChildList)
            {
                switch (child.Evaluate(blackboard))
                {
                    case ENodeState.RUNNING:
                        return ENodeState.RUNNING;

                    case ENodeState.SUCCESS:
                        return ENodeState.SUCCESS;
                }
            }

            return ENodeState.FAILURE;
        }

        #endregion Methods
    }
}