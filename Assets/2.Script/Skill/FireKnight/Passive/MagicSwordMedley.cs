using UnityEngine;

namespace FireKnightSkill
{
    public class MagicSwordMedley : PassiveSkill
    {
        #region Variables

        private float cooldown = 3f;
        private float cooldownTimer = 0f;

        #endregion Variables

        #region Properties

        public override int SkillCode => typeof(MagicSwordMedley).GetHashCode();

        #endregion Properties

        #region Unity Events

        private void Update()
        {
            if (cooldownTimer > cooldown) return;

            cooldownTimer += Time.deltaTime;
        }

        #endregion Unity Events

        #region Methods

        #region Override

        public override void ApplySkillEffects()
        {
            cooldownTimer = cooldown;

            character.AttackEvent += SpawnProjectile;
        }

        public override void RemoveSkillEffects()
        {
            character.AttackEvent -= SpawnProjectile;
        }

        #endregion Override

        private void SpawnProjectile(DNFTransform target, EAttackType attackType)
        {
            if (attackType == EAttackType.BASEATTACK)
            {
                GameManager.ObjectPool.SpawnFromPool(EObjectPoolList.MagicSwordMedley_Slash).GetComponent<Projectile>().Activate(character.DNFTransform, target);
            }
            else if (attackType == EAttackType.SKILL)
            {
                if (cooldownTimer < cooldown) return;

                GameManager.ObjectPool.SpawnFromPool(EObjectPoolList.MagicSwordMedley_Meteor).GetComponent<Projectile>().Activate(character.DNFTransform, target);

                cooldownTimer = 0f;
            }
        }

        #endregion Methods
    }
}