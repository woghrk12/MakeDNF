using UnityEngine;

public partial class FlameStrike_FireHero
{
    public class Shot : SkillState
    {
        #region Variables

        private FlameStrike_FireHero stateController = null;

        #endregion Variables

        #region Constructor

        public Shot(BehaviourController character, Skill stateController) : base(character, stateController)
        {
            this.stateController = stateController as FlameStrike_FireHero;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.FLAME_STRIKE);

            preDelay = Time.deltaTime * 2f * 4f;
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
                GameManager.ObjectPool.SpawnFromPool(EObjectPoolList.Flame_Strike_FireHero).GetComponent<Projectile>().Shot(stateController.aimTransform);
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

        public override void OnCancel()
        {
            character.Animator.SetTrigger(cancelHash);
        }

        #endregion Override

        #endregion Methods
    }
}
