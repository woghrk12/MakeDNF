using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class FindCharacterNode : ConditionNode
    {
        protected override bool CheckCondition()
        {
            List<IDamagable> characterList = GameManager.Room.CharacterList;

            if (characterList.Count <= 0) return false;

            if (!blackboard.TryGetVariable("DNF Transform", out DNFTransformVariable variable))
            {
                variable = gameObject.AddComponent<DNFTransformVariable>();

                variable.hideFlags = HideFlags.HideInInspector;
                variable.Value = gameObject.GetComponentInParent<DNFTransform>();

                blackboard.AddVariable("DNF Transform", variable);
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

            if (!blackboard.TryGetVariable("Target Transform", out DNFTransformVariable targetVariable))
            {
                targetVariable = gameObject.AddComponent<DNFTransformVariable>();
                targetVariable.hideFlags = HideFlags.HideInInspector;

                blackboard.AddVariable("Target Transform", targetVariable);
            }

            targetVariable.Value = nearestTransform;

            return true;
        }
    }
}