using UnityEngine;

namespace FireKnightSkill
{
    public partial class Dodge : ActiveSkill
    {
        private enum EState { NONE = -1, DODGE }

        #region Variables

        [Header("Animation key hash")]
        private int isHitHash = 0;

        #endregion Variables

        #region Properties

        public override int SkillCode => typeof(Dodge).GetHashCode();

        #endregion Properties

        #region Methods

        #region Override

        public override void Init(Character character, AttackBehaviour attackController)
        {
            base.Init(character, attackController);

            skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.DODGE);
            isHitHash = Animator.StringToHash(AnimatorKey.Character.IS_HIT);

            stateList.Add(new On(character, this));
        }

        public override bool CheckCanUseSkill(Skill activeSkill)
        {
            if (!character.CanAttack) return false;

            if (character.IsJump) return false;

            if (character.CurBehaviourCode == BehaviourCodeList.ATTACK_BEHAVIOUR_CODE && !CancelList.Contains(activeSkill.SkillCode)) return false;

            return true;
        }

        public override void OnStart()
        {
            character.HitboxState = EHitboxState.INVINCIBILITY;

            character.Animator.SetBool(isHitHash, false);
            character.Animator.SetTrigger(skillHash);

            curState = stateList[(int)EState.DODGE];
            curState.OnStart();
        }

        public override void OnComplete()
        {
            curState = null;

            character.HitboxState = EHitboxState.NONE;

            attackController.OnComplete();
        }

        #endregion Override

        #endregion Methods
    }
}