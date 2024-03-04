using System.Collections.Generic;
using UnityEngine;

namespace FireKnightSkill.MagicSwordMedleyProjectile
{
    public partial class Slash : Projectile, IAttackable
    {
        public enum EState { NONE = -1, SHOT }

        #region Variables

        /// <summary>
        /// The transform component of the sprite object.
        /// </summary>
        [SerializeField] private Transform spriteTransform = null;

        /// <summary>
        /// The list of objects hit after the projectile is activated.
        /// </summary>
        private List<IDamagable> alreadyHitObjects = new();

        #endregion Variables

        #region Properties

        protected override EObjectPoolList ObjectPoolIndex => EObjectPoolList.MagicSwordMedley_Slash;

        #endregion Properties

        #region IAttackable Implementation

        public DNFTransform AttackerDNFTransform { set; get; }

        public HitboxController AttackerHitboxController { set; get; }

        public bool CalculateOnHit(List<IDamagable> targets)
        {
            int count = 0;

            foreach (IDamagable target in targets)
            {
                if (alreadyHitObjects.Contains(target)) continue;
                if (AttackerHitboxController.CheckCollision(target.DefenderHitboxController))
                {
                    target.OnDamage(AttackerDNFTransform, null, 0f, Vector3.zero);
                    spawnerTransform.GetComponent<Character>().AttackEvent?.Invoke(target.DefenderDNFTransform, EAttackType.ADDITIONAL);

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

            AttackerHitboxController = GetComponent<HitboxController>();
            AttackerHitboxController.Init(dnfTransform);

            stateList.Add(new Shot(this));
        }

        #endregion Unity Events

        #region Methods

        #region Override

        public override void Activate(DNFTransform subjectTransform, DNFTransform targetTransform = null, float sizeEff = 1f)
        {
            AttackerDNFTransform = spawnerTransform = subjectTransform;

            // Set projectile transform
            dnfTransform.Position = targetTransform.Position;
            spriteTransform.Rotate(0f, 0f, Random.Range(0f, 360f));

            AttackerHitboxController.CalculateHitbox();

            curState = stateList[(int)EState.SHOT];
            curState.OnStart();
        }

        #endregion Override

        #endregion Methods
    }
}