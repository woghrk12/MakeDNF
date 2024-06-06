namespace BehaviourTree
{
    public class SelectorNode : CompositeNode
    {
        #region Variables

        private int index = 0;

        #endregion Variables

        #region Methods

        protected override void OnStart()
        {
            index = 0;
        }

        protected override ENodeState OnUpdate()
        {
            if (ChildNodeList.Count == 0) return ENodeState.FAILURE;

            switch (ChildNodeList[index].Evaluate())
            {
                case ENodeState.RUNNING:
                    return ENodeState.RUNNING;

                case ENodeState.FAILURE:
                    index++;
                    break;

                case ENodeState.SUCCESS:
                    return ENodeState.SUCCESS;
            }

            return index == ChildNodeList.Count ? ENodeState.FAILURE : ENodeState.RUNNING;
        }

        protected override void OnStop() { }

        #endregion Methods
    }
}
