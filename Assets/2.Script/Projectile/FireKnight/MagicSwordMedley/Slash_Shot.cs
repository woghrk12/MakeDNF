using UnityEngine;

namespace FireKnightSkill.MagicSwordMedleyProjectile
{
    public partial class Slash
    {
        public class Shot : ProjectileState
        {
            #region Variables

            private Slash stateController = null;

            #endregion Variables

            #region Constructor

            public Shot(Slash stateController) : base(stateController)
            {
                this.stateController = stateController;

                preDelay = 1f / 7f;
                duration = 5f / 7f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                stateController.AlreadyHitTargets.Clear();

                stateController.AttackerHitboxController.EnableHitbox((int)EState.SHOT);

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
