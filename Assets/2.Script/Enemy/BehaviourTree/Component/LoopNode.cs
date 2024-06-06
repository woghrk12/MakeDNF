namespace BehaviourTree
{
    public class LoopNode : DecoratorNode
    {
        #region Variables

        public int LoopCount = 0;

        private int curCount = 0;

        #endregion Variables

        #region Methods

        protected override void OnStart() 
        {
            curCount = 0;
        }

        protected override ENodeState OnUpdate()
        {
            if (ChildNodeList[0].Evaluate() != ENodeState.RUNNING)
            {
                curCount++;
            }

            return curCount == LoopCount ? ENodeState.SUCCESS : ENodeState.RUNNING;
        }

        protected override void OnStop() { }

        #endregion Methods
    }
}