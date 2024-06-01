using System;
using System.Collections.Generic;

namespace BehaviourTree
{
    public enum ENodeState
    {
        NONE = -1,
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public interface IBehaviourNode
    {
        public ENodeState Evaluate();
    }

    public class SelectorNode : IBehaviourNode
    {
        #region Variables

        private List<IBehaviourNode> childList = null;

        #endregion Variables

        #region Constructor

        public SelectorNode(List<IBehaviourNode> childList)
        {
            this.childList = new List<IBehaviourNode>();

            foreach (IBehaviourNode node in childList)
            {
                this.childList.Add(node);
            }
        }

        #endregion Constructor

        #region IBehaviourNode Implementation

        public ENodeState Evaluate()
        {
            if (childList == null) return ENodeState.FAILURE;

            foreach (IBehaviourNode child in childList)
            {
                switch (child.Evaluate())
                {
                    case ENodeState.RUNNING:
                        return ENodeState.RUNNING;

                    case ENodeState.SUCCESS:
                        return ENodeState.SUCCESS;
                }
            }

            return ENodeState.FAILURE;
        }

        #endregion IBehaviourNode Implementation
    }

    public class SequenceNode : IBehaviourNode
    {
        #region Variables

        private List<IBehaviourNode> childList = null;

        #endregion Variables

        #region Constructor

        public SequenceNode(List<IBehaviourNode> childList)
        {
            this.childList = new List<IBehaviourNode>();

            foreach (IBehaviourNode node in childList)
            {
                this.childList.Add(node);   
            }
        }

        #endregion Constructor

        #region IBehaviourNode Implementation

        public ENodeState Evaluate()
        {
            if (childList == null || childList.Count == 0) return ENodeState.FAILURE;

            foreach (IBehaviourNode child in childList)
            {
                switch (child.Evaluate())
                {
                    case ENodeState.RUNNING:
                        return ENodeState.RUNNING;

                    case ENodeState.SUCCESS:
                        continue;

                    case ENodeState.FAILURE:
                        return ENodeState.FAILURE;
                }
            }

            return ENodeState.SUCCESS;
        }

        #endregion IBehaviourNode Implementation
    }

    public class ConditionNode : IBehaviourNode
    {
        #region Variables

        private Func<bool> condition = null;

        #endregion Variables

        #region Constructor

        public ConditionNode(Func<bool> condition)
        {
            this.condition = condition;
        }

        #endregion Constructor

        #region IBehaviourNode Implementation

        public ENodeState Evaluate()
        {
            if (condition == null) return ENodeState.FAILURE;

            return condition.Invoke() ? ENodeState.SUCCESS : ENodeState.FAILURE;
        }

        #endregion IBehaviourNode Implementation
    }

    public class ActionNode : IBehaviourNode
    {
        #region Variables

        private Func<ENodeState> evaluatedState = null;

        #endregion Variables

        #region Constructor

        public ActionNode(Func<ENodeState> evaluatedState)
        {
            this.evaluatedState = evaluatedState;
        }

        #endregion Constructor

        #region IBehaviourNode Implementation

        public ENodeState Evaluate() => evaluatedState?.Invoke() ?? ENodeState.FAILURE;

        #endregion IBehaviourNode Implementation
    }

    public class BehaviourTreeController
    {
        #region Variables

        private IBehaviourNode rootNode = null;

        #endregion Variables

        #region Constructor

        public BehaviourTreeController(IBehaviourNode rootNode)
        {
            this.rootNode = rootNode;
        }

        #endregion Constructor

        #region Methods

        public void Operate()
        {
            rootNode.Evaluate();
        }

        #endregion Methods
    }
}