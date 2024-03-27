using UnityEngine;

namespace GroundMonkSkill
{
    public partial class NuclearPunch
    {
        public class Miss : SkillState
        {
            #region Variables

            private NuclearPunch stateController = null;

            #endregion Variables

            #region Constructor

            public Miss(Character character, NuclearPunch stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.NUCLEAR_PUNCH);
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                if (!animatorStateInfo.IsName("NuclearPunch_Punch")) return;
                if (animatorStateInfo.normalizedTime < 1f) return;

                OnComplete();
            }

            public override void OnComplete()
            {
                character.CanMove = true;
                character.CanJump = true;

                stateController.OnComplete();
            }

            public override void OnCancel()
            {
                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetTrigger(cancelHash);
            }

            #endregion Override

            #endregion Methods
        }
    }
}
