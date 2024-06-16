using System;
using System.Collections.Generic;
using System.Linq;  
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class BehaviourTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }

        #region Variables
        
        private BehaviourTree behaviourTree = null;
        private List<Node> nodeList = new();

        public event Action<NodeView> NodeViewSelected = null;

        private Vector2 cachedMousePosition = Vector2.zero;

        #endregion Variables

        #region Constructor

        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/2.Script/Editor/ResourceTools/BehaviourTree/BehaviourTreeEditor.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        #endregion Constructor

        #region Methods

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // Find all types by using reflection
            /* 
            IEnumerable<Type> typesDerivedFromActionNode, typesDerivedFromDecoratorNode, typesDerivedFromCompositeNode;
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                typesDerivedFromActionNode = assembly.GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(ActionNode)));
                typesDerivedFromDecoratorNode = assembly.GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(DecoratorNode)));
                typesDerivedFromCompositeNode = assembly.GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(CompositeNode)));

                foreach (Type type in typesDerivedFromActionNode)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (action) => AddNode(type));
                }

                foreach (Type type in typesDerivedFromDecoratorNode)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (action) => AddNode(type));
                }

                foreach (Type type in typesDerivedFromCompositeNode)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (action) => AddNode(type));
                }
            }
            */

            cachedMousePosition = evt.localMousePosition;

            // Find all types by using TypeCache class
            // Add menu for root node
            evt.menu.AppendAction("RootNode", (action) => AddNode(typeof(RootNode), cachedMousePosition));

            // Add menu for action nodes
            var typesDerivedFromActionNode = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in typesDerivedFromActionNode)
            {
                evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", (action) => AddNode(type, cachedMousePosition));
            }

            // Add menu for decorator nodes
            var typesDerivedFromDecoratorNode = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in typesDerivedFromDecoratorNode)
            {
                evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", (action) => AddNode(type, cachedMousePosition));
            }

            // Add menu for composite nodes
            var typesDerivedFromCompositeNode = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in typesDerivedFromCompositeNode)
            {
                evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", (action) => AddNode(type, cachedMousePosition));
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(endPort =>
                    endPort.direction != startPort.direction
                    && endPort.node != startPort.node)
                .ToList();
        } 

        public void PopulateView(BehaviourTree behaviourTree)
        {
            this.behaviourTree = behaviourTree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (!this.behaviourTree.gameObject.TryGetComponent(out RootNode rootNode))
            {
                rootNode = this.behaviourTree.gameObject.AddComponent<RootNode>();

                rootNode.GUID = GUID.Generate().ToString();
                rootNode.hideFlags = HideFlags.HideInInspector;
                rootNode.Name = typeof(RootNode).Name;
            }

            nodeList = this.behaviourTree.GetComponents<Node>().ToList();

            // Creates node views
            nodeList.ForEach(node => AddNodeView(node));

            // Create edges
            nodeList.ForEach(node =>
            {
                if (!node.HasChild) return;

                if (node is RootNode)
                {
                    NodeView parentNodeView = FindNodeView(node);
                    NodeView childNodeView = FindNodeView((node as RootNode).ChildNode);
                    
                    AddElement(parentNodeView.OutputPort.ConnectTo(childNodeView.InputPort));
                }
                else if (node is DecoratorNode)
                {
                    NodeView parentNodeView = FindNodeView(node);
                    NodeView childNodeView = FindNodeView((node as DecoratorNode).ChildNode);
                    
                    AddElement(parentNodeView.OutputPort.ConnectTo(childNodeView.InputPort));
                }
                else if (node is CompositeNode)
                {
                    NodeView parentNodeView = FindNodeView(node);
                    (node as CompositeNode).ChildNodeList.ForEach((childNode) =>
                    {
                        NodeView childNodeView = FindNodeView(childNode);
                        
                        AddElement(parentNodeView.OutputPort.ConnectTo(childNodeView.InputPort));
                    });
                }
            });
        }

        private void AddNode(Type type, Vector2 mousePosition)
        {
            if (type == typeof(RootNode) && behaviourTree.TryGetComponent(out RootNode rootNode))
            {
                Debug.LogWarning("Can't add 'RootNode' to Behaviour Tree because a 'RootNode' is already added to the game object.");
                return;
            }

            Node newNode = Undo.AddComponent(behaviourTree.gameObject, type) as Node;

            newNode.GUID = GUID.Generate().ToString();
            newNode.Name = type.Name;
            newNode.Position = mousePosition;
            newNode.hideFlags = HideFlags.HideInInspector;

            AddNodeView(newNode);
        }

        private void AddNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);

            nodeView.NodeViewSelected += NodeViewSelected;

            AddElement(nodeView);
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.GUID) as NodeView;
        }

        public void UpdateNodeViews()
        {
            nodes.ForEach(node => (node as NodeView).UpdateNodeView());
        }

        #region Events

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (!ReferenceEquals(graphViewChange.elementsToRemove, null))
            {
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    if (element is NodeView)
                    {
                        Node node = (element as NodeView).Node;

                        nodeList.Remove(node);

                        Undo.DestroyObjectImmediate(node);
                    }
                    else if (element is Edge)
                    {
                        Edge edge = element as Edge;

                        NodeView parentNodeView = edge.output.node as NodeView;
                        NodeView childNodeView = edge.input.node as NodeView;

                        parentNodeView.Node.RemoveChildNode(childNodeView.Node);
                    }
                });
            }

            if (!ReferenceEquals(graphViewChange.edgesToCreate, null))
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentNodeView = edge.output.node as NodeView;
                    NodeView childNodeView = edge.input.node as NodeView;

                    parentNodeView.Node.AddChildNode(childNodeView.Node);
                });
            }

            if (!ReferenceEquals(graphViewChange.movedElements, null))
            {
                nodes.ForEach(node => (node as NodeView).SortChildNode());
            }

            return graphViewChange;
        }

        private void OnUndoRedo()
        {
            PopulateView(behaviourTree);
        }

        #endregion Events

        #endregion Methods
    }
}