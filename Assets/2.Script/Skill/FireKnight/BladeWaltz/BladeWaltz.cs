using System.Collections.Generic;
using UnityEngine;

namespace FireKnightSkill
{
    public partial class BladeWaltz : ActiveSkill, IAttackable
    {
        private enum EState { NONE = -1, FIRST, SECOND }

        public const float DASH_DISTANCE = 6f;

        #region Variables

        private float dashDistance = 0f;

        /// <summary>
        /// Explosion object activated when the character slashes or cancels a skill.
        /// </summary>
        private BladeWaltzProjectile.Explosion explosion = null;

        #endregion Variables

        #region Properties

        public override int SkillCode => typeof(BladeWaltz).GetHashCode();

        public override ESkillType SkillType => ESkillType.ACTIVE | ESkillType.CLASSSPECIFIC;

        #endregion Properties

        #region IAttackable Implementation

        public DNFTransform AttackerDNFTransform { set; get; }
        
        public HitboxController AttackerHitboxController { set; get; }
        
        public List<IDamagable> AlreadyHitTargets { set; get; }

        public void CalculateOnHit(List<IDamagable> targets)
        {
            foreach (IDamagable target in targets)
            {
                if (target.HitboxState == EHitboxState.INVINCIBILITY) continue;
                if (AlreadyHitTargets.Contains(target)) continue;
                if (AttackerHitboxController.CheckCollision(target.DefenderHitboxController))
                {
                    target.OnDamage(character.DNFTransform, null, 3f, character.DNFTransform.IsLeft ? Vector3.left : Vector3.right);
                    character.OnAttack(target.DefenderDNFTransform, EAttackType.SKILL, EHitType.INDIRECT);

                    AlreadyHitTargets.Add(target);
                }
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

            skillHash = Animator.StringToHash(AnimatorKey.Character.FireKnight.BLADE_WALTZ);

            stateList.Add(new First(character, this));
            stateList.Add(new Second(character, this));
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
            character.HitboxState = EHitboxState.SUPERARMOR;

            character.Animator.SetTrigger(skillHash);

            character.DNFTransform.IsBoundaryOverride = true;

            SetState((int)EState.FIRST);
        }

        public override void OnComplete()
        {
            curState = null;

            character.HitboxState = EHitboxState.NONE;

            character.DNFTransform.IsBoundaryOverride = false;

            attackController.OnComplete();
        }

        public override void OnCancel()
        {
            base.OnCancel();

            character.DNFTransform.IsBoundaryOverride = false;
        }

        #endregion Override

        #endregion Methods
    }
}