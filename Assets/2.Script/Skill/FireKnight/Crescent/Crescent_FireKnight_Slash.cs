using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Crescent_FireKnight
{
    public class Slash : SkillState
    {
        private enum EHitboxState { NONE = -1, FIRST, SECOND, THIRD }

        #region Variables

        private Crescent_FireKnight stateController = null;

        private EHitboxState hitboxState = EHitboxState.NONE;
        private FollowingVFX slashEffect = null;

        private bool isDash = false;
        private float dashSpeed = 20f;
        private Vector3 dashDirection = Vector3.zero;

        #endregion Variables

        #region Constructor

        public Slash(Character character, Crescent_FireKnight stateController) : base(character, stateController)
        {
            this.stateController = stateController;

            skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.CRESCENT);
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override void OnStart()
        {
            character.CanMove = false;
            character.CanJump = false;

            isDash = false;
            dashDirection = Vector3.zero;

            phase = EStatePhase.PREDELAY;

            stateController.alreadyHitObjects.Clear();

            attackSpeed = character.Animator.GetFloat(attackSpeedHash);

            GameManager.Input.AddMovementDelegate(OnJoystickMoved);
        }

        public override void OnUpdate()
        {
            AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

            switch (phase)
            {
                case EStatePhase.PREDELAY:
                    if (!animatorStateInfo.IsName("Crescent_Predelay")) return;
                    if (animatorStateInfo.normalizedTime < 1f) return;

                    slashEffect = GameManager.Effect.SpawnFromPool(EEffectList.Crescent_Slash).GetComponent<FollowingVFX>();
                    slashEffect.InitEffect(character.DNFTransform);
                    slashEffect.SetMotionSpeed(attackSpeed);

                    hitboxState = EHitboxState.FIRST;
                    stateController.AttackHitboxController.EnableHitbox((int)hitboxState);

                    character.Animator.SetTrigger(skillHash);

                    phase = EStatePhase.HITBOXACTIVE;

                    break;

                case EStatePhase.HITBOXACTIVE:
                    if (!animatorStateInfo.IsName("Crescent")) return;

                    switch (hitboxState)
                    {
                        case EHitboxState.FIRST:
                            if (animatorStateInfo.normalizedTime < 1f / 3f) return;

                            hitboxState = EHitboxState.SECOND;
                            stateController.AttackHitboxController.EnableHitbox((int)EHitboxState.SECOND);
                            
                            stateController.alreadyHitObjects.Clear();

                            return;

                        case EHitboxState.SECOND:
                            if (animatorStateInfo.normalizedTime < 2f / 3f) return;

                            hitboxState = EHitboxState.THIRD;
                            stateController.AttackHitboxController.EnableHitbox((int)EHitboxState.THIRD);

                            stateController.alreadyHitObjects.Clear();

                            return;

                        case EHitboxState.THIRD:
                            if (animatorStateInfo.normalizedTime < 1f) return;

                            hitboxState = EHitboxState.NONE;
                            stateController.AttackHitboxController.DisableHitbox();

                            character.Animator.SetTrigger(skillHash);

                            phase = EStatePhase.POSTDELAY;

                            return;
                    }

                    break;

                case EStatePhase.STOPMOTION:
                    if (stiffnessTimer < stiffnessTime)
                    {
                        stiffnessTimer += Time.deltaTime;
                        return;
                    }

                    character.Animator.SetFloat(attackSpeedHash, attackSpeed);
                    slashEffect.SetMotionSpeed(attackSpeed);

                    phase = EStatePhase.HITBOXACTIVE;

                    break;

                case EStatePhase.POSTDELAY:
                    if (!animatorStateInfo.IsName("Crescent_Postdelay")) return;
                    if (animatorStateInfo.normalizedTime < 1f) return;

                    OnComplete();

                    break;
            }
        }

        public override void OnFixedUpdate()
        {
            if (!isDash) return;
            if (phase == EStatePhase.NONE || phase == EStatePhase.PREDELAY || phase == EStatePhase.STOPMOTION) return;

            AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);
            
            if (!animatorStateInfo.IsName("Crescent")) return;
            
            float dashRatio = EaseHelper.EaseInSine(1f, 0f, animatorStateInfo.normalizedTime);
            Vector3 dashDirection = Time.fixedDeltaTime * dashRatio * dashSpeed * this.dashDirection;
            
            character.DNFRigidbody.MoveDirection(dashDirection);
        }

        public override void OnLateUpdate()
        {
            if (!stateController.AttackHitboxController.IsHitboxActivated) return;

            if (stateController.CalculateOnHit(GameManager.Room.Monsters))
            {
                // Stiffness effect
                character.Animator.SetFloat(attackSpeedHash, 0f);
                slashEffect.SetMotionSpeed(0f);
                stiffnessTimer = 0f;
                phase = EStatePhase.STOPMOTION;

                // TODO : Spawn hit effects
            }
        }

        public override void OnComplete()
        {
            GameManager.Input.RemoveMovementDelegate(OnJoystickMoved);

            phase = EStatePhase.NONE;

            slashEffect.ReturnEffect();
            slashEffect = null;

            character.Animator.SetFloat(attackSpeedHash, attackSpeed);

            character.CanMove = true;
            character.CanJump = true;

            stateController.OnComplete();
        }

        public override void OnCancel()
        {
            GameManager.Input.RemoveMovementDelegate(OnJoystickMoved);

            phase = EStatePhase.NONE;

            if (stateController.AttackHitboxController.IsHitboxActivated)
            {
                stateController.AttackHitboxController.DisableHitbox();
            }

            slashEffect.ReturnEffect();
            slashEffect = null;

            character.Animator.ResetTrigger(skillHash);
            character.Animator.SetFloat(attackSpeedHash, attackSpeed);
        }

        #endregion Override

        public void OnJoystickMoved(Vector3 direction)
        {
            if (phase == EStatePhase.PREDELAY)
            {
                if (character.DNFTransform.IsLeft)
                {
                    if (direction.x <= 0f)
                    {
                        isDash = true;
                        dashDirection = Vector3.left;
                    }
                    else
                    {
                        isDash = false;
                    }
                }
                else
                {
                    if (direction.x >= 0f)
                    {
                        isDash = true;
                        dashDirection = Vector3.right;
                    }
                    else
                    {
                        isDash = false;
                    }
                }
            }
            else if (isDash)
            {
                dashDirection.z = direction.z;
            }
        }

        #endregion Methods
    }
}
