using UnityEngine;

public class PiercingBlood : Weapon
{
    [Header("Piercing Blood settings")]
    [SerializeField] private int damage;
    [SerializeField] private LayerMask damageableLayers;
    // Primary fire begin/end
    public override void StartPrimary()
    {
        animator.SetBool(FirePrimaryHash, true);
    }
    public override void EndPrimary()
    {
        animator.SetBool(FirePrimaryHash, false);
    }
    // Secondary fire begin/end (unused for not)
    public override void StartSecondary()
    {
    }
    public override void EndSecondary()
    {
    }
    // Attack used by animation event
    public override void Attack()
    {
        HitscanAttack(damage, damageableLayers);
    }
}
