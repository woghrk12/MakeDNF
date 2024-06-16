namespace BehaviourTree
{
    public abstract class ActionNode : Node
    {
        public sealed override bool HasChild => false;

        public sealed override void AddChildNode(Node node) { }

        public sealed override void RemoveChildNode(Node node) { }
    }
}
