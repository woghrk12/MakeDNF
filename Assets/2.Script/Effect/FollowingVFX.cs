using UnityEngine;

public class FollowingVFX : VFX
{
    private void FixedUpdate()
    {
        if (targetTransform == null) return;

        dnfTransform.Position = targetTransform.Position;
        dnfTransform.IsLeft = targetTransform.IsLeft;
    }
}
