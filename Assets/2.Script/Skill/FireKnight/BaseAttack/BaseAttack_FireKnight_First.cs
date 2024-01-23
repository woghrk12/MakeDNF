using UnityEngine;

public partial class BaseAttack_FireKnight
{
    public class First : SkillState
    {
        #region Variables

        private BaseAttack_FireKnight stateController = null;

        private bool isContinue = false;
        private bool isBlockKey = true;

        #endregion Variables

        #region Constructor

        public First(Character character, Skill stateController) : base(character, stateController)
        {
            this.stateController = stateController as BaseAttack_FireKnight;

            skillHash = Animator.StringToHash(AnimatorKey.Character.BASE_ATTACK);

            preDelay = 3f / 8f;
            duration = 7f / 8f;
            postDelay = 1f;
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
        }

        public override void OnUpdate()
        {
            AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

            switch (phase)
            {
                case EStatePhase.PREDELAY:
                    if (!animatorStateInfo.IsName("BaseAttack_1")) return;
                    if (animatorStateInfo.normalizedTime < preDelay) return;

                    stateController.AttackHitboxController.EnableHitbox((int)EState.FIRST);

                    isBlockKey = false;
                    character.Animator.SetBool(continueHash, false);

                    phase = EStatePhase.HITBOXACTIVE;

                    break;

                case EStatePhase.HITBOXACTIVE:
                    if (animatorStateInfo.normalizedTime < duration) return;

                    stateController.AttackHitboxController.DisableHitbox();

                    phase = EStatePhase.MOTIONINPROGRESS;

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
                // TODO : Spawn hit effects
            }
        }

        public override void OnComplete()
        {
            character.Animator.SetBool(continueHash, false);

            stateController.OnComplete();
        }

        public override void OnCancel()
        {
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
