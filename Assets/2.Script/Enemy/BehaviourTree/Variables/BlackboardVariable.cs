using UnityEngine;

namespace BehaviourTree
{
    public delegate void ValueModified<T>(T oldValue, T newValue);

    public abstract class BlackboardVariable : MonoBehaviour
    {
        public string Key = string.Empty;
    }

    public abstract class Variable<T> : BlackboardVariable
    {
        #region Variables

        [SerializeField] protected T value = default(T);

        protected event ValueModified<T> valueModifiedDelegate = delegate { };

        #endregion Variables

        #region Properties

        public T Value
        {
            set
            {
                if (Equals(this.value, value)) return;

                valueModifiedDelegate?.Invoke(this.value, value);

                this.value = value;
            }
            get => Value;
        }

        #endregion Properties

        #region Methods

        protected abstract bool Equals(T value1, T value2);

        public void AddListner(ValueModified<T> listener)
        {
            valueModifiedDelegate += listener;
        }

        public void RemoveListner(ValueModified<T> listener)
        {
            valueModifiedDelegate -= listener;
        }

        #endregion Methods
    }
}