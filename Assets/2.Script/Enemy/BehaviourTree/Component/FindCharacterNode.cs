using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class FindCharacterNode : ConditionNode
    {
        #region Variables

        [SerializeField] private string dnfTransformKey = string.Empty;
        [SerializeField] private string targetKey = string.Empty;

        #endregion Variables

        #region Methods

        protected override void OnStart() { }

        protected override bool CheckCondition()
        {
            List<IDamagable> characterList = GameManager.Room.CharacterList;

            if (characterList.Count <= 0) return false;

            if (!blackboard.TryGetVariable(dnfTransformKey, out DNFTransformVariable variable))
            {
                variable = gameObject.AddComponent<DNFTransformVariable>();

                variable.hideFlags = HideFlags.HideInInspector;
                variable.Key = dnfTransformKey;
                variable.Value = gameObject.GetComponentInParent<DNFTransform>();

                blackboard.AddVariable(dnfTransformKey, variable);
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

            if (!blackboard.TryGetVariable(targetKey, out DNFTransformVariable targetVariable))
            {
                targetVariable = gameObject.AddComponent<DNFTransformVariable>();

                targetVariable.hideFlags = HideFlags.HideInInspector;
                targetVariable.Key = targetKey;

                blackboard.AddVariable(targetKey, targetVariable);
            }

            targetVariable.Value = nearestTransform;

            return true;
        }

        #endregion Methods
    }
}