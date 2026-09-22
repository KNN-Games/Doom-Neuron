using UnityEngine;

/// <summary>
/// Projectile that explodes on contact
/// </summary>
public class ExplosiveProjectile : Projectile
{
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] protected float explosionRadius;
    protected override void OnTriggerEnter(Collider other)
    {
        // Explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        // Damage everything in radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            DamageTarget(nearbyObject);
        }
        Destroy(gameObject);

    }
}
