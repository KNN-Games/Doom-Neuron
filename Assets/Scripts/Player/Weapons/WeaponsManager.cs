using System.Collections;
using UnityEngine;

/// <summary>
/// Handles everything about weapons: firing, equipping, changing active
/// </summary>
/// <remarks>
/// There are 3 active weapons.
/// </remarks>
public class WeaponsManager : Singleton<WeaponsManager>
{
    private static readonly int ChangeWeaponHash = Animator.StringToHash("Change");
    [Header("References")]
    public Weapon[] equippedWeapons;
    public GameObject firePoint; // Used by Weapon.cs
    public Animator animator;
    private int activeSlot = 0;
    private Weapon CurrentWeapon => equippedWeapons[activeSlot];

    private IEnumerator Start()
    {
        yield return null;
        RaiseNewWeapon();
    }
    //Shooting, used by InputManager.cs
    public void SetPrimaryFireStatus(bool status)
    {
        if (status)
        {
            CurrentWeapon.StartPrimary();
        }
        else
        {
            CurrentWeapon.EndPrimary();
        }
    }
    public void SetSecondaryFireStatus(bool status)
    {
        if (status)
        {
            CurrentWeapon.StartSecondary();
        }
        else
        {
            CurrentWeapon.EndSecondary();
        }
    }
    public void Attack() // Used by animation event
    {
        CurrentWeapon.Attack();
    }
    // Changing weapons, used by InputManager.cs and animation event
    public void ChangeWeapon(int slot) // Begin change weapon sequence (lower current weapon), used by InputManager.cs
    {
        animator.SetTrigger(ChangeWeaponHash);
        activeSlot = slot;
    }
    public void NextWeapon()
    {
        ChangeWeapon(activeSlot + 1);
    }
    public void RaiseNewWeapon() // Used in animation event
    {
        animator.runtimeAnimatorController = CurrentWeapon.animatorController;
    }
    //---USED BY CheatsManager.cs---
    public void PrintWeaponInfo()
    {
        Debug.Log(CurrentWeapon.displayName);
    }
}
