using System;
using UnityEngine;

/// <summary>
/// Handles player health, hard damage and dying
/// </summary>
public class PlayerHealth : Singleton<PlayerHealth>
{
    [Header("Stats")]
    public int maxBlood;
    public float damageReduction = 0; //0 means 0%, 1 means 100%. Modify this additively please.
    public bool isInvulnerable = false;
    [Header("References")]
    public int CurrentBlood { get; private set; } //as in: current blood
    public int CurrentBlackBile { get; private set; } //as in: current black bile
    public bool IsDead { get; private set; }
    
    private void Start()
    {
        CurrentBlood = maxBlood;
        CurrentBlackBile = 0;
    }
    public void TakeDamage(int damage)
    {
        if(isInvulnerable) return;
        CurrentBlood -= Mathf.RoundToInt(damage * (1 - damageReduction));
        if(CurrentBlood <= 0)
        {
            Die();
        }
    }
    public void GainBlood(int amount)
    {
        CurrentBlood += Math.Clamp(amount, 0, maxBlood - CurrentBlackBile - CurrentBlood);
    }
    public void TakeHardDamage(int damage) //as in: black bile
    {
        if(isInvulnerable) return;
        CurrentBlackBile += damage;
        CurrentBlood = Math.Clamp(CurrentBlood, 0, maxBlood - CurrentBlackBile);
        if(CurrentBlood <= 0)
        {
            Die();
        }
    }
    public void LoseHardDamage(int amount)
    {
        CurrentBlackBile -= amount;
        CurrentBlackBile = Math.Clamp(CurrentBlackBile, 0, maxBlood);
    }
    public void Die()
    {
        IsDead = true;
        PlayerUI.Instance.ShowDeathScreen();
        Debug.Log("You died!");
    }
    public  void Resurrect() // Resurrect and/ top of player health
    {
        if(IsDead)
        {
            IsDead = false;
            PlayerUI.Instance.HideDeathScreen();
        }
        CurrentBlackBile = 0;
        CurrentBlood = maxBlood;
    }
}
