using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class FindCharacterNode : ConditionNode
    {
        #region Variables

        [SerializeField] private string dnfTransformVariableKey = string.Empty;
        [SerializeField] private string targetTransformVariableKey = string.Empty;

        #endregion Variables

        #region Methods

        protected override bool CheckCondition()
        {
            List<IDamagable> characterList = GameManager.Room.CharacterList;

            if (characterList.Count <= 0) return false;

            if (!blackboard.TryGetVariable(dnfTransformVariableKey, out DNFTransformVariable variable))
            {
                variable = gameObject.AddComponent<DNFTransformVariable>();

                variable.hideFlags = HideFlags.HideInInspector;
                variable.Value = gameObject.GetComponentInParent<DNFTransform>();

                blackboard.AddVariable(dnfTransformVariableKey, variable);
            }

            DNFTransform dnfTransform = variable.Value;
            DNFTransform nearestTransform = characterList[0].DefenderDNFTransform;
            float minDistance = (dnfTransform.Position - nearestTransform.Position).sqrMagnitude;

            for (int index = 1; index < characterList.Count; index++)
            {
                DNFTransform characterTransform = characterList[index].DefenderDNFTransform;
                float distance = (dnfTransform.Position - characterTransform.Position).sqrMagnitude;

                if (minDistance >= distance) continue;

                nearestTransform = characterTransform;
                minDistance = distance;
            }

            if (!blackboard.TryGetVariable(targetTransformVariableKey, out DNFTransformVariable targetVariable))
            {
                targetVariable = gameObject.AddComponent<DNFTransformVariable>();
                targetVariable.hideFlags = HideFlags.HideInInspector;

                blackboard.AddVariable(targetTransformVariableKey, targetVariable);
            }

            targetVariable.Value = nearestTransform;

            return true;
        }

        #endregion Methods
    }
}