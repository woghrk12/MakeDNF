using UnityEngine;

public abstract class VFX : MonoBehaviour
{
    [SerializeField] protected EEffectList effectIndex = EEffectList.NONE;

    protected Transform cachedTransform = null;
    protected DNFTransform targetTransform = null;

    private void Awake()
    {
        cachedTransform = GetComponent<Transform>();
    }

    public virtual void SetEffect(DNFTransform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    public virtual void OnStart()
    { 
        cachedTransform.position = new Vector3(targetTransform.Position.x, targetTransform.Position.y + targetTransform.Position.z * GlobalDefine.ConvRate, 0f);
        cachedTransform.localScale = new Vector3(targetTransform.IsLeft ? -1f : 1f, 1f, 1f);
    }

    public virtual void OnEnd()
    {
        GameManager.Effect.ReturnEffect(effectIndex, gameObject);
        gameObject.SetActive(false);
    }
}
