using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class CompositeNode : Node
    {
        #region Variables

        [SerializeField, HideInInspector] private List<Node> childNodeList = new();

        #endregion Variables

        #region Properties

        public List<Node> ChildNodeList => childNodeList;

        public override bool HasChild => childNodeList.Count > 0;

        #endregion Properties

        #region Methods

        public sealed override void AddChildNode(Node node)
        {
            if (childNodeList.Contains(node))
            {
                Debug.LogWarning($"The node {node.name} already exist.");
                return;
            }

            childNodeList.Add(node);
        }

        public sealed override void RemoveChildNode(Node node)
        {
            if (!childNodeList.Contains(node))
            {
                Debug.LogWarning($"The node {node.name} doesn't exist.");
                return;
            }

            childNodeList.Remove(node);
        }

        #region Events

        protected override void OnStop()
        {
            if (!HasChild) return;

            foreach (Node node in ChildNodeList)
            {
                node.State = ENodeState.NONE;
            }
        }

        #endregion Events

        #endregion Methods
    }
}
