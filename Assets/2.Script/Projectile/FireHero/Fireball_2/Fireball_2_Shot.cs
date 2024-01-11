using UnityEngine;

public partial class Fireball_2
{
    public class Shot : ProjectileState
    {
        #region Variables

        private Fireball_2 stateController = null;

        private float range = 0f;
        private float speed = 0f;
        private Vector3 moveDirection = Vector3.zero;

        private float moveDistance = 0f;

        #endregion Variables

        #region Constructor

        public Shot(Projectile stateController) : base(stateController)
        {
            this.stateController = stateController as Fireball_2;
        }

        #endregion Constructor

        #region Methods

        public override void OnStart()
        {
            range = stateController.range;
            speed = stateController.speed;
            moveDirection = stateController.moveDirection;

            moveDistance = 0f;
        }

        public override void OnFixedUpdate()
        {
            float deltaDistance = Time.fixedDeltaTime * speed;
            stateController.dnfRigidbody.MoveDirection(deltaDistance * moveDirection);
            moveDistance += deltaDistance;

            if (moveDistance >= range)
            {
                OnComplete();
            }
        }

        public override void OnLateUpdate()
        {
            stateController.AttackHitboxController.CalculateHitbox();

            if (stateController.CalculateOnHit(GameManager.Room.Monsters))
            {
                // Spawn hit effect

                OnComplete();
            }
        }

        public override void OnComplete()
        {
            stateController.Complete();
        }

        #endregion Methods
    }
}

