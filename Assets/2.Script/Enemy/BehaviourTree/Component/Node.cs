using UnityEngine;

namespace BehaviourTree
{
    public enum ENodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public abstract class Node : ScriptableObject
    {
        #region Properties

        public ENodeState State { protected set; get; } = ENodeState.RUNNING;

        public bool IsStarted { protected set; get; } = false;

        #endregion Properties

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

        protected abstract void OnStart();

        protected abstract ENodeState OnUpdate();

        protected abstract void OnStop();

        #endregion Methods
    }
}
