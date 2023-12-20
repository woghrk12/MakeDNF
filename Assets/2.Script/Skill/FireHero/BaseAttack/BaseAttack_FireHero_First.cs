using System.Collections;
using UnityEngine;

public partial class BaseAttack_FireHero
{
    public class First : SkillState
    {
        #region Variables

        private BaseAttack_FireHero stateController = null;

        private int stateHash = 0;

        private bool isBlockKey = true;

        private float preDelay = 0f;
        private float postDelay = 0f;

        #endregion Variables

        #region Constructor

        public First(Skill skill) : base(skill)
        {
            stateController = skill as BaseAttack_FireHero;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.BASE_ATTACK);

            preDelay = Time.deltaTime * 2f * 4f;
            postDelay = Time.deltaTime * 4f * 4f;
        }

        #endregion Constructor

        #region Methods

        #region Override 

        public override IEnumerator Activate()
        {
            isBlockKey = true;
            stateController.animator.SetTrigger(stateHash);

            // Pre-delay
            yield return Utilities.WaitForSeconds(preDelay);

            isBlockKey = false;

            // Instantiate the projectile
            DNFTransform characterTransform = stateController.character.DNFTransform;
            GameManager.ObjectPool.SpawnFromPool("Fireball_2").GetComponent<Projectile>().Shot(characterTransform.Position, characterTransform.IsLeft);

            // Post-delay
            yield return Utilities.WaitForSeconds(postDelay);
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
