using UnityEngine;

public partial class Dash_FireHero
{
    public class Dash : SkillState
    {
        #region Variables

        private Dash_FireHero stateController = null;

        private Vector3 dashDirection = Vector3.zero;

        #endregion Variables

        #region Constructor

        public Dash(BehaviourController character, Skill stateController) : base(character, stateController)
        {
            this.stateController = stateController as Dash_FireHero;

            stateHash = Animator.StringToHash(AnimatorKey.Character.FireHero.DASH);

            preDelay = Time.deltaTime * 1f * 4f;
            postDelay = Time.deltaTime * 1f * 4f;
        }

        #endregion Construtor

        #region Methods

        #region Override

        public override void OnStart()
        {
            dashDirection = new Vector3(character.DNFTransform.IsLeft ? -1f : 1f, 0f, 0f);

            timer = 0f;
            isPreDelay = true;
            isPostDelay = false;

            character.Animator.SetTrigger(stateHash);
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if (isPreDelay)
            {
                if (timer >= preDelay)
                {
                    InstanceVFX instanceVFX = GameManager.Effect.SpawnFromPool(EEffectList.Dash_FireHero).GetComponent<InstanceVFX>();
                    instanceVFX.Init(character.DNFTransform);
                    instanceVFX.gameObject.SetActive(true);
                    
                    isPreDelay = false;
                    timer = 0f;
                }
            }
            else if (isPostDelay)
            {
                if (timer >= postDelay)
                { 
                    OnComplete();
                }
            }
            else
            {
                character.DNFTransform.Position += dashDirection * stateController.dashRange;

                isPostDelay = true;
                timer = 0f;
            }
        }

        public override void OnComplete()
        {
            stateController.OnComplete();
        }

        public override void OnCancel()
        {
            character.Animator.SetTrigger(cancelHash);
        }

        #endregion Override

        #endregion Methods
    }
}
