using UnityEngine;

public abstract class VFX : MonoBehaviour
{
    #region Variables

    [SerializeField] protected EEffectList effectIndex = EEffectList.NONE;

    protected Animator animator = null;

    protected Transform cachedTransform = null;
    protected DNFTransform targetTransform = null;

    protected int shotHash = 0;
    protected int motionSpeedHash = 0;

    private float stopTimer = 0f;
    private float originalSpeed = 0f;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        animator = GetComponent<Animator>();

        cachedTransform = GetComponent<Transform>();

        shotHash = Animator.StringToHash(AnimatorKey.Projectile.SHOT);
        motionSpeedHash = Animator.StringToHash(AnimatorKey.Projectile.MOTION_SPEED);
    }

    protected virtual void Update()
    {
        if (stopTimer <= 0f) return;

        stopTimer -= Time.deltaTime;

        if (stopTimer <= 0f)
        {
            animator.SetFloat(motionSpeedHash, originalSpeed);

            originalSpeed = 0f;
            stopTimer = 0f;
        }
    }

    #endregion Unity Events

    #region Methods

    #region Virtual

    /// <summary>
    /// Initialize the effect to set the target's DNF transform.
    /// </summary>
    /// <param name="targetTransform">The DNF transform component of the target</param>
    public virtual void InitEffect(DNFTransform targetTransform)
    {
        this.targetTransform = targetTransform;

        cachedTransform.position = new Vector3(targetTransform.Position.x, targetTransform.Position.y + targetTransform.Position.z * GlobalDefine.CONV_RATE, 0f);
        cachedTransform.localScale = new Vector3(targetTransform.IsLeft ? -1f : 1f, 1f, 1f);
    }

    /// <summary>
    /// The event method called when the effect is end.
    /// </summary>
    public virtual void ReturnEffect()
    {
        GameManager.Effect.ReturnToPool(effectIndex, gameObject);
        gameObject.SetActive(false);
    }

    #endregion Virtual

    /// <summary>
    /// Set the motion speed of the animator.
    /// </summary>
    /// <param name="motionSpeed">The speed value of the animation</param>
    public void SetMotionSpeed(float motionSpeed)
    {
        originalSpeed = motionSpeed;

        animator.SetFloat(motionSpeedHash, motionSpeed);
    }

    /// <summary>
    /// Stop the effect by setting the motion speed to 0.
    /// The effect revert back to its original speed after a certain amount of time.
    /// </summary>
    public void StopEffect()
    {
        if (stopTimer > 0f)
        {
            stopTimer = GlobalDefine.STIFFNESS_TIME;
            return;
        }

        originalSpeed = animator.GetFloat(motionSpeedHash);
        animator.SetFloat(motionSpeedHash, 0f);

        stopTimer = GlobalDefine.STIFFNESS_TIME;
    }

    #endregion Methods
}
