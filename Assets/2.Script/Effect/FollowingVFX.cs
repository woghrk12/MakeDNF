using UnityEngine;

public class FollowingVFX : VFX
{
    private void FixedUpdate()
    {
        if (targetTransform == null) return;

        cachedTransform.position = Utilities.ConvertDNFPosToWorldPos(targetTransform.Position);
    }
}
