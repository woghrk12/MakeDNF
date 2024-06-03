using UnityEngine;

namespace BehaviourTree
{
    public abstract class Action : Node
    {
        #region Variables

        protected int animHash = 0;

        #endregion Variables

        #region Constructor

        public Action(Enemy controller) : base(controller) { }

        #endregion Constructor

        #region Methods

        protected bool CheckAnimationRunning(string animName)
        {
            if (ReferenceEquals(controller, null)) return false;
            if (ReferenceEquals(controller.Animator, null)) return false;

            AnimatorStateInfo animatorStateInfo = controller.Animator.GetCurrentAnimatorStateInfo(0);

            return animatorStateInfo.IsName(animName) && animatorStateInfo.normalizedTime < 1f;
        }

        #endregion Methods
    }
}