using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals damage to anything IDamageable inside it, at a fixed interval per target.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int hardDamage; // Applied only to the player, see below
    [SerializeField] private float interval = 1f;
    private readonly Dictionary<Collider, float> nextDamageTime = new();

    private void OnTriggerStay(Collider other)
    {
        // GetComponentInParent in case the collider sits on a child object of the target. It still works if it isn't though
        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target == null) return; // Not something that can be hurt

        if (!nextDamageTime.ContainsKey(other)) // Add to dictionary if not already in it
        {
            nextDamageTime[other] = Time.time;
        }
        if (Time.time < nextDamageTime[other]) return; // If not yet time to damage

        target.TakeDamage(damage);
        // Hard damage is for player, so it is not part of IDamageable and needs this extra check.
        if (target is PlayerHealth playerHealth)
        {
            playerHealth.TakeHardDamage(hardDamage);
        }
        nextDamageTime[other] = Time.time + interval; // Makes it damage is dealt only after the interval passes
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset so leaving and re-entering starts a fresh tick cycle
        nextDamageTime.Remove(other);
    }
}