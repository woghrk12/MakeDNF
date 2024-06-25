using UnityEngine;

namespace BehaviourTree
{
    public class InRangeNode : ConditionNode
    {
        #region Variables

        [SerializeField] private string dnfTransformKey = string.Empty;
        [SerializeField] private string targetKey = string.Empty;
        [SerializeField] private float range = 0f;

        #endregion Variables

        #region Methods

        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override bool CheckCondition()
        {
            if (!blackboard.TryGetVariable(targetKey, out DNFTransformVariable targetVariable)) return false;

            if (!blackboard.TryGetVariable(dnfTransformKey, out DNFTransformVariable variable))
            {
                variable = gameObject.AddComponent<DNFTransformVariable>();

                variable.hideFlags = HideFlags.HideInInspector;
                variable.Value = gameObject.GetComponentInParent<DNFTransform>();
                variable.Key = dnfTransformKey;

                blackboard.AddVariable(dnfTransformKey, variable);
            }

            if ((variable.Value.Position - targetVariable.Value.Position).sqrMagnitude > range) return false;

            return true;
        }

        #endregion Methods
    }
}
