using UnityEngine;

public class Dummy : Enemy
{
    protected override void Start()
    {
        // Don't check for 0 health as it is irrelevant for dummies.
        Health = 100000000;
    }
    public override void TakeDamage(int damage)
    {
        Health = 100000000;
        base.TakeDamage(damage);
        Debug.Log("Damage dealt to dummy: " + damage);
    }
}
