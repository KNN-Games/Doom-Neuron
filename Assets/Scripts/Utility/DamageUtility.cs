using UnityEngine;

/// <summary>
/// Static class that can you can you to calculate damage-related stuff
/// </summary>
/// <remarks>
/// Static means that this script is not assigned to any game object, but is just a collection of static function that u can use anywhere in the project
/// </remarks>
public static class DamageUtility
{
    public static bool IsInMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
    //public static void DamageTarget(Collider collider, LayerMask damageableLayers, int damage) // Damages if possible, does nothing if not
    //{
    //    if (!IsInMask(collider.gameObject.layer, damageableLayers)) return;
    //
    //     // GetComponentInParent in case the collider sits on a child object of the target
    //    IDamageable target = collider.GetComponentInParent<IDamageable>();
    //    target?.TakeDamage(damage); // Take damage if possible
    //}
}
