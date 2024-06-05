using UnityEngine;

namespace BehaviourTree
{
    public class DebugLogNode : ActionNode
    {
        #region Variables

        public string Message = string.Empty;

        #endregion Variables

        #region Methods

        protected override void OnStart()
        {
            Debug.Log($"OnStart : {Message}");
        }

        protected override ENodeState OnUpdate()
        {
            Debug.Log($"OnUpdate : {Message}");

            return ENodeState.SUCCESS;
        }

        protected override void OnStop()
        {
            Debug.Log($"OnStop : {Message}");
        }

        #endregion Methods
    }
}
