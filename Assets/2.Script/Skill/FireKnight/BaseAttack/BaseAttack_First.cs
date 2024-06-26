using UnityEngine;

namespace FireKnightSkill
{
    public partial class BaseAttack
    {
        public class First : SkillState
        {
            #region Variables

            private BaseAttack stateController = null;

            /// <summary>
            /// The flag indicating whether to continuously use the basic attack.
            /// </summary>
            private bool isContinue = false;

            #endregion Variables

            #region Constructor

            public First(Character character, Skill stateController) : base(character, stateController)
            {
                this.stateController = stateController as BaseAttack;

                skillHash = Animator.StringToHash(AnimatorKey.Character.BASE_ATTACK);

                preDelay = 4f / 8f;
                duration = 6f / 8f;
                postDelay = 1f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                character.CanMove = false;
                character.CanJump = false;

                isContinue = false;

                phase = EStatePhase.PREDELAY;

                stateController.AlreadyHitTargets.Clear();

                GameManager.Input.AddButtonDownDelegate(stateController.keyName, OnSkillButtonPressed);
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (!animatorStateInfo.IsName("BaseAttack_1")) return;

                        stateController.AttackerHitboxController.EnableHitbox((int)EState.FIRST);

                        character.Animator.SetBool(continueHash, false);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.HITBOXACTIVE:
                        if (animatorStateInfo.normalizedTime < duration) return;

                        stateController.AttackerHitboxController.DisableHitbox();

                        phase = EStatePhase.MOTIONINPROGRESS;

                        break;

                    case EStatePhase.MOTIONINPROGRESS:
                        if (animatorStateInfo.IsName("BaseAttack_1") && animatorStateInfo.normalizedTime < 1f) return;

                        character.Animator.SetTrigger(skillHash);

                        if (isContinue)
                        {
                            stateController.SetState((int)EState.SECOND);
                        }
                        else
                        {
                            phase = EStatePhase.POSTDELAY;
                        }

                        break;

                    case EStatePhase.POSTDELAY:
                        if (!animatorStateInfo.IsName("BaseAttack_1_End")) return;
                        if (animatorStateInfo.normalizedTime < postDelay) return;

                        OnComplete();

                        break;
                }
            }

            public override void OnLateUpdate()
            {
                if (!stateController.AttackerHitboxController.IsHitboxActivated) return;

                stateController.CalculateOnHit(GameManager.Room.Monsters);
            }

            public override void OnComplete()
            {
                GameManager.Input.RemoveButtonDownDelegate(stateController.keyName, OnSkillButtonPressed);

                phase = EStatePhase.NONE;

                character.Animator.SetBool(continueHash, false);

                character.CanMove = true;
                character.CanJump = true;

                stateController.OnComplete();
            }

            public override void OnCancel()
            {
                GameManager.Input.RemoveButtonDownDelegate(stateController.keyName, OnSkillButtonPressed);

                phase = EStatePhase.NONE;

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
            /// The event method called when the player press the button associated with the skill.
            /// </summary>
            public void OnSkillButtonPressed()
            {
                if (phase != EStatePhase.HITBOXACTIVE && phase != EStatePhase.MOTIONINPROGRESS) return;

                isContinue = true;
                character.Animator.SetBool(continueHash, true);
            }

            #endregion Methods
        }
    }
}
