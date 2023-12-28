using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ScatterFlame_FireHero
{
    public class Scatter : SkillState
    {
        #region Variables

        private ScatterFlame_FireHero stateController = null;

        #endregion Variables

        #region Constructor

        public Scatter(BehaviourController character, Skill stateController) : base(character, stateController)
        {
            this.stateController = stateController as ScatterFlame_FireHero;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.SCATTER_FLAME);

            preDelay = Time.deltaTime * 4f * 4f;
            postDelay = Time.deltaTime * 1f * 4f;
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override void OnStart()
        {
            timer = 0f;
            isPreDelay = true;
            isPostDelay = false;

            character.Animator.SetTrigger(stateHash);
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if (isPreDelay)
            {
                if (timer < preDelay) return;

                // Instantiate the projectile
                GameManager.ObjectPool.SpawnFromPool("Side_Flame_FireHero").GetComponent<Projectile>().Shot(character.DNFTransform);

                isPreDelay = false;
                timer = 0f;
            }
            else if (isPostDelay)
            {
                if (timer < postDelay) return;

                OnComplete();
            }
            else
            {
                if (character.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) return;

                isPostDelay = true;
                timer = 0f;
            }
        }

        public override void OnComplete()
        {
            stateController.OnComplete();
        }

        #endregion Override

        #endregion Methods
    }
}
