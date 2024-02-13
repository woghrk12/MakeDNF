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

            /// <summary>
            /// The flag indicating whether to block player's key input.
            /// </summary>
            private bool isBlockKey = true;

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
                isBlockKey = true;

                phase = EStatePhase.PREDELAY;

                stateController.alreadyHitObjects.Clear();

                attackSpeed = character.Animator.GetFloat(attackSpeedHash);
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (!animatorStateInfo.IsName("BaseAttack_1")) return;

                        stateController.AttackHitboxController.EnableHitbox((int)EState.FIRST);

                        isBlockKey = false;
                        character.Animator.SetBool(continueHash, false);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.HITBOXACTIVE:
                        stateController.AttackHitboxController.CalculateHitbox();

                        if (animatorStateInfo.normalizedTime < duration) return;

                        stateController.AttackHitboxController.DisableHitbox();

                        phase = EStatePhase.MOTIONINPROGRESS;

                        break;

                    case EStatePhase.STOPMOTION:
                        if (stiffnessTimer < stiffnessTime)
                        {
                            stiffnessTimer += Time.deltaTime;
                            return;
                        }

                        character.Animator.SetFloat(attackSpeedHash, attackSpeed);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.MOTIONINPROGRESS:
                        if (animatorStateInfo.IsName("BaseAttack_1") && animatorStateInfo.normalizedTime < 1f) return;

                        isBlockKey = true;
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
                if (!stateController.AttackHitboxController.IsHitboxActivated) return;

                if (stateController.CalculateOnHit(GameManager.Room.Monsters))
                {
                    // Stiffness effect
                    character.Animator.SetFloat(attackSpeedHash, 0f);
                    stiffnessTimer = 0f;
                    phase = EStatePhase.STOPMOTION;

                    // TODO : Spawn hit effects
                }
            }

            public override void OnComplete()
            {
                phase = EStatePhase.NONE;

                character.Animator.SetFloat(attackSpeedHash, attackSpeed);
                character.Animator.SetBool(continueHash, false);

                character.CanMove = true;
                character.CanJump = true;

                stateController.OnComplete();
            }

            public override void OnCancel()
            {
                phase = EStatePhase.NONE;

                if (stateController.AttackHitboxController.IsHitboxActivated)
                {
                    stateController.AttackHitboxController.DisableHitbox();
                }

                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetBool(continueHash, false);
                character.Animator.SetFloat(attackSpeedHash, attackSpeed);

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
