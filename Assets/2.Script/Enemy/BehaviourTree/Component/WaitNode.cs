using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class WaitNode : ActionNode
    {
        #region Variables

        public float Duration = 1f;

        private float startTime = 0f;

        protected override void OnStart()
        {
            startTime = Time.time;
        }

        protected override ENodeState OnUpdate()
        {
            return Time.time - startTime > Duration ? ENodeState.SUCCESS : ENodeState.RUNNING;
        }

        protected override void OnStop() { }


        #endregion Variables
    }
}
