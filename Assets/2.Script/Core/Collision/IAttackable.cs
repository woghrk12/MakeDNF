using System.Collections.Generic;

/// <summary>
/// Interface representing an subject capable of performing attacks.
/// </summary>
public interface IAttackable
{
    /// <summary>
    /// DNFTransform component of the attacking subject.
    /// </summary>
    public DNFTransform AttackerDNFTransform { set; get; }

    /// <summary>
    /// Hitbox controller component for representing the attack range.
    /// </summary>
    public HitboxController AttackerHitboxController { set; get; }

    /// <summary>
    /// The list of targets hit after the attacker hitbox is activated.
    /// </summary>
    public List<IDamagable> AlreadyHitTargets { set; get; }

    /// <summary>
    /// Calculate hit detection for each target in the given list.
    /// </summary>
    /// <param name="targets">The list of targets to perform hit detection on</param>
    public void CalculateOnHit(IDamagable[] targets);
}
