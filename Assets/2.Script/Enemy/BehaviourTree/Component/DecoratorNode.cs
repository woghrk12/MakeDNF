namespace BehaviourTree
{
    public abstract class DecoratorNode : Node
    {
        #region Variables

        private Node childNode = null;

        #endregion Variables

        #region Properties

        public Node ChildNode => childNode;

        #endregion Properties

        #region Methods

        public void AddChildNode(Node node)
        {
            if (!ReferenceEquals(childNode, null) && childNode.GUID.Equals(node.GUID)) return;

            childNode = node;
        }

        public void RemoveChildNode(Node node)
        {
            if (ReferenceEquals(childNode, null)) return;

            childNode = null;
        }

        #endregion Methods
    }
}
