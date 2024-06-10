using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourTree
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        #region Variables

        public Node RootNode = null;

        public List<Node> NodeList = new();

        #endregion Variables

        #region Methods

        public void Run()
        {
            if (ReferenceEquals(RootNode, null)) return;
            if (RootNode.State != ENodeState.RUNNING) return;

            RootNode.Evaluate();
        }

        public Node AddNode(Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;

            node.name = type.Name;
            node.GUID = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");

            NodeList.Add(node);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (AddNode)");

            AssetDatabase.SaveAssets();

            return node;
        }

        public void RemoveNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (RemoveNode)");

            NodeList.Remove(node);

            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        public void AddChildNode(Node parentNode, Node childNode)
        {
            if (parentNode is RootNode)
            {
                Undo.RecordObject(parentNode, "Behaviour Tree (AddChildNode)");

                (parentNode as RootNode).ChildNode = childNode;

                EditorUtility.SetDirty(parentNode);
            }
            else if (parentNode is DecoratorNode)
            {
                Undo.RecordObject(parentNode, "Behaviour Tree (AddChildNode)");

                (parentNode as DecoratorNode).ChildNode = childNode;

                EditorUtility.SetDirty(parentNode);
            }
            else if (parentNode is CompositeNode)
            {
                Undo.RecordObject(parentNode, "Behaviour Tree (AddChildNode)");

                (parentNode as CompositeNode).ChildNodeList.Add(childNode);

                EditorUtility.SetDirty(parentNode);
            }
        }

        public void RemoveChildNode(Node parentNode, Node childNode)
        {
            if (parentNode is RootNode)
            {
                Undo.RecordObject(parentNode, "Behaviour Tree (AddChildNode)");

                (parentNode as RootNode).ChildNode = null;

                EditorUtility.SetDirty(parentNode);
            }
            else if (parentNode is DecoratorNode)
            {
                Undo.RecordObject(parentNode, "Behaviour Tree (AddChildNode)");

                (parentNode as DecoratorNode).ChildNode = null;

                EditorUtility.SetDirty(parentNode);
            }
            else if (parentNode is CompositeNode)
            {
                Undo.RecordObject(parentNode, "Behaviour Tree (AddChildNode)");

                (parentNode as CompositeNode).ChildNodeList.Remove(childNode);

                EditorUtility.SetDirty(parentNode);
            }
        }

        public List<Node> GetChildren(Node parentNode)
        {
            if (parentNode is RootNode)
            {
                RootNode node = parentNode as RootNode;

                return ReferenceEquals(node.ChildNode, null) ? null : new List<Node>() { node.ChildNode };
            }
            else if (parentNode is DecoratorNode)
            {
                DecoratorNode node = parentNode as DecoratorNode;

                return ReferenceEquals(node.ChildNode, null) ? null : new List<Node>() { node.ChildNode };
            }
            else if (parentNode is CompositeNode)
            {
                return (parentNode as CompositeNode).ChildNodeList;
            }

            return null;
        }

        public BehaviourTree Clone()
        {
            BehaviourTree behaviourTree = Instantiate(this);

            behaviourTree.RootNode = behaviourTree.RootNode.Clone();
            behaviourTree.NodeList = new List<Node>();

            Traverse(behaviourTree.RootNode, (node) => behaviourTree.NodeList.Add(node));

            return behaviourTree;
        }
        
        private void Traverse(Node node, Action<Node> nodeVisited)
        {
            if (ReferenceEquals(node, null)) return;

            nodeVisited?.Invoke(node);

            GetChildren(node)?.ForEach((childNode) => Traverse(childNode, nodeVisited));
        }

        #endregion Methods
    }
}
