using System.Collections.Generic;
using UnityEngine;

namespace FireKnightSkill.SwiftDemonSlashProjectile
{
    public partial class Slash_5 : Projectile, IAttackable
    {
        public enum EState { NONE = -1, SHOT }

        #region Variables

        /// <summary>
        /// The list of objects hit after the projectile is activated.
        /// </summary>
        private List<IDamagable> alreadyHitObjects = new();

        #endregion Variables

        #region Properties

        protected override EObjectPoolList ObjectPoolIndex => EObjectPoolList.Slash_5_FireKnight;

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
                    spawnerTransform.GetComponent<Character>().AttackEvent?.Invoke(target.DefenderDNFTransform, EAttackType.SKILL);

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
            dnfTransform.Position = AttackerDNFTransform.Position;
            dnfTransform.IsLeft = AttackerDNFTransform.IsLeft;
            dnfTransform.LocalScale = sizeEff;

            AttackerHitboxController.CalculateHitbox();

            curState = stateList[(int)EState.SHOT];
            curState.OnStart();
        }

        #endregion Override

        #endregion Methods
    }
}
