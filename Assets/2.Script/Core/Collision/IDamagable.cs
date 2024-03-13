using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumeration representing the states of a hitbox.
/// <para>
/// NONE : the object owning hitboxes takes damage and experience knockback effect upon being hit.
/// SUPERARMOR : the object owning hitboxes takes damage upon being hit but does not experience knockback effect.
/// INVINCIBILITY : the object owning hitboxes is immune to all attacks.
/// </para>
/// </summary>
public enum EHitboxState { NONE = -1, SUPERARMOR, INVINCIBILITY }

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
    /// The hitbox state of the object.
    /// </summary>
    public EHitboxState HitboxState { set; get; }

    /// <summary>
    /// The event method called when the object is hit.
    /// </summary>
    /// <param name="attacker">The DNFTransform component of the attacker</param>
    /// <param name="damages">The list of damamges</param>
    /// <param name="knockBackPower">The power value for the knock back effect</param>
    /// <param name="knockBackDirection">The normalized direction value for the knock back effect</param>
    public void OnDamage(DNFTransform attacker, List<int> damages, float knockBackPower, Vector3 knockBackDirection);
}
