using UnityEngine;

namespace FireKnightSkill.SwiftDemonSlashProjectile
{
    public partial class Slash_5
    {
        public class Shot : ProjectileState
        {
            #region Variables

            private Slash_5 stateController = null;

            #endregion Variables

            #region Constructor

            public Shot(Slash_5 stateController) : base(stateController)
            {
                this.stateController = stateController;

                preDelay = 1f / 6f;
                duration = 5f / 6f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                stateController.AlreadyHitTargets.Clear();

                phase = EStatePhase.PREDELAY;
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = stateController.animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (animatorStateInfo.normalizedTime < preDelay) return;

                        stateController.AttackerHitboxController.EnableHitbox((int)EState.SHOT);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.HITBOXACTIVE:
                        if (animatorStateInfo.normalizedTime < duration) return;

                        stateController.AttackerHitboxController.DisableHitbox();

                        phase = EStatePhase.MOTIONINPROGRESS;

                        break;

                    case EStatePhase.MOTIONINPROGRESS:
                        if (animatorStateInfo.normalizedTime < 1f) return;

                        OnComplete();

                        break;
                }
            }

            public override void OnLateUpdate()
            {
                if (!stateController.AttackerHitboxController.IsHitboxActivated) return;

                if (stateController.CalculateOnHit(GameManager.Room.Monsters))
                {
                    // TODO : Spawn hit effect
                }
            }

            public override void OnComplete()
            {
                stateController.Complete();
            }

            #endregion Override

            #endregion Methods
        }
    }
}