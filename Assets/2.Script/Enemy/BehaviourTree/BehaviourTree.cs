using UnityEngine;

namespace BehaviourTree
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        #region Variables

        public Node RootNode = null;

        #endregion Variables

        #region Methods

        public void Run()
        {
            if (ReferenceEquals(RootNode, null)) return;
            if (RootNode.State != ENodeState.RUNNING) return;

            RootNode.Evaluate();
        }

        #endregion Methods
    }
}
