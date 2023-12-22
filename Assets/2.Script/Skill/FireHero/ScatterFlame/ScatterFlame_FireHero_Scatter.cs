using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ScatterFlame_FireHero
{
    public class Scatter : SkillState
    {
        #region Variables

        private ScatterFlame_FireHero stateController = null;

        private int stateHash = 0;

        private float preDelay = 0f;
        private float postDelay = 0f;

        #endregion Variables

        #region Constructor

        public Scatter(Skill skill) : base(skill)
        {
            stateController = skill as ScatterFlame_FireHero;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.SCATTER_FLAME);

            preDelay = Time.deltaTime * 4f * 4f;
            postDelay = Time.deltaTime * 4f * 4f;
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override IEnumerator Activate()
        {
            stateController.animator.SetTrigger(stateHash);

            // Pre-delay
            yield return Utilities.WaitForSeconds(preDelay);

            // Instantiate the projectile
            DNFTransform characterTransform = stateController.character.DNFTransform;
            GameManager.ObjectPool.SpawnFromPool("Side_Flame_FireHero").GetComponent<Projectile>().Shot(characterTransform.Position, characterTransform.IsLeft);

            // Post-delay 
            yield return Utilities.WaitForSeconds(postDelay);
        }

        #endregion Override

        #endregion Methods
    }
}
