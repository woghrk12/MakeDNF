using UnityEngine;

public partial class Meteor_FireHero
{
    public class Charging : SkillState
    {
        #region Variables

        private Meteor_FireHero stateController = null;

        private bool isCharging = true;
        private float chargingTime = 0.2f;
        private int countCharging = 0;
        private int maxCountCharging = 5;

        #endregion Variables

        #region Constructor

        public Charging(BehaviourController character, Skill stateController) : base(character, stateController)
        {
            this.stateController = stateController as Meteor_FireHero;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.METEOR);
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override void OnStart()
        {
            timer = 0f;

            isCharging = true;
            countCharging = 0;

            character.Animator.SetTrigger(stateHash);
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if (character.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) return;

            if (isCharging && countCharging < maxCountCharging)
            {
                if (timer < chargingTime) return;

                stateController.sizeEff += 0.2f;
                countCharging++;
                timer = 0f;
            }
            else
            {
                stateController.SetState((int)EState.SHOT);
            }
        }

        public override void OnCancel()
        {
            character.Animator.SetTrigger(cancelHash);
        }

        public override void OnSkillButtonReleased()
        {
            isCharging = false;
        }

        #endregion Override

        #endregion Methods
    }
}