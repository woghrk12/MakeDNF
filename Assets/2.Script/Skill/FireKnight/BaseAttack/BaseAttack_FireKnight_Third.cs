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

            preDelay = 3f / 8f;
            postDelay = 1f;
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override void OnStart()
        {
            phase = EStatePhase.PREDELAY;

            stateController.alreadyHitObjects.Clear();
        }

        public override void OnUpdate()
        {
            AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

            switch (phase)
            {
                case EStatePhase.PREDELAY:
                    if (!animatorStateInfo.IsName("BaseAttack_3")) return;
                    if (animatorStateInfo.normalizedTime < preDelay) return;

                    // TODO : Instantiate the projectile object

                    phase = EStatePhase.POSTDELAY;

                    break;

                case EStatePhase.POSTDELAY:
                    if (animatorStateInfo.normalizedTime < postDelay) return;

                    OnComplete();
                    
                    break;
            }
        }

        public override void OnComplete()
        {
            stateController.OnComplete();
        }

        public override void OnCancel()
        {
            character.Animator.SetTrigger(cancelHash);
        }

        #endregion Override

        #endregion Methods
    }
}
