using UnityEngine;

namespace BehaviourTree
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        #region Variables

        private Node rootNode = null;

        #endregion Variables

        #region Methods

        public void Run()
        {
            if (ReferenceEquals(rootNode, null)) return;

            rootNode.Evaluate();
        }

        #endregion Methods
    }
}
