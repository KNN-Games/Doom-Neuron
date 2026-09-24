using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Base class for every weapon prefab. Each weapon is its own prefab.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [Header("Identity")]
    public string displayName;
    public string description;
    public AnimatorController animatorController;
    // References
    protected Transform firePoint;
    protected Camera playerCamera;
    protected Animator animator;
    protected static readonly int FirePrimaryHash = Animator.StringToHash("FirePrimary"); // Bool

    public virtual void StartPrimary()
    {
        animator.SetBool(FirePrimaryHash, true);
    }
    public virtual void EndPrimary()
    {
        animator.SetBool(FirePrimaryHash, false);
    }
    public abstract void StartSecondary();
    public abstract void EndSecondary();
    public abstract void Attack(); // Used in animation events

    protected virtual void Start()
    {
        firePoint = WeaponsManager.Instance.firePoint.transform;
        animator = WeaponsManager.Instance.animator;
        playerCamera = PlayerController.Instance.camera;
    }
    // Generic methods you can use to calculate stuff
    protected void ProjectileAttack(GameObject projectilePrefab, float projectileSpeed, int damage, float inaccuracyInDegrees = 0f)
    {
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity).GetComponent<Projectile>();
        Ray ray = FromFirePointToCamera(inaccuracyInDegrees); // Ray from firePoint pointing in direction from camera
        Vector3 targetPoint = ray.origin + ray.direction * 100f; // Point in ray where the projectile should go to.
        Vector3 direction = (targetPoint - firePoint.position).normalized; // Velocity of the projectile - that means "where it should go"
        projectile.Launch(direction * projectileSpeed, damage);
    }
    protected void HitscanAttack(int damage, LayerMask damageableLayers, float inaccuracyInDegrees = 0f)
    {
        Ray ray = FromFirePointToCamera(inaccuracyInDegrees);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000)) return; // Nothing in range
        
        // TO DO: Copied this code from Projectile.cs, how do I make it not like that?
        if (!IsInMask(hit.collider.gameObject.layer, damageableLayers)) return;
        IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
        target?.TakeDamage(damage); // Take damage if possible
    }
    // Internal methods
    private Ray FromFirePointToCamera(float inaccuracyInDegrees = 0f) // Ray from firePoint pointing in direction from camera
    {
        // Picks a random direction inside a cone around the camera's forward vector.
        // angleDeg = 0 always returns the exact forward direction (no spread).
        float angleRad = inaccuracyInDegrees * Mathf.Deg2Rad;

        // Random angle around the cone's axis (0-360 deg)
        float theta = Random.Range(0f, Mathf.PI * 2f); // Generate random not-perfect accuracy
        // Random distance from the cone's axis, biased so the cone fills evenly (not just its edge)
        float z = Mathf.Cos(angleRad * Random.value);
        float r = Mathf.Sqrt(1 - z * z);

        float x = r * Mathf.Cos(theta);
        float y = r * Mathf.Sin(theta);

        // z is "forward" in this local space, x/y are the sideways/up offset
        Vector3 localDir = new(x, y, z);

        // Rotate the local cone direction into world space using the camera's orientation
        Vector3 directionFromCamera = playerCamera.transform.TransformDirection(localDir).normalized;
        return new(firePoint.transform.position, directionFromCamera);
    }
    private static bool IsInMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}