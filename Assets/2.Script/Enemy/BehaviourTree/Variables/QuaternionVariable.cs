using UnityEngine;

namespace BehaviourTree
{
    public class QuaternionVariable : Variable<Quaternion>
    {
        #region Methods

        protected override bool Equals(Quaternion value1, Quaternion value2)
        {
            return Mathf.Approximately(value1.x, value2.x)
                && Mathf.Approximately(value1.y, value2.y)
                && Mathf.Approximately(value1.z, value2.z)
                && Mathf.Approximately(value1.w, value2.w);
        }

        #endregion Methods
    }
}