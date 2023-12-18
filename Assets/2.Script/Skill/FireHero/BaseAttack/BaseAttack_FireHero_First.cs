using System.Collections;
using UnityEngine;

public partial class BaseAttack_FireHero
{
    public class First : SkillState
    {
        #region Variables

        private BaseAttack_FireHero stateController = null;

        private int stateHash = 0;

        private Projectile projectile = null;

        private bool isBlockKey = true;

        private WaitUntil cachedWaitUntil = null;

        #endregion Variables

        #region Constructor

        public First(Skill skill) : base(skill)
        {
            stateController = skill as BaseAttack_FireHero;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.BASE_ATTACK);

            cachedWaitUntil = new(() => stateController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        #endregion Constructor

        #region Methods

        #region Override 

        public override IEnumerator Activate()
        {
            isBlockKey = true;
            stateController.animator.SetTrigger(stateHash);

            // Pre-delay
            yield return Utilities.WaitForSeconds(0.1f);

            isBlockKey = false;

            // Instantiate the projectile

            // Post-delay
            yield return cachedWaitUntil;
        }

        public override void OnPressed()
        {
            if (isBlockKey) return;

            stateController.isContinue = true;
        }

        #endregion Override 

        #endregion Methods
    }
}
