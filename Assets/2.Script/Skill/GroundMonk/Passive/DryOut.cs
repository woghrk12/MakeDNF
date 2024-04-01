using UnityEngine;

namespace GroundMonkSkill
{
    public class DryOut : PassiveSkill
    {
        #region Variables 

        private float cooldown = 3f;
        private float cooldownTimer = 0f;

        #endregion Variables

        #region Properties

        public override int SkillCode => typeof(DryOut).GetHashCode();

        public override ESkillType SkillType => ESkillType.PASSIVE | ESkillType.CLASSSPECIFIC;

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

            character.AdditionalSkillCondition += CheckCanCancel;
        }

        public override void RemoveSkillEffects()
        {
            character.AdditionalSkillCondition -= CheckCanCancel;
        }

        #endregion Override

        private bool CheckCanCancel(ActiveSkill curSKill)
        {
            if (cooldownTimer < cooldown) return false;
            if (!curSKill.SkillType.HasFlag(ESkillType.CLASSSPECIFIC)) return false;

            cooldownTimer = 0f;

            return true;
        }

        #endregion Methods
    }
}
