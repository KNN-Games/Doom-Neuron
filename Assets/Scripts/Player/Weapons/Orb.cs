using UnityEngine;

/// <summary>
/// Fires a straight projectile on primary fire.
/// </summary>
/// <remarks>
/// This weapon has a charge mechanic:
/// 1. click quickly to fire a small projectile
/// 2. hold the fire button for a short while to make the projectile bigger, do more damage and explode on contact
/// </remarks>
public class Orb : Weapon
{
    private static readonly int CharheWeaponHash = Animator.StringToHash("ChargeWeapon");
    [Header("Orb settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject chargedProjectilePrefab;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private int damage = 20;
    [SerializeField] private int chargedDamage = 50;
    [SerializeField] private float chargeTime = 2f;
    [SerializeField] private int chargeBloodCost;
    private bool charging;
    private bool chargeReady = false;
    private float chargeTimer;

    // Primary fire begin/end
    public override void StartPrimary()
    {
        chargeTimer = 0; // Reset timer, start a new one
        charging = true;
        animator.SetBool(CharheWeaponHash, true);
    }
    public override void EndPrimary()
    {
        charging = false;
        animator.SetBool(CharheWeaponHash, false); // the animator will start "FireWeapon" animation immediately due to transition logic
    }

    // Secondary fire begin/end (unused for now, wired up for later)
    public override void StartSecondary()
    {
        // TO DO: make the player explode with lifesteal
    }
    public override void EndSecondary()
    {
        // Nothing
    }
    // Called by an Animation Event on the fire clip trigger frame
    public override void Attack()
    {
        if (chargeReady) // Is charged?
        {
            // Charged projectile
            ProjectileAttack(chargedProjectilePrefab, projectileSpeed, chargedDamage);
            chargeReady = false;
        }
        else
        {
            // Normal projectile
            ProjectileAttack(projectilePrefab, projectileSpeed, damage);
        }
    }
    private void Update()
    {
        if (charging && !chargeReady)
        {
            chargeTimer += Time.deltaTime;
            if(chargeTimer >= chargeTime)
            {
                Debug.Log("Orb charge ready!");
                PlayerHealth.Instance.TakeDamage(chargeBloodCost);
                chargeReady = true;
            }
        }
    }
}