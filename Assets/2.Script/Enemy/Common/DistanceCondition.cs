using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class DistanceCondition : Condition
    {
        #region Variables

        private string targetVariableName = string.Empty;

        private float distance = 0f;

        #endregion Variables

        #region Constructor

        public DistanceCondition(string targetVariableName, float distance, Enemy controller) : base(controller)
        {
            this.targetVariableName = targetVariableName;

            this.distance = distance;
        }

        #endregion Constructor

        #region Methods

        protected override bool CheckCondition(Blackboard blackboard)
        {
            if(!blackboard.TryGetVariable(targetVariableName, out DNFTransformVariable variable)) return false;

            return (controller.DefenderDNFTransform.Position - variable.Value.Position).sqrMagnitude < distance * distance;
        }

        #endregion Methods
    }
}
