using System.Collections.Generic;
using UnityEngine;

namespace FireKnightSkill
{
    public partial class BaseAttack : ActiveSkill, IAttackable
    {
        private enum EState { NONE = -1, FIRST, SECOND, THIRD, JUMP }

        #region Properties

        public override int SkillCode => typeof(BaseAttack).GetHashCode();

        public override ESkillType SkillType => ESkillType.ACTIVE | ESkillType.BASEATTACK;

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
                    character.OnAttack(target.DefenderDNFTransform, EAttackType.BASEATTACK, EHitType.DIRECT);

                    AlreadyHitTargets.Add(target);

                    count++;
                }
            }

            if (count > 0)
            {
                GameManager.Camera.ShakeCamera(1f);
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

            skillHash = Animator.StringToHash(AnimatorKey.Character.BASE_ATTACK);

            stateList.Add(new First(character, this));
            stateList.Add(new Second(character, this));
            stateList.Add(new Third(character, this));
            stateList.Add(new Jump(character, this));
        }

        public override bool CheckCanUseSkill(ActiveSkill curSkill)
        {
            if (!character.CanAttack) return false;

            if (character.IsJump && character.DNFRigidbody.IsGround) return false;

            if (character.CurBehaviourCode != BehaviourCodeList.IDLE_BEHAVIOUR_CODE) return false;

            return true;
        }

        public override void OnStart()
        {
            character.Animator.SetTrigger(skillHash);

            curState = stateList[character.IsJump ? (int)EState.JUMP : (int)EState.FIRST];
            curState.OnStart();
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
