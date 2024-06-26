using UnityEngine;

namespace BehaviourTree
{
    public abstract class DecoratorNode : Node
    {
        #region Variables

        [SerializeField, HideInInspector] private Node childNode = null;

        #endregion Variables

        #region Properties

        public Node ChildNode => childNode;

        public override bool HasChild => !ReferenceEquals(childNode, null);

        #endregion Properties

        #region Methods

        public sealed override void AddChildNode(Node node)
        {
            if (!ReferenceEquals(childNode, null) && childNode.GUID.Equals(node.GUID)) return;

            childNode = node;
        }

        public sealed override void RemoveChildNode(Node node)
        {
            if (ReferenceEquals(childNode, null)) return;

            childNode = null;
        }

        #region Events

        protected override void OnStop()
        {
            if (!HasChild) return;

            ChildNode.State = ENodeState.NONE;
        }

        #endregion Events

        #endregion Methods
    }
}
