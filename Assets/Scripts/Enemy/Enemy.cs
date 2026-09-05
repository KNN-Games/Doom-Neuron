using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Header("Base Enemy Stats")]
    public int Health;
    [HideInInspector] public Transform targetTransform; //as in: the thing the enemy is trying to attack (player or other target)
    protected NavMeshAgent agent;
    private Transform cameraTransform;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        cameraTransform = PlayerController.Instance.camera.transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();
        if(Health <= 0)
        {
            Debug.LogError("Enemy health is not set. Please set the health value in the inspector.");
            Destroy(gameObject);
        }
    }
    private void LateUpdate()
    {
        //billboard effect
        spriteRenderer.transform.forward = cameraTransform.forward;
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
