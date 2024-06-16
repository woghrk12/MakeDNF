using UnityEngine;

namespace BehaviourTree
{
    [DisallowMultipleComponent]
    public class BehaviourTree : MonoBehaviour
    {
        #region Variables

        private RootNode rootNode = null;

        #endregion Variables

        #region Unity Events

        private void Awake()
        {
            if (!TryGetComponent(out RootNode rootNode))
            {
                Debug.LogWarning("Missing root node in behaviour tree.", this);
                return;
            }

            this.rootNode = rootNode;
        }

        #endregion Unity Events

        #region Methods

        public void Run()
        {
            if (ReferenceEquals(rootNode, null)) return;
            if (rootNode.State != ENodeState.RUNNING) return;

            rootNode.Evaluate();
        }

        #endregion Methods
    }
}
