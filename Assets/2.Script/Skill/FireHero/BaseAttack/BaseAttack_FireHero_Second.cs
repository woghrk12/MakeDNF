using System.Collections;
using UnityEngine;

public partial class BaseAttack_FireHero
{
    public class Second : SkillState
    {
        #region Variables

        private BaseAttack_FireHero stateController = null;

        private bool isContinue = false;
        private bool isBlockKey = true;


        #endregion Variables

        #region Constructor

        public Second(BehaviourController character, Skill stateController) : base(character, stateController)
        {
            this.stateController = stateController as BaseAttack_FireHero;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.BASE_ATTACK);

            preDelay = Time.deltaTime * 2f * 4f;
            postDelay = Time.deltaTime * 1f * 4f;
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override void OnStart()
        {
            isContinue = false;
            isBlockKey = true;

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
                GameManager.ObjectPool.SpawnFromPool("Fireball_2_FireHero").GetComponent<Projectile>().Shot(character.DNFTransform);

                isBlockKey = false;

                isPreDelay = false;
                timer = 0f;
            }
            else if (isPostDelay)
            {
                if (isContinue)
                {
                    stateController.SetState((int)EState.THIRD);
                }
                else if (timer >= postDelay)
                {
                    OnComplete();
                }
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

        public override void OnPressed()
        {
            if (isBlockKey) return;

            isContinue = true;
        }

        #endregion Override

        #endregion Methods
    }
}
