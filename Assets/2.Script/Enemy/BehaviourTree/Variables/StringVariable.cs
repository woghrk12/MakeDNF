namespace BehaviourTree
{
    public class StringVariable : Variable<string>
    {
        #region Methods

        protected override bool Equals(string value1, string value2)
        {
            return value1.Equals(value2);
        }

        #endregion Methods
    }
}
