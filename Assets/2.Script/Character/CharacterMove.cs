using UnityEngine;

[RequireComponent(typeof(DNFTransform))]
public class CharacterMove : MonoBehaviour
{
    private DNFTransform dnfTransform = null;
    private PlayerJoystick joystick = null;

    [SerializeField] private float xMoveSpeed = 1f;
    [SerializeField] private float zMoveSpeed = 1f;

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();
    }

    private void Start()
    {
        joystick = GameManager.Input.PlayerJoystick;
    }

    private void FixedUpdate()
    {
        Move(joystick.MoveDirection);
    }

    public void Move(Vector3 moveDir)
    {
        moveDir.x *= xMoveSpeed * Time.fixedDeltaTime;
        moveDir.z *= zMoveSpeed * Time.fixedDeltaTime;

        dnfTransform.Position += moveDir;
    }
}
