using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Meteor_FireHero
{
    public class Charging : SkillState
    {
        #region Variables

        private Meteor_FireHero stateController = null;

        private int stateHash = 0;

        private float preDelay = 0f;

        private bool isCharging = true;
        private int countCharging = 0;

        #endregion Variables

        #region Constructor

        public Charging(Skill skill) : base(skill)
        {
            stateController = skill as Meteor_FireHero;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.METEOR);

            preDelay = Time.deltaTime * 4f * 4f;
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override IEnumerator Activate()
        {
            stateController.animator.SetTrigger(stateHash);

            isCharging = true;
            countCharging = 0;

            // Pre-delay
            yield return Utilities.WaitForSeconds(preDelay);

            // Charging
            while (isCharging && countCharging < 5)
            {
                yield return Utilities.WaitForSeconds(0.2f);

                stateController.sizeEff += 0.2f;
                countCharging++;
            }
        }

        public override void OnReleased()
        {
            isCharging = false;
        }

        #endregion Override

        #endregion Methods
    }
}