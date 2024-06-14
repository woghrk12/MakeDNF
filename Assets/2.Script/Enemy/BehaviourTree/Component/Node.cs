using UnityEngine;

namespace BehaviourTree
{
    public enum ENodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    [RequireComponent(typeof(BehaviourTree), typeof(Blackboard))]
    public abstract class Node : MonoBehaviour
    {
        #region Variables

        [HideInInspector] public string GUID = string.Empty;
        [HideInInspector] public Vector2 Position = Vector2.zero;

        protected Blackboard blackboard = null;

        [TextArea] public string description = string.Empty;

        #endregion Variables

        #region Properties

        public ENodeState State { protected set; get; } = ENodeState.RUNNING;

        public bool IsStarted { protected set; get; } = false;

        #endregion Properties

        #region Unity Events

        protected void Awake()
        {
            blackboard = GetComponent<Blackboard>();
        }

        #endregion Unity Events

        #region Methods

        public ENodeState Evaluate()
        {
            if (!IsStarted)
            {
                OnStart();
                IsStarted = true;
            }

            State = OnUpdate();

            if (State == ENodeState.SUCCESS || State == ENodeState.FAILURE)
            {
                OnStop();
                IsStarted = false;
            }

            return State;
        }

        #region Events

        protected abstract void OnStart();

        protected abstract ENodeState OnUpdate();

        protected abstract void OnStop();

        #endregion Events

        #endregion Methods
    }
}
