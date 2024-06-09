using UnityEngine;

namespace BehaviourTree
{
    public class RootNode : Node
    {
        #region Variables 

        [HideInInspector] public Node ChildNode = null;

        #endregion Variables

        #region Methods

        public override Node Clone()
        {
            RootNode node = Instantiate(this);
            node.ChildNode = ChildNode.Clone();

            return node;
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