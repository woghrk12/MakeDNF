using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBehaviour : GenericBehaviour<Character>
{
    private enum EState { NONE = -1, STIFFNESS, DURATION }

    #region Variables

    private EState state = EState.NONE;
    private float hitStiffnessTime = 0.08f;
    private float hitDuration = 0f;
    private float timer = 0f;

    [Header("Animation key hash")]
    private int normalSpeedHash = 0;
    private int doHitHash = 0;
    private int isHitHash = 0;

    #endregion Variables

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        controller = GetComponent<Character>();

        normalSpeedHash = Animator.StringToHash(AnimatorKey.Character.NORMAL_SPEED);
        doHitHash = Animator.StringToHash(AnimatorKey.Character.DO_HIT);
        isHitHash = Animator.StringToHash(AnimatorKey.Character.IS_HIT);
    }

    #endregion Unity Events

    #region Methods

    public void Hit(float hitDuration, Vector3 knockBackVector)
    {
        this.hitDuration = hitDuration;

        controller.DNFRigidbody.Velocity = Vector3.zero;
        controller.DNFRigidbody.AddForce(knockBackVector);

        controller.SetBehaviour(behaviourCode);
    }

    public override void OnStart()
    {
        timer = 0f;

        controller.CanMove = false;
        controller.CanJump = false;
        controller.CanAttack = false;

        controller.Animator.SetTrigger(doHitHash);
        controller.Animator.SetBool(isHitHash, true);
        controller.Animator.SetFloat(normalSpeedHash, 0f);

        controller.DNFRigidbody.enabled = false;

        state = EState.STIFFNESS;
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case EState.STIFFNESS:
                if (timer < hitStiffnessTime) return;

                timer = 0f;

                controller.DNFRigidbody.enabled = true;
                controller.Animator.SetFloat(normalSpeedHash, 1f);

                state = EState.DURATION;

                break;

            case EState.DURATION:
                if (timer < hitDuration) return;

                OnComplete();

                break;
        } 
    }

    public override void OnComplete()
    {
        controller.CanMove = true;
        controller.CanJump = true;
        controller.CanAttack = true;

        controller.Animator.SetBool(isHitHash, false);
        controller.Animator.SetFloat(normalSpeedHash, 1f);

        controller.SetBehaviour(BehaviourCodeList.IDLE_BEHAVIOUR_CODE);
    }

    public override void OnCancel()
    {
        controller.DNFRigidbody.enabled = true;

        controller.Animator.SetBool(isHitHash, false);
        controller.Animator.SetFloat(normalSpeedHash, 1f);
    }

    #endregion Methods
}
