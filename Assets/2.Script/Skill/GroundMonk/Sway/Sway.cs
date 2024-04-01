using UnityEngine;

namespace GroundMonkSkill
{
    public partial class Sway : ActiveSkill
    {
        private enum EState { NONE = -1, DASH }

        #region Properties

        public override int SkillCode => typeof(Sway).GetHashCode();

        public override ESkillType SkillType => ESkillType.ACTIVE | ESkillType.COMMON;

        #endregion Properties

        #region Methods

        #region Override

        public override void Init(Character character, AttackBehaviour attackController, EKeyName keyName)
        {
            base.Init(character, attackController, keyName);

            skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.SWAY);

            stateList.Add(new Dash(character, this));
        }

        public override bool CheckCanUseSkill(Skill activeSkill)
        {
            if (!character.CanAttack) return false;

            if (character.IsJump) return false;

            if (character.CurBehaviourCode == BehaviourCodeList.HIT_BEHAVIOUR_CODE) return false;

            if (activeSkill != null && !CancelList.Contains(activeSkill.SkillCode)) return false;

            return true;
        }

        public override void OnStart()
        {
            character.Animator.SetTrigger(skillHash);

            SetState((int)EState.DASH);
        }

        public override void OnComplete()
        {
            curState = null;

            attackController.OnComplete();
        }

        #endregion Override

        #endregion Methods
    }
}
