using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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

        public NodeView(Node node)
        {
            Node = node;
            title = node.name;
            viewDataKey = node.GUID;

            style.left = node.Position.x;
            style.top = node.Position.y;

            AddInputPorts();
            AddOutputPorts();
        }

        #endregion Constructor

        #region Methods

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
                
            Node.Position.x = newPos.xMin;
            Node.Position.y = newPos.yMin;
        }

        public override void OnSelected()
        {
            base.OnSelected();

            nodeViewSelected?.Invoke(this);
        }

        private void AddInputPorts()
        {
            if (Node is ActionNode)
            {
                InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (Node is DecoratorNode)
            {
                InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (Node is CompositeNode)
            {
                InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if (InputPort != null)
            {
                InputPort.portName = "";
                inputContainer.Add(InputPort);
            }
        }

        private void AddOutputPorts()
        {
            if (Node is RootNode)
            {
                OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            else if (Node is DecoratorNode)
            {
                OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            else if (Node is CompositeNode)
            {
                OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }

            if (OutputPort != null)
            {
                OutputPort.portName = "";
                outputContainer.Add(OutputPort);
            }
        }

        #endregion Methods
    }
}
