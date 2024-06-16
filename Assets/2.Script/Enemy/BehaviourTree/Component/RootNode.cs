using UnityEngine;

namespace BehaviourTree
{
    [DisallowMultipleComponent]
    public sealed class RootNode : Node
    {
        #region Variables 

        private Node childNode = null;

        #endregion Variables

        #region Properties

        public Node ChildNode => childNode;

        public override bool HasChild => !ReferenceEquals(childNode, null);

        #endregion Properties

        #region Methods

        public override void AddChildNode(Node node)
        {
            if (!ReferenceEquals(childNode, null) && childNode.GUID.Equals(node.GUID)) return;

            childNode = node;
        }

        public override void RemoveChildNode(Node node)
        {
            if (ReferenceEquals(childNode, null)) return;

            childNode = null;
        }

        #region Events

        protected override void OnStart() { }

        protected override ENodeState OnUpdate()
        {
            if (ReferenceEquals(ChildNode, null)) return ENodeState.FAILURE;

            return ChildNode.Evaluate();
        }

        protected override void OnStop() { }

        #endregion Events

        #endregion Methods
    }
}