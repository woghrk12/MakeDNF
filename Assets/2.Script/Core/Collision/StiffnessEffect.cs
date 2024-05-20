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
    private bool hasAttackSpeedHash = false;
    private bool hasHitSpeedHash = false;

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
        hasAttackSpeedHash = attackSpeed > 0;
        hasHitSpeedHash = hitSpeed > 0;
    }

    private void Update()
    {
        if (stiffnessTimer > 0f)
        {
            stiffnessTimer -= Time.deltaTime;

            if (stiffnessTimer < 0f)
            {
                ResetStiffnessEffect();
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

        if (hasAttackSpeedHash)
        {
            attackSpeed = animator.GetFloat(attackSpeedHash);
            animator.SetFloat(attackSpeedHash, 0f);
        }

        if (hasHitSpeedHash)
        {
            hitSpeed = animator.GetFloat(hitSpeedHash);
            animator.SetFloat(hitSpeedHash, 0f);
        }

        stiffnessTimer = GlobalDefine.STIFFNESS_TIME;
    }

    /// <summary>
    /// Reset the stiffness effect applied to the object.
    /// </summary>
    public void ResetStiffnessEffect()
    {
        dnfRigidbody.enabled = true;

        if (hasAttackSpeedHash)
        {
            animator.SetFloat(attackSpeedHash, attackSpeed);
        }

        if (hasHitSpeedHash)
        {
            animator.SetFloat(hitSpeedHash, hitSpeed);
        }

        stiffnessTimer = 0f;
    }

    #endregion Methods
}
