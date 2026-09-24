using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Explosion : MonoBehaviour
{
    private float radius;
    private void Start()
    {
        // Destroy after explosion animation ends. The same can be done with animation events, but this is forget-to-do-it-resistant
        Animator animator = GetComponent<Animator>();
        float length = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, length);
    }
    public void DrawExplosionRadius(float radius)
    {
        this.radius = radius;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
