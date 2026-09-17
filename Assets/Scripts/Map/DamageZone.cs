using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals damage to player and enemies in an interval
/// </summary>
[RequireComponent(typeof(Collider))]
public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int hardDamage;
    [SerializeField] private float interval = 1f;
    private readonly Dictionary<Collider, float> nextDamageTime = new();

    private void OnTriggerStay(Collider other)
    {
        bool isPlayer = other.CompareTag("Player");
        bool isEnemy = other.TryGetComponent<Enemy>(out var enemy);
        if (!isPlayer && !isEnemy) return; // If not an enemy or player return

        if (!nextDamageTime.ContainsKey(other)) // Add to dictionary if not already in ti.
        {
            nextDamageTime[other] = Time.time;
        }

        if (Time.time < nextDamageTime[other]) return; // If not yet time to damage

        if (isPlayer) // Is Player
        {
            PlayerHealth.Instance.TakeDamage(damage);
            PlayerHealth.Instance.TakeHardDamage(hardDamage);
        }
        else // Is enemy
        {
            enemy.TakeDamage(damage);
        }
        nextDamageTime[other] = Time.time + interval;
    }
    private void OnTriggerExit(Collider other)
    {
        // Reset so leaving and re-entering starts a fresh tick cycle
        nextDamageTime.Remove(other);
    }
}
