using System.Collections.Generic;
using UnityEngine;

namespace GroundMonkSkill
{
    public partial class StraightPunch : ActiveSkill, IAttackable
    {
        private enum EState { NONE = -1, PUNCH }

        #region Properties 

        public override int SkillCode => typeof(StraightPunch).GetHashCode();

        public override ESkillType SkillType => ESkillType.ACTIVE | ESkillType.CLASSSPECIFIC;

        #endregion Properties

        #region IAttackable Implementation

        public DNFTransform AttackerDNFTransform { set; get; }

        public HitboxController AttackerHitboxController { set; get; }

        public List<IDamagable> AlreadyHitTargets { set; get; }

        public void CalculateOnHit(List<IDamagable> targets)
        {
            int count = 0;

            foreach (IDamagable target in targets)
            {
                if (target.HitboxState == EHitboxState.INVINCIBILITY) continue;
                if (AlreadyHitTargets.Contains(target)) continue;
                if (AttackerHitboxController.CheckCollision(target.DefenderHitboxController))
                {
                    target.OnDamage(character.DNFTransform, null, 3f, character.DNFTransform.IsLeft ? Vector3.left : Vector3.right);
                    character.OnAttack(target.DefenderDNFTransform, EAttackType.SKILL, EHitType.DIRECT);

                    AlreadyHitTargets.Add(target);

                    count++;
                }
            }

            if (count > 0)
            {
                GameManager.Camera.ShakeCamera(2f);
            }
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

            skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.STRAIGHT_PUNCH);

            stateList.Add(new Punch(character, this));
        }

        public override bool CheckCanUseSkill(ActiveSkill curSkill)
        {
            if (curSkill == null) return false;

            return curSkill.SkillCode == typeof(Sway).GetHashCode();
        }

        public override void OnStart()
        {
            character.HitboxState = EHitboxState.SUPERARMOR;

            character.Animator.SetTrigger(skillHash);

            SetState((int)EState.PUNCH);
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