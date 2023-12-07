using UnityEngine;

[RequireComponent(typeof(DNFTransform))]
public class CharacterMove : MonoBehaviour
{
    private DNFTransform dnfTransform = null;

    [SerializeField] private float xMoveSpeed = 1f;
    [SerializeField] private float zMoveSpeed = 1f;

    private void Awake()
    {
        dnfTransform = GetComponent<DNFTransform>();
    }

    private void Start()
    {
        GameManager.Input.SetMovementDelegate(Move);
    }

    public void Move(Vector3 moveDir)
    {
        moveDir.x *= xMoveSpeed * Time.fixedDeltaTime;
        moveDir.z *= zMoveSpeed * Time.fixedDeltaTime;

        dnfTransform.Position += moveDir;
    }
}
