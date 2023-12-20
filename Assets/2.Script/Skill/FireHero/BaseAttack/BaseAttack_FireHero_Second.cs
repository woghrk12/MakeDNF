using System.Collections;
using UnityEngine;

public partial class BaseAttack_FireHero
{
    public class Second : SkillState
    {
        #region Variables

        private BaseAttack_FireHero stateController = null;

        private int stateHash = 0;

        private bool isBlockKey = true;

        private WaitUntil cachedWaitUntil = null;

        #endregion Variables

        #region Constructor

        public Second(Skill skill) : base(skill)
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
            DNFTransform characterTransform = stateController.character.DNFTransform;
            GameManager.ObjectPool.SpawnFromPool("Fireball_2").GetComponent<Projectile>().Shot(characterTransform.Position, characterTransform.IsLeft);

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
