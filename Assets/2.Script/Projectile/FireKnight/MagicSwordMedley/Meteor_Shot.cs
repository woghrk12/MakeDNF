using UnityEngine;

namespace FireKnightSkill.MagicSwordMedleyProjectile
{
    public partial class Meteor
    {
        public class Shot : ProjectileState
        {
            #region Variables

            private Meteor stateController = null;

            #endregion Variables

            #region Constructor 

            public Shot(Meteor stateController) : base(stateController)
            {
                this.stateController = stateController;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                stateController.AlreadyHitTargets.Clear();

                stateController.AttackerHitboxController.EnableHitbox((int)EState.SHOT);
            }

            public override void OnFixedUpdate()
            {
                stateController.dnfRigidbody.MoveDirection(Time.fixedDeltaTime * stateController.speed * stateController.moveDirection);

                if (stateController.dnfRigidbody.IsGround)
                {
                    OnComplete();
                }
            }

            public override void OnLateUpdate()
            {
                stateController.CalculateOnHit(GameManager.Room.Monsters);
            }

            public override void OnComplete()
            {
                stateController.SetState((int)EState.EXPLOSION);
            }

            #endregion Override

            #endregion Methods
        }
    }
}
