using UnityEngine;

namespace FireKnightSkill
{
    public partial class BaseAttack
    {
        public class Second : SkillState
        {
            #region Variables

            private BaseAttack stateController = null;

            /// <summary>
            /// The flag indicating whether to continuously use the basic attack.
            /// </summary>
            private bool isContinue = false;

            /// <summary>
            /// The flag indicating whether to block player's key input.
            /// </summary>
            private bool isBlockKey = true;

            #endregion Variables

            #region Constructor

            public Second(Character character, Skill stateController) : base(character, stateController)
            {
                this.stateController = stateController as BaseAttack;

                skillHash = Animator.StringToHash(AnimatorKey.Character.BASE_ATTACK);

                preDelay = 1f / 3f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                isContinue = false;
                isBlockKey = true;

                phase = EStatePhase.PREDELAY;

                stateController.AlreadyHitTargets.Clear();
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (!animatorStateInfo.IsName("BaseAttack_2")) return;
                        if (animatorStateInfo.normalizedTime < preDelay) return;

                        stateController.AttackerHitboxController.EnableHitbox((int)EState.SECOND);

                        isBlockKey = false;

                        character.Animator.SetBool(continueHash, false);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.HITBOXACTIVE:
                        stateController.AttackerHitboxController.CalculateHitbox();

                        if (!animatorStateInfo.IsName("BaseAttack_2")) return;
                        if (animatorStateInfo.normalizedTime < 1f) return;

                        stateController.AttackerHitboxController.DisableHitbox();

                        character.Animator.SetTrigger(skillHash);

                        phase = EStatePhase.POSTDELAY;

                        break;

                    case EStatePhase.POSTDELAY:
                        if (!animatorStateInfo.IsName("BaseAttack_2_End")) return;
                        if (animatorStateInfo.normalizedTime < 1f) return;

                        if (isContinue)
                        {
                            character.Animator.SetTrigger(skillHash);
                            stateController.SetState((int)EState.THIRD);
                        }
                        else
                        {
                            OnComplete();
                        }

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
                phase = EStatePhase.NONE;

                character.Animator.SetBool(continueHash, false);

                character.CanMove = true;
                character.CanJump = true;

                stateController.OnComplete();
            }

            public override void OnCancel()
            {
                if (stateController.AttackerHitboxController.IsHitboxActivated)
                {
                    stateController.AttackerHitboxController.DisableHitbox();
                }

                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetBool(continueHash, false);

                character.Animator.SetTrigger(cancelHash);
            }

            public override void OnSkillButtonPressed()
            {
                if (isBlockKey) return;

                isContinue = true;
                character.Animator.SetBool(continueHash, true);
            }

            #endregion Override

            #endregion Methods
        }
    }
}
