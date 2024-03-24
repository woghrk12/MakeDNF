using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GroundMonkSkill
{
    public partial class GattlingPunch
    {
        public class Combo : SkillState
        {
            #region Variables

            private GattlingPunch stateController = null;

            /// <summary>
            /// The maximum number of combo attacks
            /// </summary>
            private int maxCombo = 0;

            /// <summary>
            /// The maximum number of combo attacks the player can input additionally.
            /// </summary>
            private int maxAdditionalCombo = 3;

            /// <summary>
            /// The number of combo attacks performed so far.
            /// </summary>
            private int curCombo = 0;

            [Header("Variables for attack motion speed")]
            private int attackSpeedHash = 0;
            private float attackSpeed = 0f;
            private float originalAttackSpeed = 0f;

            #endregion Variables

            #region Constructor 

            public Combo(Character character, GattlingPunch stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.GATTLING_PUNCH);
                attackSpeedHash = Animator.StringToHash(AnimatorKey.Character.ATTACK_SPEED);

                duration = 2f / 3f;
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                character.CanMove = false;
                character.CanJump = false;

                maxCombo = stateController.numCombo;
                curCombo = 0;

                originalAttackSpeed = character.Animator.GetFloat(attackSpeedHash);
                attackSpeed = originalAttackSpeed;

                phase = EStatePhase.PREDELAY;

                stateController.AlreadyHitTargets.Clear();
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (!animatorStateInfo.IsName("GattlingPunch_Combo_" + (curCombo % 2).ToString())) return;

                        GameManager.Effect.SpawnFromPool(EEffectList.Side_Dust).GetComponent<InstanceVFX>().InitEffect(character.DNFTransform);
                        
                        character.AddEffect(EEffectList.Straight_Fist_1, true);

                        stateController.AttackerHitboxController.EnableHitbox((int)EState.COMBO);

                        curCombo++;

                        phase = EStatePhase.HITBOXACTIVE;

                        break;

                    case EStatePhase.HITBOXACTIVE:
                        stateController.AttackerHitboxController.CalculateHitbox();

                        if (animatorStateInfo.normalizedTime < duration) return;

                        stateController.AttackerHitboxController.DisableHitbox();

                        stateController.AlreadyHitTargets.Clear();

                        phase = EStatePhase.MOTIONINPROGRESS;

                        break;

                    case EStatePhase.MOTIONINPROGRESS:
                        if (animatorStateInfo.normalizedTime < 1f) return;

                        if (curCombo < maxCombo)
                        {
                            character.Animator.SetBool(continueHash, true);
                            character.Animator.SetTrigger(skillHash);

                            phase = EStatePhase.PREDELAY;
                        }
                        else
                        {
                            character.Animator.SetFloat(attackSpeedHash, originalAttackSpeed);

                            character.Animator.SetBool(continueHash, false);
                            character.Animator.SetTrigger(skillHash);

                            stateController.SetState((int)EState.FINISH);
                        }

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
                phase = EStatePhase.NONE;

                character.Animator.SetFloat(attackSpeedHash, originalAttackSpeed);

                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetBool(continueHash, false);

                character.Animator.SetTrigger(cancelHash);
            }

            public override void OnSkillButtonPressed()
            {
                if (maxCombo - stateController.numCombo >= maxAdditionalCombo) return;

                maxCombo++;
                attackSpeed += 0.3f;

                character.Animator.SetFloat(attackSpeedHash, attackSpeed);
            }

            #endregion Override

            #endregion Methods
        }
    }
}