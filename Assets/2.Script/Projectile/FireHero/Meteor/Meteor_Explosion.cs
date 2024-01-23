using UnityEngine;

public partial class Meteor
{
    public class Explosion : ProjectileState
    {
        #region Variables
        
        private Meteor stateController = null;

        private int stateHash = 0;

        #endregion Variables

        #region Constructor

        public Explosion(Projectile stateController) : base(stateController)
        {
            this.stateController = stateController as Meteor;

            stateHash = Animator.StringToHash(AnimatorKey.Projectile.SHOT);
        }

        #endregion Constructor

        #region Methods

        public override void OnStart()
        {
            stateController.dnfTransform.Y = 0f;

            stateController.animator.SetTrigger(stateHash);

            stateController.alreadyHitObjects.Clear();

            stateController.AttackHitboxController.EnableHitbox((int)EState.EXPLOSION);
        }

        public override void OnUpdate()
        {
            var animationStateInfo = stateController.animator.GetCurrentAnimatorStateInfo(0);

            if (!animationStateInfo.IsName("Explosion")) return;
            if (animationStateInfo.normalizedTime < 1f) return;
            
            OnComplete();
        }

        public override void OnLateUpdate()
        {
            stateController.AttackHitboxController.CalculateHitbox();

            if (stateController.CalculateOnHit(GameManager.Room.Monsters))
            { 
                // Spawn hit effect
            }
        }

        public override void OnComplete()
        {
            stateController.Complete();
        }

        #endregion Methods
    }
}
