using UnityEngine;

public enum ESortingType { NONE = -1, STATIC, UPDATE }

public class SpriteSorter : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform back = null;
    [SerializeField] private Transform front = null;

    private float minZ = 0f;
    private float maxZ = 0f;
    private float totalZ = 0f;

    #endregion Variables

    #region Unity Events

    private void Awake()
    {
        minZ = front.position.y * GlobalDefine.CONV_RATE;
        maxZ = back.position.y * GlobalDefine.CONV_RATE;

        totalZ = maxZ - minZ;
    }

    #endregion Unity Events

    #region Methods

    public int GetSortingOrder(DNFTransform p_obj)
    {
        float objDist = Mathf.Abs(maxZ - p_obj.Position.z);

        return (int)(Mathf.Lerp(System.Int16.MinValue, System.Int16.MaxValue, objDist / totalZ));
    }

    #endregion Methods
}
