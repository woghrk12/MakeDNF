using System.Collections.Generic;
using UnityEngine;

namespace GroundMonkSkill
{
    public partial class NuclearPunch
    {
        public class Charging : SkillState
        {
            #region Variables

            private NuclearPunch stateController = null;

            /// <summary>
            /// The power value gathering enemies around the point of impact during skill charging.
            /// </summary>
            private float gatheringPower = 5f;

            /// <summary>
            /// The range value gathering enemies around the point of impact during skill charging.
            /// </summary>
            private float gatheringRange = 5f;

            #endregion Variables

            #region Constructor

            public Charging(Character character, NuclearPunch stateController) : base(character, stateController)
            {
                this.stateController = stateController;

                skillHash = Animator.StringToHash(AnimatorKey.Character.GroundMonk.NUCLEAR_PUNCH);
            }

            #endregion Constructor

            #region Methods

            #region Override

            public override void OnStart()
            {
                character.CanMove = false;
                character.CanJump = false;

                stateController.AlreadyHitTargets.Clear();

                character.AddEffect(EEffectList.Charging, true);
            }

            public override void OnUpdate()
            {
                AnimatorStateInfo animatorStateInfo = character.Animator.GetCurrentAnimatorStateInfo(0);

                if (!animatorStateInfo.IsName("NuclearPunch_Charging")) return;
                if (animatorStateInfo.normalizedTime < 1f) return;

                character.RemoveEffect(EEffectList.Charging);

                stateController.AttackerHitboxController.EnableHitbox((int)EState.CHARGING);
                
                stateController.AttackerHitboxController.CalculateHitbox();
                stateController.CalculateOnHit(GameManager.Room.Monsters);

                stateController.AttackerHitboxController.DisableHitbox();

                character.Animator.SetTrigger(skillHash);
            }

            public override void OnFixedUpdate()
            {
                List<IDamagable> monsters = GameManager.Room.Monsters;

                Vector3 characterPos = character.DNFTransform.Position;
                bool isLeft = character.DNFTransform.IsLeft;

                Vector3 gatheringPoint = new Vector3(characterPos.x, 0f, characterPos.z)
                    + new Vector3(isLeft ? -1.4f : 1.4f, 0f, 0f);

                foreach (IDamagable monster in monsters)
                {
                    Vector3 monsterPos = monster.DefenderDNFTransform.Position;
                    Vector3 gatheringVector = gatheringPoint - new Vector3(monsterPos.x, 0f, monsterPos.z);

                    if (gatheringVector.sqrMagnitude > gatheringRange * gatheringRange || gatheringVector.sqrMagnitude < GlobalDefine.EPSILON) continue;

                    gatheringVector = gatheringVector.normalized;
                    monster.DefenderDNFTransform.Position += gatheringPower * Time.fixedDeltaTime * gatheringVector;
                }
            }

            public override void OnCancel()
            {
                stateController.AttackerHitboxController.DisableHitbox();

                character.Animator.ResetTrigger(skillHash);
                character.Animator.SetTrigger(cancelHash);
            }

            #endregion Override

            #endregion Methods
        }
    }
}
