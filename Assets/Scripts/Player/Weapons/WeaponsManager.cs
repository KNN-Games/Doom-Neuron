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
    private bool primaryStatus = false; // made so that if player changes weapon while holding attack their input perservere

    private IEnumerator Start()
    {
        yield return null;
        RaiseNewWeapon();
    }
    // Shooting, used by InputManager.cs
    public void SetPrimaryFireStatus(bool status)
    {
        primaryStatus = status;
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
        if (slot == activeSlot) return;
        animator.SetTrigger(ChangeWeaponHash);
        activeSlot = slot;
    }
    public void NextWeapon()
    {
        ChangeWeapon((activeSlot + 1) % equippedWeapons.Length); // Wrap around if there is no next weapon
    }
    public void RaiseNewWeapon() // Used in animation event on the last frame of lowering weapon animation
    {
        // Swapping the controller resets the Animator to whatever state is marked "default" in the new controller.
        // Don't forget to mark the Raise animation as default is every weapon!
        animator.runtimeAnimatorController = CurrentWeapon.animatorController;

        if (primaryStatus)
        {
            CurrentWeapon.StartPrimary();
        } // the same can easily be added for secondary weapon, but since how different they can be I don't think the player would want that
    }
    //---USED BY CheatsManager.cs---
    public void PrintWeaponInfo()
    {
        Debug.Log(CurrentWeapon.displayName + "; " + CurrentWeapon.description);
    }
}
