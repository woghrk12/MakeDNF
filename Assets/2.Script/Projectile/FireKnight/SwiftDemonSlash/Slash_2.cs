using System.Collections.Generic;
using UnityEngine;

namespace FireKnightSkill.SwiftDemonSlashProjectile
{
    public partial class Slash_2 : Projectile, IAttackable
    {
        public enum EState { NONE = -1, SHOT }

        #region Properties

        protected override EObjectPoolList ObjectPoolIndex => EObjectPoolList.Slash_2_FireKnight;

        #endregion Properties

        #region IAttackable Implementation

        public DNFTransform AttackerDNFTransform { set; get; }

        public HitboxController AttackerHitboxController { set; get; }

        public List<IDamagable> AlreadyHitTargets { set; get; }

        public void CalculateOnHit(IDamagable[] targets)
        {
            foreach (IDamagable target in targets)
            {
                if (target.HitboxState == EHitboxState.INVINCIBILITY) continue;
                if (AlreadyHitTargets.Contains(target)) continue;
                if (AttackerHitboxController.CheckCollision(target.DefenderHitboxController))
                {
                    target.OnDamage(AttackerDNFTransform, null, 5f, dnfTransform.IsLeft ? Vector3.left : Vector3.right);
                    spawnerTransform.GetComponent<Character>().OnAttack(target.DefenderDNFTransform, EAttackType.SKILL, EHitType.INDIRECT);

                    AlreadyHitTargets.Add(target);
                }
            }
        }

        #endregion IAttackable Implementation

        #region Unity Events

        protected override void Awake()
        {
            base.Awake();

            AttackerHitboxController = GetComponent<HitboxController>();
            AttackerHitboxController.Init(dnfTransform);
            AlreadyHitTargets = new List<IDamagable>();

            stateList.Add(new Shot(this));
        }

        #endregion Unity Events

        #region Methods

        #region Override

        public override void Activate(DNFTransform subjectTransform, DNFTransform targetTransform, float sizeEff = 1f)
        {
            AttackerDNFTransform = spawnerTransform = subjectTransform;

            // Set projectile transform
            dnfTransform.Position = AttackerDNFTransform.Position;
            dnfTransform.IsLeft = AttackerDNFTransform.IsLeft;
            dnfTransform.LocalScale = sizeEff;

            curState = stateList[(int)EState.SHOT];
            curState.OnStart();
        }

        #endregion Override

        #endregion Methods
    }
}
