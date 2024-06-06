using System.Collections.Generic;

namespace BehaviourTree
{
    public abstract class DecoratorNode : Node
    {
        #region Variables

        public List<Node> ChildNodeList = new();

        #endregion Variables
    }
}
