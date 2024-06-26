using UnityEngine;

namespace BehaviourTree
{
    public class DebugLogNode : ActionNode
    {
        #region Variables

        public string Message = string.Empty;

        #endregion Variables

        #region Methods

        protected override void OnStart() { }

        protected override ENodeState OnUpdate()
        {
            Debug.Log(Message);

            return ENodeState.SUCCESS;
        }

        protected override void OnStop() { }

        #endregion Methods
    }
}
