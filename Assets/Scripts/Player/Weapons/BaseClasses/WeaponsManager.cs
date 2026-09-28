using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles everything about weapons: firing, equipping, changing active
/// </summary>
/// <remarks>
/// There are 3 active weapons. The player starts unarmed - all slots are null until PickUpWeapon fills them.
/// This code will ready kinda weird until we decide if we really want to only allow 3 weapons at a time instead of every single one like other boomer shooters
/// TO DO: Maybe add a default weapon? Like a fist or something
/// </remarks>
public class WeaponsManager : Singleton<WeaponsManager>
{
    private static readonly int ChangeWeaponHash = Animator.StringToHash("Change");

    [Header("References")]
    public GameObject firePoint; // Used by Weapon.cs
    public Animator animator;
    [SerializeField] private GameObject weaponRenderer;
    private readonly Weapon[] equippedWeapons = new Weapon[3];
    private int activeSlot = 0;
    private Weapon CurrentWeapon => equippedWeapons[activeSlot]; // Can be null - the player may be unarmed
    private bool primaryStatus = false; // made so that if player changes weapon while holding attack their input perservere

    private IEnumerator Start()
    {
        yield return null;
        RaiseNewWeapon();
    }
    // Shooting, used by InputManager.cs
    public void SetPrimaryFireStatus(bool status)
    {
        primaryStatus = status; // Remembered even while unarmed, so picking up a weapon while holding the button resumes firing
        if (CurrentWeapon == null) return;
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
        if (CurrentWeapon == null) return;
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
        if (CurrentWeapon != null) CurrentWeapon.Attack();
    }
    // Changing weapons, used by InputManager.cs and animation event
    public void ChangeWeapon(int slot) // Begin change weapon sequence (lower current weapon), used by InputManager.cs
    {
        if (slot == activeSlot || equippedWeapons[slot] == null) return; // Same slot, or nothing equipped there

        bool hadWeapon = CurrentWeapon != null; // Is there something to play a lowering animation for?
        activeSlot = slot;

        if (hadWeapon)
        {
            animator.SetTrigger(ChangeWeaponHash); // Plays the lowering clip, which raises the new weapon via an animation event
        }
        else
        {
            RaiseNewWeapon(); // Was unarmed - nothing to lower, just raise the new weapon directly
        }
    }
    public void NextWeapon()
    {
        // Skips empty slots - equippedWeapons can have gaps while the player is still finding weapons
        for (int step = 1; step <= equippedWeapons.Length; step++)
        {
            int slot = (activeSlot + step) % equippedWeapons.Length;
            if (equippedWeapons[slot] != null)
            {
                ChangeWeapon(slot);
                return;
            }
        }
        // No other weapon equipped - stay on the current one (possibly still unarmed)
    }
    public void PickUpWeapon(Weapon weaponPrefab) // add a new weapon to equippedWeapons. Discard if not enough place.
    {
        if (weaponPrefab == null) return;

        int emptySlot = Array.IndexOf(equippedWeapons, null);
        if (emptySlot == -1)
        {
            Debug.Log($"No room for {weaponPrefab.displayName}"); // Discard - no free slot
            return;
        }

        // Instatiate weapon prefab
        Weapon weapon = Instantiate(weaponPrefab, transform);

        bool wasUnarmed = CurrentWeapon == null;
        equippedWeapons[emptySlot] = weapon;

        if (wasUnarmed) // Found first weapon
        {
            activeSlot = emptySlot;
            RaiseNewWeapon();
        }
    }
    public void RaiseNewWeapon() // Used in animation event on the last frame of lowering weapon animation
    {
        if (CurrentWeapon == null)
        {
            // Unarmed - nothing to show
            animator.runtimeAnimatorController = null;
            weaponRenderer.SetActive(false);
            return;
        }
        weaponRenderer.SetActive(true);
        // Swapping the controller resets the Animator to whatever state is marked "default" in the new controller.
        animator.runtimeAnimatorController = CurrentWeapon.animatorController;

        if (primaryStatus)
        {
            CurrentWeapon.StartPrimary();
        } // the same can easily be added for secondary weapon, but since how different they can be I don't think the player would want that
    }
    //---USED BY CheatsManager.cs---
    public void PrintWeaponInfo()
    {
        Debug.Log(CurrentWeapon != null ? $"{CurrentWeapon.displayName}; {CurrentWeapon.description}" : "Unarmed.");
    }
}