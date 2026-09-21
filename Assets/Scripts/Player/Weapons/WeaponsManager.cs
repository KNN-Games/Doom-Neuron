using UnityEngine;

/// <summary>
/// Handles everything about weapons: firing, equipping, changing active
/// </summary>
/// <remarks>
/// 
/// </remarks>
[RequireComponent(typeof(Animator))]
public class WeaponsManager : Singleton<WeaponsManager>
{
    private static readonly int Lower = Animator.StringToHash("Change");
    public Weapon[] equippedWeapons;
    [HideInInspector] public Animator animator;
    private int activeSlot = 0;
    //private bool weaponBlocked = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        equippedWeapons[activeSlot].animator = animator;
    }
    //Shooting, used by InputManager.cs
    public void SetPrimaryFireStatus(bool status)
    {
        if(status)
        {
            equippedWeapons[activeSlot].StartPrimary();
        } else
        {
            equippedWeapons[activeSlot].EndPrimary();
        }
    }
    public void SetSecondaryFireStatus(bool status)
    {
        if(status)
        {
            equippedWeapons[activeSlot].StartSecondary();
        } else
        {
            equippedWeapons[activeSlot].EndSecondary();
        }
    }
    // Changing weapons, used by InputManager.cs and animation event
    public void ChangeWeapon(int slot) // Begin change weapon sequence (lower current weapon), used by InputManager.cs
    {
        //weaponBlocked = true;
        activeSlot = slot;
        animator.SetTrigger(Lower);
        equippedWeapons[activeSlot].animator = animator;
    }
    public void OnWeaponLowered() // Signal that new weapon is ready to raise, used by animation event
    {
        animator.runtimeAnimatorController = equippedWeapons[activeSlot].animatorOverride;
    }
    //---USED BY CheatsManager.cs---
    public void PrintWeaponInfo()
    {
        Debug.Log(equippedWeapons[activeSlot].displayName);
    }
}
