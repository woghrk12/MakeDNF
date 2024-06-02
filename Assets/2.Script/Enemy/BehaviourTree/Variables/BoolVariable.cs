namespace BehaviourTree
{
    public class BoolVariable : Variable<bool>
    {
        #region Methods

        protected override bool Equals(bool value1, bool value2)
        {
            return value1 == value2;
        }

        #endregion Methods
    }
}