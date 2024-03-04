using System.Collections.Generic;
using UnityEngine;

namespace FireKnightSkill
{
    public partial class Crescent : ActiveSkill, IAttackable
    {
        private enum EState { NONE = -1, SLASH }

        #region Properties 

        public override int SkillCode => typeof(Crescent).GetHashCode();

        #endregion Properties

        #region IAttackable Implementation

        public DNFTransform AttackerDNFTransform { set; get; }

        public HitboxController AttackerHitboxController { set; get; }

        public List<IDamagable> AlreadyHitTargets { set; get; }

        public bool CalculateOnHit(List<IDamagable> targets)
        {
            int count = 0;

            foreach (IDamagable target in targets)
            {
                if (AlreadyHitTargets.Contains(target)) continue;
                if (AttackerHitboxController.CheckCollision(target.DefenderHitboxController))
                {
                    target.OnDamage(character.DNFTransform, null, 3f, character.DNFTransform.IsLeft ? Vector3.left : Vector3.right);
                    character.AttackEvent?.Invoke(target.DefenderDNFTransform, EAttackType.SKILL);

                    AlreadyHitTargets.Add(target);

                    count++;
                }
            }

            return count > 0;
        }

        #endregion IAttackable Implementation

        #region Methods

        #region Override

        public override void Init(Character character, AttackBehaviour attackController)
        {
            base.Init(character, attackController);

            AttackerDNFTransform = character.DNFTransform;
            AttackerHitboxController = GetComponent<HitboxController>();
            AttackerHitboxController.Init(AttackerDNFTransform);
            AlreadyHitTargets = new List<IDamagable>();

            skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.CRESCENT);

            stateList.Add(new Slash(character, this));
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

            SetState((int)EState.SLASH);
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
