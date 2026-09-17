using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Base class for all enemies. Handles health and navigation basics
/// </summary>
[RequireComponent(typeof(Collider))] // because of DamageZone.cs
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Header("Base Enemy Stats")]
    public int Health;
    [HideInInspector] public Transform targetTransform; //as in: the thing the enemy is trying to attack (player or other target)
    protected NavMeshAgent agent;
    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (Health <= 0)
        {
            Debug.LogError("Enemy health is not set. Please set the health value in the inspector.");
            Destroy(gameObject);
        }
    }
    //---DAMAGE SYSTEM---
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
