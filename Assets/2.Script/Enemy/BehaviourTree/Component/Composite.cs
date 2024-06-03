using System.Collections.Generic;

namespace BehaviourTree
{
    public abstract class Composite : Node
    {
        #region Properties

        public List<Node> ChildList { protected set; get; } = new();

        public int ChildCount => ChildList.Count;

        #endregion Properties

        #region Methods

        public void AddChild(Node node)
        {
            if (ChildList.Contains(node)) return;

            ChildList.Add(node);
        }

        public void RemoveChild(Node node)
        {
            if (!ChildList.Contains(node)) return;

            ChildList.Remove(node);
        }

        #endregion Methods
    }
}
