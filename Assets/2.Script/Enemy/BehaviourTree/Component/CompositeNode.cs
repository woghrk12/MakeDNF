using UnityEngine;
using System.Collections.Generic;

namespace BehaviourTree
{
    public abstract class CompositeNode : Node
    {
        #region Variables

        [HideInInspector] public List<Node> ChildNodeList = new();

        #endregion Variables

        #region Methods

        public override Node Clone()
        {
            CompositeNode node = Instantiate(this);
            node.ChildNodeList = ChildNodeList.ConvertAll(childNode => childNode.Clone());

            return node;

        }

        #endregion Methods
    }
}
