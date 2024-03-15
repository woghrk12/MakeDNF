using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireKnightSkill
{
    public partial class BladeWaltz
    {
        public class Second : SkillState
        {
            #region Variables

            private BladeWaltz stateController = null;

            #endregion Variables

            #region Constructor

            public Second(Character character, BladeWaltz stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.BLADE_WALTZ);

                preDelay = 11f / 18f;
                duration = 14f / 18f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                stateController.dashDistance *= -1f;

                character.DNFTransform.IsLeft = stateController.dashDistance < 0f;

                phase = EStatePhase.PREDELAY;

                stateController.AlreadyHitTargets.Clear();
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (!animatorStateInfo.IsName("BladeWaltz_Second")) return;
                        if (animatorStateInfo.normalizedTime < preDelay) return;

                        stateController.AttackerHitboxController.EnableHitbox((int)EState.SECOND);

                        GameManager.Effect.SpawnFromPool(EEffectList.Side_Dust).GetComponent<InstanceVFX>().InitEffect(character.DNFTransform);
                        character.DNFRigidbody.MoveDirection(new Vector3(stateController.dashDistance, 0f, 0f));

                        GameManager.Effect.SpawnFromPool(EEffectList.Horizontal_Slash).GetComponent<InstanceVFX>().InitEffect(character.DNFTransform);

                        stateController.explosion.TriggerExplosion();

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
                character.DNFTransform.IsLeft = !character.DNFTransform.IsLeft;

                character.CanMove = true;
                character.CanJump = true;

                stateController.OnComplete();
            }

            public override void OnCancel()
            {
                if (phase == EStatePhase.PREDELAY)
                { 
                    stateController.explosion.TriggerExplosion();
                }

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
