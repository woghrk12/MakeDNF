using UnityEngine;

namespace GroundMonkSkill
{
    public partial class Sway
    {
        public class Dash : SkillState
        {
            #region Variables

            private Sway stateController = null;

            [Header("Variables for dash during the skill")]
            private float dashSpeed = 7f;
            private Vector3 dashDirection = Vector3.zero;

            #endregion Variables

            #region Constructor

            public Dash(Character character, Sway stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.SWAY);

                preDelay = 1f / 6f;
                duration = 5f / 6f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                character.CanMove = false;
                character.CanJump = false;

                dashDirection = Vector3.zero;

                phase = EStatePhase.PREDELAY;

                GameManager.Input.AddMovementDelegate(OnJoystickMoved);
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (!animatorStateInfo.IsName("Sway")) return;
                        if (animatorStateInfo.normalizedTime < preDelay) return;

                        GameManager.Input.RemoveMovementDelegate(OnJoystickMoved);

                        character.HitboxState = EHitboxState.INVINCIBILITY;

                        phase = EStatePhase.MOTIONINPROGRESS;

                        break;

                    case EStatePhase.MOTIONINPROGRESS:
                        if (animatorStateInfo.normalizedTime < duration) return;

                        character.HitboxState = EHitboxState.NONE;

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

                float dashRatio = EaseHelper.EaseInQuart(1f, 0f, animatorStateInfo.normalizedTime);
                Vector3 dashDirection = Time.fixedDeltaTime * dashRatio * dashSpeed * this.dashDirection;
                
                if (character.DNFRigidbody.enabled)
                {
                    character.DNFRigidbody.MoveDirection(dashDirection);
                }
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
                GameManager.Input.RemoveMovementDelegate(OnJoystickMoved);

                phase = EStatePhase.NONE;

                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetTrigger(cancelHash);
            }

            #endregion Override

            /// <summary>
            /// The event method called when the player control the joystick during the skill.
            /// </summary>
            /// <param name="direction">The direction vector received through the joystick</param>
            public void OnJoystickMoved(Vector3 direction)
            {
                if (phase != EStatePhase.PREDELAY) return;

                dashDirection = (character.DNFTransform.IsLeft ? Vector3.right : Vector3.left) + new Vector3(0f, 0f, direction.z);
            }

            #endregion Methods
        }
    }
}