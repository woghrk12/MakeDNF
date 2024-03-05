using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface representing an object capable of receiving damage.
/// </summary>
public interface IDamagable
{
    /// <summary>
    /// DNFTransform component of the defending object.
    /// </summary>
    public DNFTransform DefenderDNFTransform { set; get; }

    /// <summary>
    /// Hitbox controller component used to check whether the object has been hit.
    /// </summary>
    public HitboxController DefenderHitboxController { set; get; }

    /// <summary>
    /// The event method called when the object is hit.
    /// </summary>
    /// <param name="attacker">The DNFTransform component of the attacker</param>
    /// <param name="damages">The list of damamges</param>
    /// <param name="knockBackPower">The power value for the knock back effect</param>
    /// <param name="knockBackDirection">The normalized direction value for the knock back effect</param>
    public void OnDamage(DNFTransform attacker, List<int> damages, float knockBackPower, Vector3 knockBackDirection);
}
