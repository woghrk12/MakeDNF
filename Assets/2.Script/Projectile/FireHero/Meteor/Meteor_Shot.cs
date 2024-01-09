using UnityEngine;

public partial class Meteor
{
    public class Shot : ProjectileState
    {
        #region Variables

        private Meteor stateController = null;

        private float speed = 0f;
        private Vector3 moveDirection = Vector3.zero;

        #endregion Variables

        #region Constructor

        public Shot(Projectile stateController) : base(stateController)
        {
            this.stateController = stateController as Meteor;
        }

        #endregion Constructor

        #region Methods

        public override void OnStart()
        {
            speed = stateController.speed;
            moveDirection = stateController.moveDirection;
            
            stateController.alreadyHitObjects.Clear();
            
            stateController.AttackHitboxController.HitboxIndex = (int)EState.SHOT;
        }

        public override void OnFixedUpdate()
        {
            stateController.dnfRigidbody.MoveDirection(Time.fixedDeltaTime * speed * moveDirection);

            if (stateController.dnfRigidbody.IsGround)
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
            }
        }

        public override void OnComplete()
        {
            stateController.SetState((int)EState.EXPLOSION);
        }

        #endregion Methods
    }
}
