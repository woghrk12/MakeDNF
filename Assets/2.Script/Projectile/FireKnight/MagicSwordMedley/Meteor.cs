using System.Collections.Generic;
using UnityEngine;

namespace FireKnightSkill.MagicSwordMedleyProjectile
{
    public partial class Meteor : Projectile, IAttackable
    {
        public enum EState { NONE = -1, SHOT, EXPLOSION }

        #region Variables

        private DNFRigidbody dnfRigidbody = null;

        [SerializeField] private float speed = 0f;
        private Vector3 moveDirection = Vector3.zero;

        #endregion Variables

        #region Properties

        protected override EObjectPoolList ObjectPoolIndex => EObjectPoolList.MagicSwordMedley_Meteor;

        #endregion Properties

        #region IAttackable Implementation

        public DNFTransform AttackerDNFTransform { set; get; }

        public HitboxController AttackerHitboxController { set; get; }

        public List<IDamagable> AlreadyHitTargets { set; get; }

        public void CalculateOnHit(List<IDamagable> targets)
        {
            foreach (IDamagable target in targets)
            {
                if (AlreadyHitTargets.Contains(target)) continue;
                if (AttackerHitboxController.CheckCollision(target.DefenderHitboxController))
                {
                    target.OnDamage(AttackerDNFTransform, null, 0f, Vector3.zero);
                    spawnerTransform.GetComponent<Character>().OnAttack(target.DefenderDNFTransform, EAttackType.ADDITIONAL, EHitType.INDIRECT);

                    AlreadyHitTargets.Add(target);
                }
            }
        }

        #endregion IAttackable Implementation

        #region Unity Events

        protected override void Awake()
        {
            base.Awake();

            dnfRigidbody = GetComponent<DNFRigidbody>();

            AttackerHitboxController = GetComponent<HitboxController>();
            AttackerHitboxController.Init(dnfTransform);
            AlreadyHitTargets = new List<IDamagable>();

            stateList.Add(new Shot(this));
            stateList.Add(new Explosion(this));
        }

        #endregion Unity Events

        #region Methods

        #region Override

        public override void Activate(DNFTransform subjectTransform, DNFTransform targetTransform = null, float sizeEff = 1f)
        {
            AttackerDNFTransform = spawnerTransform = subjectTransform;

            // Set projectile transform
            dnfTransform.Position = new Vector3(targetTransform.Position.x, 0f, targetTransform.Position.z) + new Vector3(spawnerTransform.IsLeft ? 5f : -5f, 5f, 0f);
            dnfTransform.IsLeft = spawnerTransform.IsLeft;

            // Set projectile direction
            moveDirection = ((spawnerTransform.IsLeft ? Vector3.left : Vector3.right) + Vector3.down).normalized;
            
            AttackerHitboxController.CalculateHitbox();

            curState = stateList[(int)EState.SHOT];
            curState.OnStart();
        }

        #endregion Override

        #endregion Methods
    }
}
