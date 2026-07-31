using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerHealth : Singleton<PlayerHealth>
{
    [Header("Stats")]
    public int maxBlood;
    [Header("References")]
    public GameObject deathScreen;
    [HideInInspector] public int CurrentBlood { get; private set; } //as in: current blood
    [HideInInspector] public int CurrentBlackBile { get; private set; } //as in: current black bile
    public bool IsGooped => CurrentBlackBile > 0;
    private PlayerInput input;
    void Start()
    {
        CurrentBlood = maxBlood;
        CurrentBlackBile = 0;
        input = GetComponent<PlayerInput>();
    }
    public void TakeDamage(int damage)
    {
        CurrentBlood -= damage;
        if(CurrentBlood <= 0)
        {
            Die();
        }
    }
    public void GainBlood(int amount)
    {
        CurrentBlood += Math.Clamp(amount, 0, maxBlood - CurrentBlackBile - CurrentBlood);
    }
    public void TakeBlackDamage(int damage) //as in: black bile
    {
        CurrentBlackBile += damage;
        CurrentBlood = Math.Clamp(CurrentBlood, 0, maxBlood - CurrentBlackBile);
        if(CurrentBlood <= 0)
        {
            Die();
        }
    }
    public void LoseBlackDamage(int amount)
    {
        CurrentBlackBile -= amount;
        CurrentBlackBile = Math.Clamp(CurrentBlackBile, 0, maxBlood);
    }
    public void Die()
    {
        deathScreen.SetActive(true);
        PlayerUI.Instance.hud.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        input.currentActionMap.Disable();
        Time.timeScale = 0f;
        Debug.Log("You died!");
    }
}
