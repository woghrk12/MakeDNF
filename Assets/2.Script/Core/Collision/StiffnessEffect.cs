using UnityEngine;

public class StiffnessEffect : MonoBehaviour
{
    #region Variables

    private DNFRigidbody dnfRigidbody = null;

    [Header("Variables for animator")]
    private Animator animator = null;
    private int attackSpeedHash = 0;
    private int hitSpeedHash = 0;

    [Header("Variables for stiffness effect")]
    private float stiffnessTimer = 0f;
    private float attackSpeed = 0f;
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
        if (stiffnessTimer > 0f)
        {
            stiffnessTimer -= Time.deltaTime;

            if (stiffnessTimer < 0f)
            {
                dnfRigidbody.enabled = true;

                animator.SetFloat(attackSpeedHash, attackSpeed);
                animator.SetFloat(hitSpeedHash, hitSpeed);

                stiffnessTimer = 0f;
            }
        }
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Apply the stiffness effect to the object.
    /// The animation briefly pauses, and the DNFRigidbody component momentarily turns off.
    /// </summary>
    public void ApplyStiffnessEffect()
    {
        dnfRigidbody.enabled = false;

        if (stiffnessTimer > 0f)
        {
            stiffnessTimer = GlobalDefine.STIFFNESS_TIME;
            return;
        }

        attackSpeed = animator.GetFloat(attackSpeedHash);
        hitSpeed = animator.GetFloat(hitSpeedHash);

        animator.SetFloat(attackSpeedHash, 0f);
        animator.SetFloat(hitSpeedHash, 0f);

        stiffnessTimer = GlobalDefine.STIFFNESS_TIME;
    }

    #endregion Methods
}
