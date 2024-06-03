using System;
using System.Collections.Generic;

namespace BehaviourTree
{
    public class Blackboard
    {
        #region Variables

        private Dictionary<string, BlackboardVariable> variableDictionary = new();

        private List<Type> typeList = new();

        #endregion Variables

        #region Methods

        public T GetVariable<T>(string key) where T : BlackboardVariable
        {
            return variableDictionary.TryGetValue(key, out BlackboardVariable variable) ? (T)variable : null;
        }

        public bool CheckHasVariable(string key) => variableDictionary.ContainsKey(key);

        public void AddVariable<T>(string key, T variable) where T : BlackboardVariable
        {
            if (variableDictionary.ContainsKey(key)) return;

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
