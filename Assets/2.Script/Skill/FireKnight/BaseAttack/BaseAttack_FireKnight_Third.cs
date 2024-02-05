using UnityEngine;

public partial class BaseAttack_FireKnight
{
    public class Third : SkillState
    {
        #region Variables

        private BaseAttack_FireKnight stateController = null;

        #endregion Variables

        #region Constructor

        public Third(Character character, Skill stateController) : base(character, stateController)
        {
            this.stateController = stateController as BaseAttack_FireKnight;

            preDelay = 2f / 8f;
            duration = 7f / 8f;
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override void OnStart()
        {
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
                    if (!animatorStateInfo.IsName("BaseAttack_3")) return;
                    if (animatorStateInfo.normalizedTime < preDelay) return;

                    stateController.AttackHitboxController.EnableHitbox((int)EState.THIRD);

                    phase = EStatePhase.HITBOXACTIVE;

                    break;

                case EStatePhase.HITBOXACTIVE:
                    stateController.AttackHitboxController.CalculateHitbox();

                    if (animatorStateInfo.normalizedTime < duration) return;

                    stateController.AttackHitboxController.DisableHitbox();

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
            character.CanMove = true;
            character.CanJump = true;

            character.Animator.SetBool(continueHash, false);

            stateController.OnComplete();
        }

        public override void OnCancel()
        {
            if (stateController.AttackHitboxController.IsHitboxActivated)
            {
                stateController.AttackHitboxController.DisableHitbox();
            }

            character.Animator.SetTrigger(cancelHash);
            character.Animator.SetBool(continueHash, false);
        }

        #endregion Override

        #endregion Methods
    }
}
