using System.Collections;
using UnityEngine;

public partial class BaseAttack_FireHero
{
    public class First : SkillState
    {
        #region Variables

        private BaseAttack_FireHero stateController = null;

        private Animator characterAnimator = null;
        private DNFTransform characterTransform = null;

        private int stateHash = 0;

        private bool isBlockKey = true;

        private float preDelay = 0f;
        private float postDelay = 0f;

        #endregion Variables

        #region Constructor

        public First(Skill stateController, Character character) : base(stateController, character)
        {
            this.stateController = stateController as BaseAttack_FireHero;

            characterAnimator = character.Animator;
            characterTransform = character.DNFTransform;

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
            
            characterAnimator.SetTrigger(stateHash);

            // Pre-delay
            yield return Utilities.WaitForSeconds(preDelay);

            isBlockKey = false;

            // Instantiate the projectile
            GameManager.ObjectPool.SpawnFromPool("Fireball_2_FireHero").GetComponent<Projectile>().Shot(characterTransform);

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
