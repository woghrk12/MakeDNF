using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GroundMonkSkill
{
    public partial class GattlingPunch
    {
        public class Finish : SkillState
        {
            #region Variables

            private GattlingPunch stateController = null;

            #endregion Variables

            #region Constructor 

            public Finish(Character character, GattlingPunch stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.GATTLING_PUNCH);
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
                        if (!animatorStateInfo.IsName("GattlingPunch_Finish")) return;

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
}