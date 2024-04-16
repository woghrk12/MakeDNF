using UnityEngine;

namespace GroundMonkSkill
{
    public partial class BaseAttack
    {
        public class Third : SkillState
        {
            #region Variables

            private BaseAttack stateController = null;

            /// <summary>
            /// The flag indicating whether to continuously use the basic attack.
            /// </summary>
            private bool isContinue = false;

            [Header("Variables for dash during the skill")]
            [SerializeField] private float dashSpeed = 0.05f;
            private Vector3 dashDirection = Vector3.zero;

            #endregion Variables

            #region Constructor

            public Third(Character character, Skill stateController) : base(character, stateController)
            {
                this.stateController = stateController as BaseAttack;

                skillHash = Animator.StringToHash(AnimatorKey.Character.BASE_ATTACK);

                duration = 2f / 3f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                isContinue = false;
                dashDirection = Vector3.zero;

                phase = EStatePhase.PREDELAY;

                stateController.AlreadyHitTargets.Clear();

                GameManager.Input.AddMovementDelegate(OnJoystickMoved);
                GameManager.Input.AddButtonDownDelegate(stateController.keyName, OnSkillButtonPressed);
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (!animatorStateInfo.IsName("BaseAttack_3")) return;

                        stateController.AttackerHitboxController.EnableHitbox((int)EState.THIRD);

                        character.Animator.SetBool(continueHash, false);

                        character.AddEffect(EEffectList.Horizontal_Fist, true);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.HITBOXACTIVE:
                        if (animatorStateInfo.normalizedTime < duration) return;

                        stateController.AttackerHitboxController.DisableHitbox();

                        phase = EStatePhase.POSTDELAY;

                        break;

                    case EStatePhase.POSTDELAY:
                        if (animatorStateInfo.normalizedTime < 1f) return;

                        GameManager.Input.RemoveMovementDelegate(OnJoystickMoved);
                        GameManager.Input.RemoveButtonDownDelegate(stateController.keyName, OnSkillButtonPressed);

                        if (isContinue)
                        {
                            character.Animator.SetTrigger(skillHash);
                            stateController.SetState((int)EState.FOURTH);
                        }
                        else
                        {
                            OnComplete();
                        }

                        break;
                }
            }

            public override void OnFixedUpdate()
            {
                if (phase == EStatePhase.PREDELAY) return;

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

                character.Animator.SetBool(continueHash, false);

                character.CanMove = true;
                character.CanJump = true;

                stateController.OnComplete();
            }

            public override void OnCancel()
            {
                GameManager.Input.RemoveMovementDelegate(OnJoystickMoved);
                GameManager.Input.RemoveButtonDownDelegate(stateController.keyName, OnSkillButtonPressed);

                if (stateController.AttackerHitboxController.IsHitboxActivated)
                {
                    stateController.AttackerHitboxController.DisableHitbox();
                }

                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetBool(continueHash, false);

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

                if (character.DNFTransform.IsLeft)
                {
                    dashDirection = (direction.x > 0f ? Vector3.zero : Vector3.left) * dashSpeed;
                }
                else
                {
                    dashDirection = (direction.x < 0f ? Vector3.zero : Vector3.right) * dashSpeed;
                }
            }

            /// <summary>
            /// The event method called when the player press the button associated with the skill.
            /// </summary>
            public void OnSkillButtonPressed()
            {
                isContinue = true;
                character.Animator.SetBool(continueHash, true);
            }

            #endregion Methods
        }
    }
}
