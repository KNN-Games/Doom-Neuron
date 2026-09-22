using UnityEngine;

/// <summary>
/// Fires a straight projectile on primary fire.
/// </summary>
public class Orb : Weapon
{
    [Header("Orb settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private int damage = 20;

    // Primary fire begin/end
    public override void StartPrimary() => animator.SetBool(FirePrimaryHash, true);
    public override void EndPrimary() => animator.SetBool(FirePrimaryHash, false);

    // Secondary fire begin/end (unused for now, wired up for later)
    public override void StartSecondary()
    {
        // TO DO: make the player explode with lifesteal
    }
    public override void EndSecondary()
    {
        // Nothing
    }
    // Called by an Animation Event on the fire clip's trigger frame
    public override void Attack()
    {
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Launch(CalculateDirectionVectorWithCamera(0) * projectileSpeed, damage);
    }
}