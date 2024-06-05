namespace BehaviourTree
{
    public class SequnceNode : CompositeNode
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
            if (childNodeList.Count == 0) return ENodeState.FAILURE;

            switch (childNodeList[index].Evaluate())
            {
                case ENodeState.RUNNING:
                    return ENodeState.RUNNING;

                case ENodeState.FAILURE:
                    return ENodeState.FAILURE;

                case ENodeState.SUCCESS:
                    index++;
                    break;
            }

            return index == childNodeList.Count ? ENodeState.SUCCESS : ENodeState.RUNNING;
        }

        protected override void OnStop() { }

        #endregion Methods
    }
}
