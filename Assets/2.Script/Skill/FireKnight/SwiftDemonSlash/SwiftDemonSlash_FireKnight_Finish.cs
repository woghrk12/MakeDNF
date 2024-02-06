using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SwiftDemonSlash_FireKnight
{
    public class Finish : SkillState
    {
        #region Variables

        private SwiftDemonSlash_FireKnight stateController = null;

        #endregion Variables

        #region Constructor

        public Finish(Character character, SwiftDemonSlash_FireKnight stateController) : base(character, stateController)
        {
            this.stateController = stateController;

            skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.SLASH_COMBO);

            preDelay = 11f / 18f;
        }

        #endregion Constructor

        #region Methods

        #region Override

        public override void OnStart()
        {
            phase = EStatePhase.PREDELAY;
        }

        public override void OnUpdate()
        {
            AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

            switch (phase)
            {
                case EStatePhase.PREDELAY:
                    if (!animatorStateInfo.IsName("SlashCombo_Finish")) return;
                    if (animatorStateInfo.normalizedTime < preDelay) return;

                    GameManager.ObjectPool.SpawnFromPool(EObjectPoolList.Slash_2_FireKnight).GetComponent<Projectile>().Activate(character.DNFTransform);

                    phase = EStatePhase.MOTIONINPROGRESS;

                    break;

                case EStatePhase.MOTIONINPROGRESS:
                    if (animatorStateInfo.normalizedTime < 1f) return;

                    OnComplete();

                    break;
            }
        }

        public override void OnComplete()
        {
            phase = EStatePhase.NONE;

            character.Animator.SetBool(continueHash, false);

            character.CanMove = true;
            character.CanJump = true;

            stateController.OnComplete();
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
