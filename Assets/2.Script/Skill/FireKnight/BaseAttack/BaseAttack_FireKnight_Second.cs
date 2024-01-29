using UnityEngine;

public partial class BaseAttack_FireKnight
{
    public class Second : SkillState
    {
        #region Variables

        private BaseAttack_FireKnight stateController = null;

        private bool isContinue = false;
        private bool isBlockKey = true;

        #endregion Variables

        #region Constructor

        public Second(Character character, Skill stateController) : base(character, stateController)
        {
            this.stateController = stateController as BaseAttack_FireKnight;

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

            stateController.alreadyHitObjects.Clear();

            attackSpeed = character.Animator.GetFloat(attackSpeedHash);
        }

        public override void OnUpdate()
        {
            AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);
            
            switch (phase)
            {
                case EStatePhase.PREDELAY:
                    if (!animatorStateInfo.IsName("BaseAttack_2")) return;
                    if (animatorStateInfo.normalizedTime < preDelay) return;

                    stateController.AttackHitboxController.EnableHitbox((int)EState.SECOND);

                    isBlockKey = false;

                    character.Animator.SetBool(continueHash, false);

                    phase = EStatePhase.HITBOXACTIVE;

                    break;

                case EStatePhase.HITBOXACTIVE:
                    stateController.AttackHitboxController.CalculateHitbox();

                    if (!animatorStateInfo.IsName("BaseAttack_2")) return;
                    if(animatorStateInfo.normalizedTime < 1f) return;

                    stateController.AttackHitboxController.DisableHitbox();

                    character.Animator.SetTrigger(skillHash);

                    phase = EStatePhase.POSTDELAY;

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

            character.Animator.SetBool(continueHash, false);
            character.Animator.SetFloat(attackSpeedHash, attackSpeed);

            character.CanMove = true;
            character.CanJump = true;

            stateController.OnComplete();
        }

        public override void OnCancel()
        {
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
