using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Meteor_FireHero
{
    public class Shot : SkillState
    {
        #region Variables

        private Meteor_FireHero stateController = null;

        private Animator characterAnimator = null;
        private DNFTransform characterTransform = null;

        private int stateHash = 0;

        private float preDelay = 0f;
        private float postDelay = 0f;

        #endregion Variables

        #region Constructor

        public Shot(Skill stateController, Character character) : base(stateController, character)
        {
            this.stateController = stateController as Meteor_FireHero;

            characterAnimator = character.Animator;
            characterTransform = character.DNFTransform;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.METEOR);

            preDelay = Time.deltaTime * 4f * 4f;
            postDelay = Time.deltaTime * 3f * 3f;
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
            GameManager.ObjectPool.SpawnFromPool("Meteor_FireHero").GetComponent<Projectile>().Shot(characterTransform, stateController.sizeEff);

            // Post-delay
            yield return Utilities.WaitForSeconds(postDelay);
        }

        #endregion Override

        #endregion Methods
    }
}
