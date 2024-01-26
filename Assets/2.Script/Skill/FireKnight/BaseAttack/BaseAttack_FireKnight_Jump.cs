using UnityEngine;

public partial class BaseAttack_FireKnight
{
    public class Jump : SkillState
    {
        #region Variables

        private BaseAttack_FireKnight stateController = null;

        #endregion Variables

        #region Constructor

        public Jump(Character character, Skill stateController) : base(character, stateController)
        {
            this.stateController = stateController as BaseAttack_FireKnight;

            preDelay = 2f / 8f;
            duration = 5f / 8f;
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override void OnStart()
        {
            phase = EStatePhase.PREDELAY;

            stateController.alreadyHitObjects.Clear();

            character.CanLookBack = false;
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

            stateController.OnComplete();
        }

        public override void OnCancel()
        {
            phase = EStatePhase.NONE;

            if (stateController.AttackHitboxController.IsHitboxActivated)
            {
                stateController.AttackHitboxController.DisableHitbox();
            }

            character.Animator.SetTrigger(cancelHash);
        }

        #endregion Override

        #endregion Methods
    }
}