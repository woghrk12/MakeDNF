using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    [RequireComponent(typeof(BehaviourTree))]
    public class Blackboard : MonoBehaviour
    {
        #region Variables

        [SerializeField] private List<BlackboardVariable> variableList = new();

        private Dictionary<string, BlackboardVariable> variableDictionary = new();

        #endregion Variables

        #region Unity Events

        private void Awake()
        {
            variableDictionary.Clear();

            variableList.ForEach((variable) => variableDictionary.Add(variable.Key, variable));
        }

        #endregion Unity Events

        #region Methods

        public BlackboardVariable[] GetAllVariables()
        {
            return variableList.ToArray();
        }

        public T GetVariable<T>(string key) where T : BlackboardVariable
        {
            return variableDictionary.TryGetValue(key, out BlackboardVariable variable) ? (T)variable : null;
        }

        public bool CheckHasVariable(string key) => variableDictionary.ContainsKey(key);

        public void AddVariable(string key, BlackboardVariable variable)
        {
            if (!variableDictionary.ContainsKey(key))
            {
                Debug.LogWarning($"Variable {key} already exists.");
                return;
            }

            variableDictionary.Add(key, variable);
        }

        public void RemoveVariable(string key)
        {
            if (!variableDictionary.ContainsKey(key)) return;

            variableDictionary.Remove(key);
        }

        #endregion Methods
    }
}
