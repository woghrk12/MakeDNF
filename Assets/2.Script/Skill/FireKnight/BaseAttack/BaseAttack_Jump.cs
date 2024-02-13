using UnityEngine;

namespace FireKnightSkill
{
    public partial class BaseAttack
    {
        public class Jump : SkillState
        {
            #region Variables

            private BaseAttack stateController = null;

            #endregion Variables

            #region Constructor

            public Jump(Character character, Skill stateController) : base(character, stateController)
            {
                this.stateController = stateController as BaseAttack;

                skillHash = Animator.StringToHash(AnimatorKey.Character.BASE_ATTACK);

                preDelay = 3f / 8f;
                duration = 5f / 8f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                character.CanLookBack = false;

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
                        if (!animatorStateInfo.IsName("BaseAttack_Jump")) return;
                        if (animatorStateInfo.normalizedTime < preDelay) return;

                        stateController.AttackHitboxController.EnableHitbox((int)EState.JUMP);

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

                        character.DNFRigidbody.enabled = true;
                        character.CanMove = true;
                        character.Animator.SetFloat(attackSpeedHash, attackSpeed);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.MOTIONINPROGRESS:
                        if (animatorStateInfo.normalizedTime < 1f) return;

                        OnComplete();

                        break;
                }
            }

            public override void OnFixedUpdate()
            {
                if (!character.DNFRigidbody.IsGround) return;

                OnComplete();
            }

            public override void OnLateUpdate()
            {
                if (!stateController.AttackHitboxController.IsHitboxActivated) return;

                if (stateController.CalculateOnHit(GameManager.Room.Monsters))
                {
                    // Stiffness effect
                    character.DNFRigidbody.enabled = false;
                    character.CanMove = false;
                    character.Animator.SetFloat(attackSpeedHash, 0f);
                    stiffnessTimer = 0f;
                    phase = EStatePhase.STOPMOTION;

                    // TODO : Spawn hit effects
                }
            }

            public override void OnComplete()
            {
                phase = EStatePhase.NONE;

                if (stateController.AttackHitboxController.IsHitboxActivated)
                {
                    stateController.AttackHitboxController.DisableHitbox();
                }

                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetFloat(attackSpeedHash, attackSpeed);

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
                character.Animator.SetTrigger(cancelHash);
                character.Animator.SetFloat(attackSpeedHash, attackSpeed);
            }

            #endregion Override

            #endregion Methods
        }
    }
}