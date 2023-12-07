using UnityEngine;

[RequireComponent(typeof(DNFRigidbody))]
public class CharacterMove : MonoBehaviour
{
    #region Variables

    private DNFRigidbody dnfRigidbody = null;
    
    [SerializeField] private float xMoveSpeed = 0f;
    [SerializeField] private float zMoveSpeed = 0f;
    [SerializeField] private float jumpPower = 0f;

    #endregion Variables

    #region Methods

    public void Init(DNFRigidbody dnfRigidbody)
    {
        this.dnfRigidbody = dnfRigidbody;
    }

    #region Move

    public void Move(Vector3 moveDir)
    {
        moveDir.x *= xMoveSpeed * Time.fixedDeltaTime;
        moveDir.z *= zMoveSpeed * Time.fixedDeltaTime;

        dnfRigidbody.MoveDirection(moveDir);
    }

    #endregion Move

    #region Jump

    public void Jump()
    {
        if (!dnfRigidbody.IsGround) return;

        dnfRigidbody.AddForce(new Vector3(0f, jumpPower, 0f));
    }

    #endregion Jump

    #endregion Methods
}
