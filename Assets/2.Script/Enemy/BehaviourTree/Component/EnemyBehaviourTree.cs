using UnityEngine;

namespace BehaviourTree
{
    public abstract class EnemyBehaviourTree : MonoBehaviour
    {
        #region Variables

        protected Enemy controller = null;

        protected Node rootNode = null;

        protected Blackboard blackboard = new();

        #endregion Variables

        #region Unity Events

        private void Awake()
        {
            controller = GetComponent<Enemy>();

            SetBehaviourTree();
        }

        #endregion Unity Events

        #region Methods

        public void OnUpdate()
        {
            if (ReferenceEquals(rootNode, null)) return;

            rootNode.Evaluate(blackboard);
        }

        protected abstract void SetBehaviourTree();

        #endregion Methods
    }
}
