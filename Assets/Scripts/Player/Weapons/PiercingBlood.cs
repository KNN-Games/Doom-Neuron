using UnityEngine;

public class PiercingBlood : Weapon
{
    private static readonly int Secondary = Animator.StringToHash("FireSecondary");
    // Primary fire begin/end
    public override void StartPrimary()
    {
        animator.SetBool(FirePrimaryHash, true);
    }
    public override void EndPrimary()
    {
        animator.SetBool(FirePrimaryHash, false);
    }
    // Secondary fire begin/end
    public override void StartSecondary()
    {
        animator.SetBool(Secondary, true);
    }
    public override void EndSecondary()
    {
        animator.SetBool(Secondary, false);
    }
    // Attack used by animation event
    public override void Attack()
    {
        Ray ray = new(firePoint.transform.position, Vector3.zero);
        Vector3 targetPoint = ray.origin + ray.direction * 100f;
        Vector3 direction = (targetPoint - firePoint.position).normalized;
       // GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
      //  projectile.GetComponent<Rigidbody>().linearVelocity = direction * bulletSpeed;
    }
}
