using System.Collections.Generic;

namespace BehaviourTree
{
    public abstract class CompositeNode : Node
    {
        #region Variables

        protected List<Node> childNodeList = new();

        #endregion Variables
    }
}
