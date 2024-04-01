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
        private int numCombo = 7;

        #endregion Variables

        #region Properties

        public override int SkillCode => typeof(GattlingPunch).GetHashCode();

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
                    target.OnDamage(character.DNFTransform, null, 0f, Vector3.zero);
                    character.OnAttack(target.DefenderDNFTransform, EAttackType.SKILL, EHitType.INDIRECT);

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
