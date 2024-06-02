using UnityEngine;

namespace BehaviourTree
{
    public class Vector2Variable : Variable<Vector2>
    {
        #region Methods

        protected override bool Equals(Vector2 value1, Vector2 value2)
        {
            return Mathf.Approximately(value1.x, value2.x) && Mathf.Approximately(value1.y, value2.y);
        }

        #endregion Methods
    }
}
