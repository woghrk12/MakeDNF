using UnityEngine;

namespace FireKnightSkill
{
    public partial class SwiftDemonSlash : ActiveSkill
    {
        private enum EState { NONE = -1, COMBO, FINISH }

        #region Variables

        /// <summary>
        /// The number of slash attacks.
        /// </summary>
        private int numSlash = 12;

        #endregion Variables

        #region Properties

        public override int SkillCode => typeof(SwiftDemonSlash).GetHashCode();

        public override ESkillType SkillType => ESkillType.ACTIVE | ESkillType.CLASSSPECIFIC;

        #endregion Properties

        #region Methods

        #region Override

        public override void Init(Character character, AttackBehaviour attackController, EKeyName keyName)
        {
            base.Init(character, attackController, keyName);

            skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.SWIFT_DEMON_SLASH);

            stateList.Add(new Combo(character, this));
            stateList.Add(new Finish(character, this));
        }

        public override bool CheckCanUseSkill(ActiveSkill curSkill)
        {
            if (!character.CanAttack) return false;

            if (character.IsJump) return false;

            if (character.CurBehaviourCode == BehaviourCodeList.HIT_BEHAVIOUR_CODE) return false;

            if (curSkill != null && !CancelList.Contains(curSkill.SkillCode)) return false;

            return true;
        }

        public override void OnStart()
        {
            character.HitboxState = EHitboxState.INVINCIBILITY;

            character.Animator.SetTrigger(skillHash);

            curState = stateList[(int)EState.COMBO];
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