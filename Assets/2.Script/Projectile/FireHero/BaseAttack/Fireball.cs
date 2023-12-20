using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Projectile
{
    #region Variables

    private DNFRigidbody dnfRigidbody = null;

    [SerializeField] private float duration = 0f;
    [SerializeField] private float speed = 0f;
    private Vector3 moveDirection = Vector3.zero;

    #endregion Variables

    #region Methods

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        dnfRigidbody = GetComponent<DNFRigidbody>();
    }

    #endregion Unity Events

    #region Override 

    public override void Shot(Vector3 startPos, bool isLeft, float sizeEff = 1)
        => StartCoroutine(Activate(startPos, isLeft, sizeEff));

    public override void Clear()
    {
        GameManager.ObjectPool.ReturnToPool(gameObject);
    }

    protected override IEnumerator Activate(Vector3 startPos, bool isLeft, float sizeEff = 1f)
    {
        // Set target List

        // Set projectile transform
        dnfTransform.Position = startPos;
        dnfTransform.IsLeft = isLeft;
        dnfTransform.LocalScale = sizeEff;

        // Set projectile direction
        moveDirection = Time.fixedDeltaTime * speed * (isLeft ? Vector3.left : Vector3.right);

        float timer = 0f;
        while (timer < duration)
        {
            dnfRigidbody.MoveDirection(moveDirection);

            timer += Time.fixedDeltaTime;
            yield return Utilities.WaitForFixedUpdate;
        }

        Clear();
    }

    protected override void CalculateOnHit(List<Hitbox> targets)
    {
        
    }

    #endregion Override

    #endregion Methods
}
