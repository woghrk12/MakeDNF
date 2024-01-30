using UnityEngine;

public class FollowingVFX : VFX
{
    private void FixedUpdate()
    {
        if (targetTransform == null) return;

        cachedTransform.position = new Vector3(targetTransform.Position.x, targetTransform.Position.y + targetTransform.Position.z * GlobalDefine.CONV_RATE, 0f);
    }
}
