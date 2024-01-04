using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FlameStrike_FireHero
{
    public class Aim : SkillState
    {
        #region Variables

        private FlameStrike_FireHero stateController = null;

        private bool isAiming = false;
        private float aimingTime = 2f;

        #endregion Variables

        #region Constructor

        public Aim(BehaviourController character, Skill stateController) : base(character, stateController)
        {
            this.stateController = stateController as FlameStrike_FireHero;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.FLAME_STRIKE);

            preDelay = Time.deltaTime * 1f * 4f;
        }

        #endregion Constructor

        #region Methods

        #region Override    

        public override void OnStart()
        {
            timer = 0f;
            isPreDelay = true;
            isAiming = true;

            stateController.aimTransform.Position = character.DNFTransform.Position;
            stateController.aimTransform.gameObject.SetActive(true);

            GameManager.Input.AddMovementDelegate(OnJoystickMoved);

            character.Animator.SetTrigger(stateHash);
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if (isPreDelay)
            {
                if (timer < preDelay) return;

                isPreDelay = false;
                timer = 0f;
            }
            else
            {
                if (isAiming && timer < aimingTime) return;

                stateController.aimTransform.gameObject.SetActive(false);

                GameManager.Input.RemoveMovementDelegate(OnJoystickMoved);

                stateController.SetState((int)EState.SHOT);
            }
        }

        public override void OnCancel()
        {
            GameManager.Input.RemoveMovementDelegate(OnJoystickMoved);
            character.Animator.SetTrigger(cancelHash);
        }

        public override void OnSkillButtonReleased()
        {
            isAiming = false;
        }

        #endregion Override

        public void OnJoystickMoved(Vector3 direction)
        {
            if (isPreDelay) return;
            if (!isAiming) return;

            stateController.aimTransform.Position += stateController.aimSpeed * Time.fixedDeltaTime * direction;
        }

        #endregion Methods
    }
}
