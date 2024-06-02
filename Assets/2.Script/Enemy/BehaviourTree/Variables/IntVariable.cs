namespace BehaviourTree
{
    public class IntVariable : Variable<int>
    {
        #region Methods

        protected override bool Equals(int value1, int value2)
        {
            return value1 == value2;
        }

        #endregion Methods
    }
}
