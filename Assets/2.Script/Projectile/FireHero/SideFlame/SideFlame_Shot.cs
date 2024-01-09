using UnityEngine;

public partial class SideFlame
{
    public class Shot : ProjectileState
    {
        #region Variables

        private SideFlame stateController = null;

        #endregion Variables

        #region Constructor

        public Shot(Projectile stateController) : base(stateController)
        {
            this.stateController = stateController as SideFlame;
        }

        #endregion Constructor

        #region Methods

        public override void OnStart()
        {
            stateController.alreadyHitObjects.Clear();
        }

        public override void OnUpdate()
        {
            if (stateController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) return;

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
