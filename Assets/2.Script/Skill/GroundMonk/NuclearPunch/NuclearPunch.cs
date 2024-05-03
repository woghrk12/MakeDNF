using System.Collections.Generic;
using UnityEngine;

namespace GroundMonkSkill
{
    public partial class NuclearPunch : ActiveSkill, IAttackable
    {
        private enum EState { NONE = -1, CHARGING, HIT, MISS }

        #region Properties 

        public override int SkillCode => typeof(NuclearPunch).GetHashCode();

        public override ESkillType SkillType => ESkillType.ACTIVE | ESkillType.CLASSSPECIFIC;

        #endregion Properties

        #region IAttackable Implementation

        public DNFTransform AttackerDNFTransform { set; get; }

        public HitboxController AttackerHitboxController { set; get; }

        public List<IDamagable> AlreadyHitTargets { set; get; }

        public void CalculateOnHit(IDamagable[] targets)
        {
            int count = 0;

            foreach (IDamagable target in targets)
            {
                if (target.HitboxState == EHitboxState.INVINCIBILITY) continue;
                if (AlreadyHitTargets.Contains(target)) continue;
                if (AttackerHitboxController.CheckCollision(target.DefenderHitboxController))
                {
                    character.OnAttack(target.DefenderDNFTransform, EAttackType.SKILL, EHitType.DIRECT);
                    count++;
                }
            }

            SetState(count > 0 ? (int)EState.HIT : (int)EState.MISS);
        }

        #endregion IAttackable Implementation

        #region Methods

        #region Override

        public override void Init(Character character, AttackBehaviour attackController, EKeyName keyName)
        {
            base.Init(character, attackController, keyName);

            AttackerDNFTransform = character.DNFTransform;
            AttackerHitboxController = GetComponent<HitboxController>();
            AttackerHitboxController.Init(AttackerDNFTransform);
            AlreadyHitTargets = new List<IDamagable>();

            skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.NUCLEAR_PUNCH);

            stateList.Add(new Charging(character, this));
            stateList.Add(new Hit(character, this));
            stateList.Add(new Miss(character, this));
        }

        public override bool CheckCanUseSkill(ActiveSkill curSkill)
        {
            if (!character.CanAttack) return false;

            if (character.IsJump) return false;

            if (character.CurBehaviourCode == BehaviourCodeList.HIT_BEHAVIOUR_CODE) return false;

            if (curSkill != null && !CancelList.Contains(curSkill.SkillCode))
            {
                if (curSkill.SkillCode == SkillCode) return false;
                if (ReferenceEquals(character.AdditionalSkillCondition, null)) return false;
                if (!character.AdditionalSkillCondition(curSkill)) return false;
            } 

            return true;
        }

        public override void OnStart()
        {
            character.HitboxState = EHitboxState.SUPERARMOR;

            character.Animator.SetTrigger(skillHash);

            SetState((int)EState.CHARGING);
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
