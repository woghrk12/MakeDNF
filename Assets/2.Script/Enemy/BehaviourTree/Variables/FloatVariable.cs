using UnityEngine;

namespace BehaviourTree
{
    public class FloatVariable : Variable<float>
    {
        #region Methods

        protected override bool Equals(float value1, float value2)
        {
            return Mathf.Approximately(value1, value2);
        }

        #endregion Methods
    }
}