using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles player health, hard damage and dying
/// </summary>
/// <remarks>
/// We could easily change lose health + gain health into a single function, but I fear this might make this just more confusing.
/// Same with black bile
/// </remarks>
public class PlayerHealth : Singleton<PlayerHealth>, IDamageable
{
    [Header("References")]
    [SerializeField] private Image bloodBar;
    [SerializeField] private Image blackBileBar;
    [Header("Stats")]
    public int maxBlood;
    public bool isInvulnerable = false;
    // Health related values
    public int CurrentBlood { get; private set; } // this is player health/mana (that's the same thing in this game really)
    public int CurrentBlackBile { get; private set; } // alternative names: black damage, hard damage
    public bool IsDead { get; private set; }
    private float damageReduction = 0; //0 means 0%, 1 means 100% (invincibility).

    private void Start()
    {
        CurrentBlood = maxBlood;
        CurrentBlackBile = 0;
        UpdateHealthBar();
    }
    // Health
    public void TakeDamage(int damage)
    {
        if (isInvulnerable || IsDead || damage <= 0) return;
        CurrentBlood -= Mathf.RoundToInt(damage * (1 - damageReduction));
        CurrentBlood = Mathf.Max(0, CurrentBlood); // Never go negative
        UpdateHealthBar();
        if (CurrentBlood <= 0) Die();
    }
    public void RegenerateHealth(int amount)
    {
        if(amount <= 0) return;
        // Maybe we could make it so that RegenerateHealth() while dead causes Resurrect()? But that would require for death to not cause pause.
        CurrentBlood += Math.Clamp(amount, 0, maxBlood - CurrentBlackBile - CurrentBlood);
        UpdateHealthBar();
    }
    // Hard damage/black damage/black bile. We really should decide on the name!
    public void TakeHardDamage(int damage)
    {
        if (isInvulnerable || IsDead || damage <= 0) return;
        CurrentBlackBile = Math.Clamp(CurrentBlackBile + damage, 0, maxBlood); // Keep bile in valid range
        CurrentBlood = Math.Clamp(CurrentBlood, 0, maxBlood - CurrentBlackBile);
        UpdateHealthBar();
        if (CurrentBlood <= 0) Die();
    }
    public void LoseHardDamage(int amount)
    {
        if(amount <= 0) return;
        CurrentBlackBile -= amount;
        CurrentBlackBile = Math.Clamp(CurrentBlackBile, 0, maxBlood);
        UpdateHealthBar();
    }
    // Health bar
    private void UpdateHealthBar()
    {
        bloodBar.fillAmount = CurrentBlood / (float)maxBlood;
        blackBileBar.fillAmount = CurrentBlackBile / (float)maxBlood;
    }
    // Death
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        PlayerUI.Instance.ShowDeathScreen();
        Debug.Log("You died!");
    }
    public void Resurrect() // Resurrect and/ top of player health
    {
        if (IsDead)
        {
            IsDead = false;
            PlayerUI.Instance.HideDeathScreen();
        } // Resurrection if player is not dead just makes you max out health
        CurrentBlackBile = 0;
        CurrentBlood = maxBlood;
    }
    // Damage reduction
    public void ChangeDamageReduction(float value)
    {
        damageReduction = Math.Clamp(damageReduction + value, 0, 1);
        if (damageReduction >= 1f)
        {
            Debug.LogWarning("damage reduction made the player invincible. Is this intended?");
        }
    }
}
