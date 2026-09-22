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
    [SerializeField] private LayerMask damageableLayers;
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
        // If player is not in damageable layer, ignore him completely
        int layer = other.gameObject.layer;
        if(!IsLayerInDamageableMask(layer) && layer == LayerMask.NameToLayer("Player")) return;
        
        DamageTarget(other);
        Destroy(gameObject);
    }
    protected void DamageTarget(Collider collider) // Damages if possible, does nothing if not
    {
        if (IsLayerInDamageableMask(collider.gameObject.layer)) // Is in damageable layer?
        {
            // GetComponentInParent in case the collider sits on a child object of the target
            IDamageable target = collider.GetComponentInParent<IDamageable>();
            target?.TakeDamage(damage); // Take damage if possible
        }
    }
    private bool IsLayerInDamageableMask(int layer)
    {
        return (damageableLayers.value & (1 << layer)) != 0;
    }
}
