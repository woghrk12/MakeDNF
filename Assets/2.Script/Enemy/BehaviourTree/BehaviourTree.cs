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

            NodeList.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();

            return node;
        }

        public void RemoveNode(Node node)
        {
            NodeList.Remove(node);

            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChildNode(Node parentNode, Node childNode)
        {
            if (parentNode is RootNode)
            {
                (parentNode as RootNode).ChildNode = childNode;
            }
            else if (parentNode is DecoratorNode)
            {
                (parentNode as DecoratorNode).ChildNode = childNode;
            }
            else if (parentNode is CompositeNode)
            {
                (parentNode as CompositeNode).ChildNodeList.Add(childNode); 
            }
        }

        public void RemoveChildNode(Node parentNode, Node childNode)
        {
            if (parentNode is RootNode)
            {
                (parentNode as RootNode).ChildNode = null;
            }
            else if (parentNode is DecoratorNode)
            {
                (parentNode as DecoratorNode).ChildNode = null;
            }
            else if (parentNode is CompositeNode)
            {
                (parentNode as CompositeNode).ChildNodeList.Remove(childNode);
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

        #endregion Methods
    }
}
