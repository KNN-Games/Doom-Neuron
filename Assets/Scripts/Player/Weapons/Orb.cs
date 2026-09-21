using UnityEngine;

public class Orb : Weapon
{
    // Primary fire
    public override void StartPrimary()
    {
        animator.SetBool(Primary, true);
    }
    public override void EndPrimary()
    {
        animator.SetBool(Primary, false);
    }
    // Secondary fire
    public override void StartSecondary()
    {
        animator.SetBool(Secondary, true);
    }
    public override void EndSecondary()
    {
        animator.SetBool(Secondary, false);
    }
}
