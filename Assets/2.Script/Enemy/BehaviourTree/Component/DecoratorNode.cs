using UnityEngine;

namespace BehaviourTree
{
    public abstract class DecoratorNode : Node
    {
        #region Variables

        [HideInInspector] public Node ChildNode = null;

        #endregion Variables
    }
}
