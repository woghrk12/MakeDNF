using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using GraphNode = UnityEditor.Experimental.GraphView.Node;

namespace BehaviourTree
{
    public class NodeView : GraphNode
    {
        #region Variables

        public Node Node = null;

        private Action<NodeView> nodeViewSelected = null;

        #endregion Variables

        #region Properites

        public Port InputPort { private set; get; } = null;

        public Port OutputPort { private set; get; } = null;

        public event Action<NodeView> NodeViewSelected
        {
            add { nodeViewSelected += value; }
            remove { nodeViewSelected -= value; }
        }

        #endregion Properties

        #region Constructor

        public NodeView(Node node) : base("Assets/2.Script/Editor/ResourceTools/BehaviourTree/NodeView.uxml")
        {
            Node = node;
            title = node.Name;
            viewDataKey = node.GUID;

            style.left = node.Position.x;
            style.top = node.Position.y;

            AddInputPorts();
            AddOutputPorts();
            SetupClasses();

            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(new SerializedObject(node));
        }

        #endregion Constructor

        #region Methods

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            Undo.RecordObject(Node, "Behaviour Tree (Set Position)");

            Node.Position.x = newPos.x;
            Node.Position.y = newPos.y;
        }

        private void AddInputPorts()
        {
            if (Node is ActionNode)
            {
                InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (Node is DecoratorNode)
            {
                InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (Node is CompositeNode)
            {
                InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if (InputPort != null)
            {
                InputPort.portName = "";
                InputPort.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(InputPort);
            }
        }

        private void AddOutputPorts()
        {
            if (Node is RootNode)
            {
                OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            else if (Node is DecoratorNode)
            {
                OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            else if (Node is CompositeNode)
            {
                OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }

            if (OutputPort != null)
            {
                OutputPort.portName = "";
                OutputPort.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(OutputPort);
            }
        }

        private void SetupClasses()
        {
            if (Node is RootNode)
            {
                AddToClassList("root");
            }
            else if (Node is ActionNode)
            {
                AddToClassList("action");
            }
            else if (Node is DecoratorNode)
            {
                AddToClassList("decorator");
            }
            else if (Node is CompositeNode)
            {
                AddToClassList("composite");
            }
        }

        public void SortChildNode()
        {
            if (Node is not CompositeNode) return;

            (Node as CompositeNode).ChildNodeList.Sort(SortByHorizontalPosition);
        }

        private int SortByHorizontalPosition(Node leftNode, Node rightNode)
        {
            return leftNode.Position.x < rightNode.Position.x ? -1 : 1;
        }

        public void UpdateNodeView()
        {
            if (ClassListContains("running"))
            {
                RemoveFromClassList("running");
            }

            if (ClassListContains("failure"))
            {
                RemoveFromClassList("failure");
            }

            if (ClassListContains("success"))
            {
                RemoveFromClassList("success");
            }

            if (!Application.isPlaying) return;

            switch (Node.State)
            {
                case ENodeState.RUNNING:
                    if (!Node.IsStarted) break;

                    AddToClassList("running");
                    break;

                case ENodeState.FAILURE:
                    AddToClassList("failure");
                    break;

                case ENodeState.SUCCESS:
                    AddToClassList("success");
                    break;
            }
        }

        #region Events

        public override void OnSelected()
        {
            base.OnSelected();

            nodeViewSelected?.Invoke(this);
        }

        #endregion Events

        #endregion Methods
    }
}
