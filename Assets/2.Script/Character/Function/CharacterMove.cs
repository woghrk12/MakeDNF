using UnityEngine;

[RequireComponent(typeof(DNFTransform))]
public class CharacterMove : MonoBehaviour
{
    #region Variables

    private DNFTransform dnfTransform = null;

    [SerializeField] private float xMoveSpeed = 1f;
    [SerializeField] private float zMoveSpeed = 1f;

    #endregion Variables

    #region Methods

    public void Init(DNFTransform dnfTransform)
    {
        this.dnfTransform = dnfTransform;
    }

    public void Move(Vector3 moveDir)
    {
        moveDir.x *= xMoveSpeed * Time.fixedDeltaTime;
        moveDir.z *= zMoveSpeed * Time.fixedDeltaTime;

        dnfTransform.Position += moveDir;
    }

    #endregion Methods
}
