using UnityEngine;

namespace FireKnightSkill
{
    public partial class SwiftDemonSlash
    {
        public class Combo : SkillState
        {
            #region Variables

            private SwiftDemonSlash stateController = null;

            /// <summary>
            /// The number of slash attacks performed so far.
            /// </summary>
            private int curSlash = 0;

            #endregion Variables

            #region Constructor

            public Combo(Character character, SwiftDemonSlash stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.SWIFT_DEMON_SLASH);
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                character.CanMove = false;
                character.CanJump = false;

                curSlash = 0;

                phase = EStatePhase.PREDELAY;
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                switch (phase)
                {
                    case EStatePhase.PREDELAY:
                        if (!animatorStateInfo.IsName("SwiftDemonSlash_Combo_" + (curSlash % 2).ToString())) return;

                        switch (curSlash % 4)
                        {
                            case 0:
                                GameManager.ObjectPool.SpawnFromPool(EObjectPoolList.Slash_3_FireKnight).GetComponent<Projectile>().Activate(character.DNFTransform);
                                break;

                            case 1:
                                GameManager.ObjectPool.SpawnFromPool(EObjectPoolList.Slash_1_FireKnight).GetComponent<Projectile>().Activate(character.DNFTransform);
                                break;

                            case 2:
                                GameManager.ObjectPool.SpawnFromPool(EObjectPoolList.Slash_5_FireKnight).GetComponent<Projectile>().Activate(character.DNFTransform);
                                break;

                            case 3:
                                GameManager.ObjectPool.SpawnFromPool(EObjectPoolList.Slash_4_FireKnight).GetComponent<Projectile>().Activate(character.DNFTransform);
                                break;
                        }

                        curSlash++;

                        phase = EStatePhase.MOTIONINPROGRESS;

                        break;

                    case EStatePhase.MOTIONINPROGRESS:
                        if (animatorStateInfo.normalizedTime < 1f) return;

                        if (curSlash < stateController.numSlash)
                        {
                            character.Animator.SetBool(continueHash, true);
                            character.Animator.SetTrigger(skillHash);

                            phase = EStatePhase.PREDELAY;
                        }
                        else
                        {
                            character.Animator.SetBool(continueHash, false);
                            character.Animator.SetTrigger(skillHash);

                            stateController.SetState((int)EState.FINISH);
                        }

                        break;
                }
            }

            public override void OnCancel()
            {
                phase = EStatePhase.NONE;

                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetBool(continueHash, false);

                character.Animator.SetTrigger(cancelHash);
            }

            #endregion Override

            #endregion Methods
        }
    }
}