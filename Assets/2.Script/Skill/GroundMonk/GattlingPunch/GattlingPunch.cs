using System.Collections.Generic;
using UnityEngine;

namespace GroundMonkSkill
{
    public partial class GattlingPunch : ActiveSkill, IAttackable
    {
        private enum EState { NONE = -1, COMBO, FINISH }

        #region Variables

        /// <summary>
        /// The number of combo attacks.
        /// </summary>
        private int numCombo = 10;

        #endregion Variables

        #region Properties

        public override int SkillCode => typeof(GattlingPunch).GetHashCode();

        #endregion Properties

        #region IAttackable Implementation

        public DNFTransform AttackerDNFTransform { set; get; }

        public HitboxController AttackerHitboxController { set; get; }

        public List<IDamagable> AlreadyHitTargets { set; get; }
        
        public void CalculateOnHit(List<IDamagable> targets)
        {
            throw new System.NotImplementedException();
        }

        #endregion IAttackable Implementation

        #region Methods

        #region Override

        public override void Init(Character character, AttackBehaviour attackController)
        {
            base.Init(character, attackController);

            skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.GATTLING_PUNCH);

            stateList.Add(new Combo(character, this));
            stateList.Add(new Finish(character, this));
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
            character.HitboxState = EHitboxState.SUPERARMOR;

            character.Animator.SetTrigger(skillHash);

            SetState((int)EState.COMBO);
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
