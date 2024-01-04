using UnityEngine;

public abstract class VFX : MonoBehaviour
{
    #region Variables

    [SerializeField] protected EEffectList effectIndex = EEffectList.NONE;

    protected Transform cachedTransform = null;
    protected DNFTransform targetTransform = null;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        cachedTransform = GetComponent<Transform>();
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Initialize the effect to set the target's DNF transform.
    /// </summary>
    /// <param name="targetTransform">The DNF transform component of the target</param>
    public virtual void Init(DNFTransform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    #region Virtual

    /// <summary>
    /// The event method called when the effect is started.
    /// </summary>
    public virtual void OnStart()
    { 
        cachedTransform.position = new Vector3(targetTransform.Position.x, targetTransform.Position.y + targetTransform.Position.z * GlobalDefine.CONV_RATE, 0f);
        cachedTransform.localScale = new Vector3(targetTransform.IsLeft ? -1f : 1f, 1f, 1f);
    }

    /// <summary>
    /// The event method called when the effect is end.
    /// </summary>
    public virtual void OnEnd()
    {
        GameManager.Effect.ReturnToPool(effectIndex, gameObject);
        gameObject.SetActive(false);
    }

    #endregion Virtual

    #endregion Methods
}
