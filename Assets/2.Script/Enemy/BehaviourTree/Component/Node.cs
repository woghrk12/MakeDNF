using UnityEngine;

namespace BehaviourTree
{
    public enum ENodeState
    {
        NONE = -1,
        RUNNING,
        SUCCESS,
        FAILURE
    }

    [RequireComponent(typeof(BehaviourTree), typeof(Blackboard))]
    public abstract class Node : MonoBehaviour
    {
        #region Variables

        [HideInInspector] public string Name = string.Empty;
        [HideInInspector] public string GUID = string.Empty;
        [HideInInspector] public Vector2 Position = Vector2.zero;

        protected Blackboard blackboard = null;

        [TextArea] public string description = string.Empty;

        #endregion Variables

        #region Properties

        public ENodeState State { protected set; get; } = ENodeState.NONE;

        public bool IsRunning { protected set; get; } = false;

        public abstract bool HasChild { get; }

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
            if (!IsRunning)
            {
                OnStart();
                IsRunning = true;
            }

            State = OnUpdate();

            if (State == ENodeState.SUCCESS || State == ENodeState.FAILURE)
            {
                OnStop();
                IsRunning = false;
            }

            return State;
        }

        public abstract void AddChildNode(Node node);

        public abstract void RemoveChildNode(Node node);

        #region Events

        protected abstract void OnStart();

        protected abstract ENodeState OnUpdate();

        protected abstract void OnStop();

        #endregion Events

        #endregion Methods
    }
}
