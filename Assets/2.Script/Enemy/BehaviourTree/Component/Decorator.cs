namespace BehaviourTree
{
    public abstract class Decorator : Node
    {
        #region Properties

        public Node Child { protected set; get; } = null;

        public bool IsChildEmpty => ReferenceEquals(Child, null);

        #endregion Properties

        #region Constructor

        public Decorator(Enemy controller) : base(controller) { }

        #endregion Constructor

        #region Methods

        public void AddChild(Node node)
        {
            if (!ReferenceEquals(Child, null) && Child == node) return;

            Child = node;
        }

        public void RemoveChild()
        {
            if (ReferenceEquals(Child, null)) return;

            Child = null;
        }

        #endregion Methods
    }
}
