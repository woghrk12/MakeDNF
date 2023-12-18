using System.Collections;
using UnityEngine;

public partial class BaseAttack_FireHero
{
    public class Third : SkillState
    {
        #region Variables

        private BaseAttack_FireHero stateController = null;

        private int stateHash = 0;

        private Projectile projectile = null;

        private WaitUntil cachedWaitUntil = null;

        #endregion Variables

        #region Constructor

        public Third(Skill skill) : base(skill)
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
            stateController.animator.SetTrigger(stateHash);

            // Pre-delay
            yield return Utilities.WaitForSeconds(0.1f);

            // Instantiate the projectile

            // Post-delay
            yield return cachedWaitUntil;
        }

        #endregion Override 

        #endregion Methods
    }
}
