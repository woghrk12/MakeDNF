public partial class FlameStrike
{
    public class Shot : ProjectileState
    {
        #region Variables

        private FlameStrike stateController = null;

        #endregion Variables

        #region Constructor

        public Shot(Projectile stateController) : base(stateController)
        {
            this.stateController = stateController as FlameStrike;
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