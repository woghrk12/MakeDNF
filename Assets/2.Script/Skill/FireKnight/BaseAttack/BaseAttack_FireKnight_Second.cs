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
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override void OnStart()
        {
            isContinue = false;
            isBlockKey = false;

            phase = EStatePhase.PREDELAY;

            stateController.alreadyHitObjects.Clear();
        }

        public override void OnUpdate()
        {
            AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

            switch (phase)
            {
                case EStatePhase.PREDELAY:
                    if (!animatorStateInfo.IsName("BaseAttack_2")) return;

                    stateController.AttackHitboxController.EnableHitbox((int)EState.SECOND);

                    phase = EStatePhase.HITBOXACTIVE;

                    break;

                case EStatePhase.HITBOXACTIVE:
                    if(animatorStateInfo.IsName("BaseAttack_2")) return;

                    stateController.AttackHitboxController.DisableHitbox();

                    isBlockKey = true;

                    phase = EStatePhase.MOTIONINPROGRESS;

                    break;

                case EStatePhase.MOTIONINPROGRESS:
                    if (animatorStateInfo.IsName("BaseAttack_2_End")) return;
                    if (animatorStateInfo.normalizedTime < 1f) return;

                    if (isContinue)
                    {
                        stateController.SetState((int)EState.THIRD);
                    }
                    else
                    {
                        phase = EStatePhase.POSTDELAY;
                    }

                    break;

                case EStatePhase.POSTDELAY:
                    if (!animatorStateInfo.IsName("BaseAttack_2_PostDelay")) return;
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
                // TODO : Spawn hit effects
            }
        }

        public override void OnComplete()
        {
            stateController.OnComplete();
        }

        public override void OnCancel()
        {
            character.Animator.SetTrigger(cancelHash);
            character.Animator.ResetTrigger(skillHash);
        }

        public override void OnSkillButtonPressed()
        {
            if (isBlockKey) return;

            isContinue = true;
            character.Animator.SetTrigger(skillHash);
        }

        #endregion Override

        #endregion Methods
    }
}
