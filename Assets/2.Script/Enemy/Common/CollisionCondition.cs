using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class CollisionCondition : Condition
    {
        #region Variables

        private string targetVariableName = string.Empty;

        private HitboxController detectorHitboxController = null;

        #endregion Variables

        #region Constructor

        public CollisionCondition(string targetVariableName, HitboxController hitboxController, Enemy controller) : base(controller)
        {
            this.targetVariableName = targetVariableName;
            detectorHitboxController = hitboxController;
        }

        #endregion Consturctor

        #region Methods

        protected override bool CheckCondition(Blackboard blackboard)
        {
            if (!blackboard.TryGetVariable(targetVariableName, out TransformVariable variable)) return false;

            if (!variable.Value.TryGetComponent(out IDamagable target)) return false;

            return detectorHitboxController.CheckCollision(target.DefenderHitboxController);
        }

        #endregion Methods
    }
}
