using UnityEngine;

namespace FireKnightSkill
{
    public partial class BladeWaltz
    {
        public class First : SkillState
        {
            #region Variables

            private BladeWaltz stateController = null;

            #endregion Variables

            #region Constructor

            public First(Character character, BladeWaltz stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.BLADE_WALTZ);
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                character.CanMove = false;
                character.CanJump = false;

                bool isLeft = character.DNFTransform.IsLeft;
                
                stateController.dashDistance = DASH_DISTANCE * (isLeft ? -1f : 1f);
                
                stateController.explosion = GameManager.ObjectPool.SpawnFromPool(EObjectPoolList.BladeWaltz_Explosion).GetComponent<BladeWaltzProjectile.Explosion>();
                stateController.explosion.Activate(character.DNFTransform);

                phase = EStatePhase.PREDELAY;

                stateController.AlreadyHitTargets.Clear();
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (!animatorStateInfo.IsName("BladeWaltz_First_Predelay")) return;
                        if (animatorStateInfo.normalizedTime < 1f) return;

                        stateController.AttackerHitboxController.EnableHitbox((int)EState.FIRST);

                        character.Animator.SetTrigger(skillHash);

                        GameManager.Effect.SpawnFromPool(EEffectList.Side_Dust).GetComponent<InstanceVFX>().InitEffect(character.DNFTransform);
                        character.DNFRigidbody.MoveDirection(new Vector3(stateController.dashDistance, 0f, 0f));

                        GameManager.Effect.SpawnFromPool(EEffectList.Horizontal_Slash).GetComponent<InstanceVFX>().InitEffect(character.DNFTransform);

                        GameManager.Camera.ShakeCamera(3f);

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.HITBOXACTIVE:
                        if (!animatorStateInfo.IsName("BladeWaltz_First")) return;
                        if (animatorStateInfo.normalizedTime < 1f) return;

                        stateController.AttackerHitboxController.DisableHitbox();

                        character.Animator.SetTrigger(skillHash);

                        stateController.SetState((int)EState.SECOND);

                        break;
                }
            }

            public override void OnLateUpdate()
            {
                if (!stateController.AttackerHitboxController.IsHitboxActivated) return;

                stateController.CalculateOnHit(GameManager.Room.Monsters);
            }

            public override void OnCancel()
            {
                stateController.explosion.TriggerExplosion();

                if (stateController.AttackerHitboxController.IsHitboxActivated)
                {
                    stateController.AttackerHitboxController.DisableHitbox();
                }

                character.Animator.ResetTrigger(skillHash);

                character.Animator.SetTrigger(cancelHash);
            }

            #endregion Override

            #endregion Methods
        }
    }
}
