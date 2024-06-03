namespace BehaviourTree
{
    public class DNFTransformVariable : Variable<DNFTransform>
    {
        #region Methods

        protected override bool Equals(DNFTransform value1, DNFTransform value2)
        {
            return ReferenceEquals(value1, value2);
        }

        #endregion Methods
    }
}