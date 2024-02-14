using System.Collections.Generic;
using UnityEngine;

namespace FireKnightSkill.MagicSwordMedleyProjectile
{
    public partial class Meteor : Projectile, IAttackable
    {
        public enum EState { NONE = -1, SHOT, EXPLOSION }

        #region Variables

        private DNFRigidbody dnfRigidbody = null;

        /// <summary>
        /// The list of objects hit after the projectile is activated.
        /// </summary>
        private List<IDamagable> alreadyHitObjects = new();

        [SerializeField] private float speed = 0f;
        private Vector3 moveDirection = Vector3.zero;

        #endregion Variables

        #region Properties

        protected override EObjectPoolList ObjectPoolIndex => EObjectPoolList.MagicSwordMedley_Meteor;

        #endregion Properties

        #region IAttackable Implementation

        public DNFTransform AttackDNFTransform { set; get; }

        public HitboxController AttackHitboxController { set; get; }

        public bool CalculateOnHit(List<IDamagable> targets)
        {
            int count = 0;

            foreach (IDamagable target in targets)
            {
                if (alreadyHitObjects.Contains(target)) continue;
                if (AttackHitboxController.CheckCollision(target.DefenseHitboxController))
                {
                    target.OnDamage(AttackDNFTransform, null, 0f, Vector3.zero);
                    spawnerTransform.GetComponent<Character>().AttackEvent?.Invoke(target.DefenseDNFTransform, EAttackType.ADDITIONAL);

                    alreadyHitObjects.Add(target);

                    count++;
                }
            }

            return count > 0;
        }

        #endregion IAttackable Implementation

        #region Unity Events

        protected override void Awake()
        {
            base.Awake();

            dnfRigidbody = GetComponent<DNFRigidbody>();

            AttackHitboxController = GetComponent<HitboxController>();
            AttackHitboxController.Init(dnfTransform);

            stateList.Add(new Shot(this));
            stateList.Add(new Explosion(this));
        }

        #endregion Unity Events

        #region Methods

        #region Override

        public override void Activate(DNFTransform subjectTransform, DNFTransform targetTransform = null, float sizeEff = 1f)
        {
            AttackDNFTransform = spawnerTransform = subjectTransform;

            // Set projectile transform
            dnfTransform.Position = new Vector3(targetTransform.Position.x, 0f, targetTransform.Position.z) + new Vector3(spawnerTransform.IsLeft ? 5f : -5f, 5f, 0f);
            dnfTransform.IsLeft = spawnerTransform.IsLeft;

            // Set projectile direction
            moveDirection = ((spawnerTransform.IsLeft ? Vector3.left : Vector3.right) + Vector3.down).normalized;
            
            AttackHitboxController.CalculateHitbox();

            curState = stateList[(int)EState.SHOT];
            curState.OnStart();
        }

        #endregion Override

        #endregion Methods
    }
}
