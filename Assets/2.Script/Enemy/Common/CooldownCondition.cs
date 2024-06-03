using UnityEngine;

namespace BehaviourTree
{
    public class CooldownCondition : Condition
    {
        #region Variables

        private string variableName = string.Empty;

        private float cooldown = 0f;

        #endregion Variables

        #region Constructor

        public CooldownCondition(string variableName, float cooldown, Enemy controller) : base(controller)
        {
            this.variableName = variableName;
            this.cooldown = cooldown;
        }

        #endregion Constructor

        #region Methods

        protected override bool CheckCondition(Blackboard blackboard)
        {
            if (!blackboard.TryGetVariable(variableName, out FloatVariable variable))
            {
                variable = new FloatVariable();
                blackboard.AddVariable(variableName, variable);

                return false;
            }

            if (variable.Value < cooldown)
            {
                variable.Value += Time.deltaTime;
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion Methods
    }
}