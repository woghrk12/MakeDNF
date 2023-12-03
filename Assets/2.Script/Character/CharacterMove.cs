using UnityEngine;

[RequireComponent(typeof(DNFTransform))]
public class CharacterMove : MonoBehaviour
{
    private DNFTransform dnfTransform = null;

    [SerializeField] private float xMoveSpeed = 1f;
    [SerializeField] private float zMoveSpeed = 1f;
    private Vector3 moveDirection = Vector3.zero;

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveDirection.x = h;
        moveDirection.z = v;
    }

    private void FixedUpdate()
    {
        Move(moveDirection);
    }

    public void Move(Vector3 moveDir)
    {
        moveDir.x *= xMoveSpeed * Time.fixedDeltaTime;
        moveDir.z *= zMoveSpeed * Time.fixedDeltaTime;

        dnfTransform.Position += moveDir;
    }
}
