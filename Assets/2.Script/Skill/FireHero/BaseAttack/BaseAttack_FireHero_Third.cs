using System.Collections;
using UnityEngine;

public partial class BaseAttack_FireHero
{
    public class Third : SkillState
    {
        #region Variables

        private Animator characterAnimator = null;
        private DNFTransform characterTransform = null;

        private int stateHash = 0;

        private float preDelay = 0f;
        private float postDelay = 0f;

        #endregion Variables

        #region Constructor

        public Third(Skill stateController, Character character) : base(stateController, character)
        {
            characterAnimator = character.Animator;
            characterTransform = character.DNFTransform;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.BASE_ATTACK);

            preDelay = Time.deltaTime * 2f * 4f;
            postDelay = Time.deltaTime * 5f * 4f;
        }

        #endregion Constructor

        #region Methods

        #region Override 

        public override IEnumerator Activate()
        {
            characterAnimator.SetTrigger(stateHash);

            // Pre-delay
            yield return Utilities.WaitForSeconds(preDelay);

            // Instantiate the projectile
            GameManager.ObjectPool.SpawnFromPool("Fireball_1_FireHero").GetComponent<Projectile>().Shot(characterTransform);

            // Post-delay
            yield return Utilities.WaitForSeconds(postDelay);
        }

        #endregion Override 

        #endregion Methods
    }
}
