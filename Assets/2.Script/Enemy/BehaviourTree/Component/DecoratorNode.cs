using UnityEngine;

namespace BehaviourTree
{
    public abstract class DecoratorNode : Node
    {
        #region Variables

        [HideInInspector] public Node ChildNode = null;

        #endregion Variables

        #region Methods

        public override Node Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.ChildNode = ChildNode.Clone();

            return node;
        }

        #endregion Methods
    }
}
