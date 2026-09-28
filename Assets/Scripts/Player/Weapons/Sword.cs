using UnityEngine;

/// <summary>
/// Slashes in a X pattern with light attacks and in a horizontal pattern on heavy attacks. Or not, ask Oliwia!
/// </summary>
public class Sword : Weapon
{
    private static readonly int PrepareHeavyAttackHash = Animator.StringToHash("PrepareHeavy");
    private static readonly int ExecuteHeavyAttackHash = Animator.StringToHash("ExecuteHeavy");
    [Header("Sword Settings")]
    [SerializeField] private int lightAttackDamage;
    [SerializeField] private int heavyAttackDamage;
    // Charge things
    [SerializeField] private float chargeTime;
    [SerializeField] private int chargeBloodCost;
    private bool charging;
    private bool chargeReady = false;
    private float chargeTimer;
    // Primary fire begin/end
    public override void StartPrimary()
    {
        chargeTimer = 0; // Reset timer, start a new one
        charging = true;
    }
    public override void EndPrimary()
    {
        charging = false;
        if(chargeReady)
        {
            animator.SetTrigger(ExecuteHeavyAttackHash);
        } else
        {
            animator.SetBool(FirePrimaryHash, true);
        }
    }
    // Secondary fire begin/end
    public override void StartSecondary()
    {

    }
    public override void EndSecondary()
    {

    }
    // Attack used by animation event
    public override void Attack()
    {
        if(chargeReady)
        {
            
        } else
        {
            
        }
    }
    private void Update()
    {
        if (charging && !chargeReady)
        {
            chargeTimer += Time.deltaTime;
            if(chargeTimer >= chargeTime)
            {
                Debug.Log("Sword Charge ready!");
                PlayerHealth.Instance.TakeDamage(chargeBloodCost);
                chargeReady = true;
                animator.SetTrigger(PrepareHeavyAttackHash);
            }
        }
    }
}
