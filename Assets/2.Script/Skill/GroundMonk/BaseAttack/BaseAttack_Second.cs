using UnityEngine;

namespace GroundMonkSkill
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

            #endregion Variables

            #region Constructor

            public Second(Character character, Skill stateController) : base(character, stateController)
            {
                this.stateController = stateController as BaseAttack;

                skillHash = Animator.StringToHash(AnimatorKey.Character.BASE_ATTACK);

                duration = 2f / 3f;
                postDelay = 1f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                isContinue = false;

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
                        
                        stateController.AttackerHitboxController.EnableHitbox((int)EState.SECOND);

                        character.Animator.SetBool(continueHash, false);

                        character.AddEffect(EEffectList.Straight_Fist_2, true);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.HITBOXACTIVE:
                        stateController.AttackerHitboxController.CalculateHitbox();

                        if (animatorStateInfo.normalizedTime < duration) return;

                        stateController.AttackerHitboxController.DisableHitbox();

                        phase = EStatePhase.POSTDELAY;

                        break;

                    case EStatePhase.POSTDELAY:
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
                isContinue = true;
                character.Animator.SetBool(continueHash, true);
            }

            #endregion Override

            #endregion Methods
        }
    }
}
