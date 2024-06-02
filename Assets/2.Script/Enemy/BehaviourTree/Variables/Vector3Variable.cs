using UnityEngine;

namespace BehaviourTree
{
    public class Vector3Variable : Variable<Vector3>
    {
        #region Methods

        protected override bool Equals(Vector3 value1, Vector3 value2)
        {
            return Mathf.Approximately(value1.x, value2.x)
                && Mathf.Approximately(value1.y, value2.y)
                && Mathf.Approximately(value1.z, value2.z);
        }

        #endregion Methods
    }
}
