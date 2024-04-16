using UnityEngine;

namespace GroundMonkSkill
{
    public partial class StraightPunch 
    {
        public class Punch : SkillState
        {
            #region Variables

            private StraightPunch stateController = null;

            [Header("Variables for dash during the skill")]
            private float dashSpeed = 3f;
            private Vector3 dashDirection = Vector3.zero;

            #endregion Variables

            #region Constructor

            public Punch(Character character, StraightPunch stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.STRAIGHT_PUNCH);

                duration = 2f / 3f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                dashDirection = character.DNFTransform.IsLeft ? Vector3.left : Vector3.right;

                phase = EStatePhase.PREDELAY;

                stateController.AlreadyHitTargets.Clear();
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (!animatorStateInfo.IsName("StraightPunch")) return;

                        character.AddEffect(EEffectList.Straight_Fist_4, true);

                        stateController.AttackerHitboxController.EnableHitbox((int)EState.PUNCH);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.HITBOXACTIVE:
                        if (animatorStateInfo.normalizedTime < duration) return;

                        stateController.AttackerHitboxController.DisableHitbox();

                        phase = EStatePhase.POSTDELAY;

                        break;

                    case EStatePhase.POSTDELAY:
                        if (animatorStateInfo.normalizedTime < 1f) return;

                        OnComplete();

                        break;
                }
            }

            public override void OnFixedUpdate()
            {
                if (phase == EStatePhase.PREDELAY) return;

                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                float dashRatio = EaseHelper.EaseInSine(1f, 0f, animatorStateInfo.normalizedTime);
                Vector3 dashDirection = Time.fixedDeltaTime * dashRatio * dashSpeed * this.dashDirection;

                if (character.DNFRigidbody.enabled)
                {
                    character.DNFRigidbody.MoveDirection(dashDirection);
                }
            }

            public override void OnLateUpdate()
            {
                if (!stateController.AttackerHitboxController.IsHitboxActivated) return;

                stateController.CalculateOnHit(GameManager.Room.Monsters);
            }

            public override void OnComplete()
            {
                phase = EStatePhase.NONE;

                character.RemoveEffect(EEffectList.Straight_Fist_4);

                character.CanMove = true;
                character.CanJump = true;

                stateController.OnComplete();
            }

            public override void OnCancel()
            {
                phase = EStatePhase.NONE;

                if (stateController.AttackerHitboxController.IsHitboxActivated)
                {
                    stateController.AttackerHitboxController.DisableHitbox();
                }

                character.RemoveEffect(EEffectList.Straight_Fist_4);

                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetTrigger(cancelHash);
            }

            #endregion Override

            #endregion Methods
        }
    }
}