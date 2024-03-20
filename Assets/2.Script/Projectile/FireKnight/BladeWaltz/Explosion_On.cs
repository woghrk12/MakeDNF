using UnityEngine;

namespace FireKnightSkill.BladeWaltzProjectile
{
    public partial class Explosion
    {
        public class On : ProjectileState
        {
            #region Variables

            private Explosion stateController = null;

            private int stateHash = 0;

            #endregion Variables

            #region Constructor 

            public On(Explosion stateController) : base(stateController)
            {
                this.stateController = stateController;

                stateHash = Animator.StringToHash(AnimatorKey.Projectile.SHOT);
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                stateController.animator.SetTrigger(stateHash);

                stateController.AlreadyHitTargets.Clear();

                stateController.AttackerHitboxController.EnableHitbox((int)EState.ON);

                GameManager.Camera.ShakeCamera(1f);
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = stateController.animator.GetCurrentAnimatorStateInfo(0);

                if (!animatorStateInfo.IsName("On")) return;
                if (animatorStateInfo.normalizedTime < 1f) return;

                OnComplete();
            }

            public override void OnLateUpdate()
            {
                stateController.CalculateOnHit(GameManager.Room.Monsters);
            }

            public override void OnComplete()
            {
                stateController.AttackerHitboxController.DisableHitbox();

                stateController.Complete();
            }

            #endregion Override

            #endregion Methods
        }
    }
}
