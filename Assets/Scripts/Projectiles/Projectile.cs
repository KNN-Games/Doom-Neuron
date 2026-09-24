using UnityEngine;

/// <summary>
/// Base class for all projectiles
/// </summary>
/// <remarks>
/// If the projectile doesn't do anything interesting u can just use it directly
/// </remarks>
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    // Set in prefab
    [SerializeField] protected LayerMask damageableLayers; // Deals damage and destroys the projectile
    [SerializeField] private LayerMask ignoredLayers; // Passes through silently
    [SerializeField] private float lifetime; // Auto-destroy after this many seconds in case it hits nothing
    // Set in pseudo-constructor Launch
    protected Vector3 velocity;
    private int damage;

    public virtual void Launch(Vector3 velocity, int damage)
    {
        this.velocity = velocity;
        this.damage = damage;
        if (lifetime == 0) // If lifetime is not set use default
        {
            lifetime = 60f;
        }
        Destroy(gameObject, lifetime);
    }
    protected virtual void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsInMask(other.gameObject.layer, ignoredLayers)) return; // Pass through

        DamageTarget(other); // No-ops if the layer isn't damageable, but the projectile is destroyed either way
        Destroy(gameObject);
    }
    protected void DamageTarget(Collider collider) // Damages if possible, does nothing if not
    {
        if (!IsInMask(collider.gameObject.layer, damageableLayers)) return;

        // GetComponentInParent in case the collider sits on a child object of the target
        IDamageable target = collider.GetComponentInParent<IDamageable>();
        target?.TakeDamage(damage); // Take damage if possible
    }
    private static bool IsInMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}