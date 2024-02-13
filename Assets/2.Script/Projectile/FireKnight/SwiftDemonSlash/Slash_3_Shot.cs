using UnityEngine;

namespace FireKnightSkill.SwiftDemonSlashProjectile
{
    public partial class Slash_3
    {
        public class Shot : ProjectileState
        {
            #region Variables

            private Slash_3 stateController = null;

            #endregion Variables

            #region Constructor

            public Shot(Slash_3 stateController) : base(stateController)
            {
                this.stateController = stateController;

                preDelay = 1f / 7f;
                duration = 6f / 7f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                stateController.alreadyHitObjects.Clear();

                phase = EStatePhase.PREDELAY;
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = stateController.animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (animatorStateInfo.normalizedTime < preDelay) return;

                        stateController.AttackHitboxController.EnableHitbox((int)EState.SHOT);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.HITBOXACTIVE:
                        if (animatorStateInfo.normalizedTime < duration) return;

                        stateController.AttackHitboxController.DisableHitbox();

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
                if (!stateController.AttackHitboxController.IsHitboxActivated) return;

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