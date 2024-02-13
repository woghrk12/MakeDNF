using UnityEngine;

namespace FireKnightSkill
{
    public partial class Dodge
    {
        public class On : SkillState
        {
            #region Variables

            private Dodge stateController = null;

            [Header("Variables for dodge dash during the skill")]
            private float dodgeSpeed = 15f;
            private Vector3 dodgeDirection = Vector3.zero;

            #endregion Variables

            #region Constructor

            public On(Character character, Dodge stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.DODGE);
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                character.CanMove = false;
                character.CanJump = false;

                dodgeDirection = character.DNFTransform.IsLeft ? Vector3.left : Vector3.right;
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                if (!animatorStateInfo.IsName("Dodge")) return;
                if (animatorStateInfo.normalizedTime < 1f) return;

                OnComplete();
            }

            public override void OnFixedUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                if (!animatorStateInfo.IsName("Dodge")) return;

                float dodgeSpeedRatio = EaseHelper.EaseInSine(1f, 0f, animatorStateInfo.normalizedTime);
                Vector3 dodgeDirection = Time.fixedDeltaTime * dodgeSpeedRatio * dodgeSpeed * this.dodgeDirection;

                character.DNFRigidbody.MoveDirection(dodgeDirection);
            }

            public override void OnComplete()
            {
                phase = EStatePhase.NONE;

                character.CanMove = true;
                character.CanJump = true;

                stateController.OnComplete();
            }

            public override void OnCancel()
            {
                phase = EStatePhase.NONE;

                character.Animator.ResetTrigger(skillHash);
            }

            #endregion Override

            #endregion Methods
        }
    }
}
