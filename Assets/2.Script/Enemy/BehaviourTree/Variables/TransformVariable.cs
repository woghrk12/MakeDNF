using UnityEngine;

namespace BehaviourTree
{
    public class TransformVariable : Variable<Transform>
    {
        #region Methods

        protected override bool Equals(Transform value1, Transform value2)
        {
            return ReferenceEquals(value1, value2);
        }

        #endregion Methods
    }
}