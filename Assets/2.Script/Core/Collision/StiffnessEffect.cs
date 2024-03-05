using UnityEngine;

public enum EStiffnessType { NONE = -1, ATTACK, HIT }

public class StiffnessEffect : MonoBehaviour
{
    #region Variables

    private DNFRigidbody dnfRigidbody = null;

    [Header("Variables for animator")]
    private Animator animator = null;
    private int attackSpeedHash = 0;
    private int hitSpeedHash = 0;

    private float attackStiffnessTimer = 0f;
    private float attackSpeed = 0f;

    private float hitStiffnessTimer = 0f;
    private float hitSpeed = 0f;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        dnfRigidbody = GetComponent<DNFRigidbody>();

        animator = GetComponent<Animator>();
        attackSpeedHash = Animator.StringToHash(AnimatorKey.Character.ATTACK_SPEED);
        hitSpeedHash = Animator.StringToHash(AnimatorKey.Character.HIT_SPEED);

        attackSpeed = animator.GetFloat(attackSpeedHash);
        hitSpeed = animator.GetFloat(hitSpeedHash);
    }

    private void Update()
    {
        if (attackStiffnessTimer > 0f)
        {
            attackStiffnessTimer -= Time.deltaTime;

            if (attackStiffnessTimer < 0f)
            {
                dnfRigidbody.enabled = true;

                animator.SetFloat(attackSpeedHash, attackSpeed);
                attackSpeed = 0f;
                attackStiffnessTimer = 0f;
            }
        }

        if (hitStiffnessTimer > 0f)
        {
            hitStiffnessTimer -= Time.deltaTime;

            if (hitStiffnessTimer < 0f)
            {
                dnfRigidbody.enabled = true;

                animator.SetFloat(hitSpeedHash, hitSpeed);
                hitSpeed = 0f;
                hitStiffnessTimer = 0f;
            }
        }
    }

    #endregion Unity Events

    #region Methods

    public void ApplyStiffnessEffect(EStiffnessType stiffnessType)
    {
        dnfRigidbody.enabled = false;

        if (stiffnessType == EStiffnessType.ATTACK)
        {
            if (attackStiffnessTimer > 0f)
            {
                attackStiffnessTimer = GlobalDefine.ATTACK_STIFFNESS_TIME;
                return;
            }

            animator.SetFloat(hitSpeedHash, hitSpeed);
            hitSpeed = 0f;
            hitStiffnessTimer = 0f;

            attackSpeed = animator.GetFloat(attackSpeedHash);
            animator.SetFloat(attackSpeedHash, 0f);

            attackStiffnessTimer = GlobalDefine.ATTACK_STIFFNESS_TIME;
        }
        else if (stiffnessType == EStiffnessType.HIT)
        {
            if (hitStiffnessTimer > 0f)
            {
                hitStiffnessTimer = GlobalDefine.HIT_STIFFNESS_TIME;
                return;
            }

            animator.SetFloat(attackSpeedHash, attackSpeed);
            attackSpeed = 0f;
            attackStiffnessTimer = 0f;

            hitSpeed = animator.GetFloat(hitSpeedHash);
            animator.SetFloat(hitSpeedHash, 0f);

            hitStiffnessTimer = GlobalDefine.HIT_STIFFNESS_TIME;
        }
    }

    #endregion Methods
}
