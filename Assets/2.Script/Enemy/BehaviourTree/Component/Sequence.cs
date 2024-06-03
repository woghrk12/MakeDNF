namespace BehaviourTree
{
    public class Sequence : Composite
    {
        #region Constructor

        public Sequence(Enemy controller) : base(controller) { }

        #endregion Constructor

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

                    case ENodeState.FAILURE:
                        return ENodeState.FAILURE;
                }
            }

            return ENodeState.SUCCESS;
        }

        #endregion Methods
    }
}