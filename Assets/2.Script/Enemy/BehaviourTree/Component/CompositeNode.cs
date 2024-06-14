using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class CompositeNode : Node
    {
        #region Variables

        private List<Node> childNodeList = new();

        #endregion Variables

        #region Properties

        public List<Node> ChildNodeList => childNodeList;

        #endregion Properties

        #region Methods

        public void AddChildNode(Node node)
        {
            if (childNodeList.Contains(node))
            {
                Debug.LogWarning($"The node {node.name} already exist.");
                return;
            }

            childNodeList.Add(node);
        }

        public void RemoveChildNode(Node node)
        {
            if (!childNodeList.Contains(node))
            {
                Debug.LogWarning($"The node {node.name} doesn't exist.");
                return;
            }

            childNodeList.Remove(node);
        }

        #endregion Methods
    }
}
