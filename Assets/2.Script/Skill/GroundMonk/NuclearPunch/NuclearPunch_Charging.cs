using UnityEngine;

namespace GroundMonkSkill
{
    public partial class NuclearPunch
    {
        public class Charging : SkillState
        {
            #region Variables

            private NuclearPunch stateController = null;

            #endregion Variables

            #region Constructor

            public Charging(Character character, NuclearPunch stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.NUCLEAR_PUNCH);
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                character.CanMove = false;
                character.CanJump = false;

                stateController.AlreadyHitTargets.Clear();

                character.AddEffect(EEffectList.Charging, true);
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                if (!animatorStateInfo.IsName("NuclearPunch_Charging")) return;
                if (animatorStateInfo.normalizedTime < 1f) return;

                character.RemoveEffect(EEffectList.Charging);

                stateController.AttackerHitboxController.EnableHitbox((int)EState.CHARGING);
                
                stateController.AttackerHitboxController.CalculateHitbox();
                stateController.CalculateOnHit(GameManager.Room.Monsters);

                stateController.AttackerHitboxController.DisableHitbox();

                character.Animator.SetTrigger(skillHash);
            }

            public override void OnCancel()
            {
                stateController.AttackerHitboxController.DisableHitbox();

                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetTrigger(cancelHash);
            }

            #endregion Override

            #endregion Methods
        }
    }
}
