using UnityEngine;

namespace BehaviourTree
{
    public class GameObjectVariable : Variable<GameObject>
    {
        #region Methods

        protected override bool Equals(GameObject value1, GameObject value2)
        {
            return ReferenceEquals(value1, value2);
        }

        #endregion Methods
    }
}