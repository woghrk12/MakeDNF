using System;

namespace BehaviourTree
{
    public class ConditionNode : DecoratorNode
    {
        #region Variables

        public event Func<bool> CheckingConditionHandler = null;

        #endregion Variables

        #region Methods

        protected override void OnStart() { }

        protected override ENodeState OnUpdate()
        {
            if (childNode.State == ENodeState.RUNNING) return ENodeState.RUNNING;

            if (CheckingConditionHandler?.Invoke() ?? false)
            {
                return childNode.Evaluate();
            }

            return ENodeState.FAILURE;
        }

        protected override void OnStop() { }

        #endregion Methods
    }
}