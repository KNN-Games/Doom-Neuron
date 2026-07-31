using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CheatsManager : Singleton<CheatsManager>
{
    public bool IsConsoleActive => consolePanel.activeSelf;
    [Header("References")]
    [SerializeField] private GameObject consolePanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private ScrollRect scrollRect;
    private Dictionary<string, System.Action<string[]>> commands;
    private bool isOpen;
    private void Start()
    {
        commands = new() //list of commands
        {
            {"blood", AddBlood},
            {"blackbile", AddBlackBile},
            {"divine", ToggleInvulnerability},
            {"suicide", Die}
        };
    }
    //---HANDLE INPUT---
    public void OnOpenConsole(InputAction.CallbackContext context)
    {
        if (!context.started || SceneManager.GetActiveScene().name == "MainMenu") return;
        ToggleConsole();
    }
    public void OnSubmitCommand(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        SubmitCommand();
    }
    private void ToggleConsole()
    {
        isOpen = !isOpen;
        consolePanel.SetActive(isOpen);
        if (isOpen) //open console
        {
            inputField.ActivateInputField();
            PlayerUI.Instance.PauseGame();
        }
        else if (PlayerUI.Instance.isPaused)//close console
        {
            PlayerUI.Instance.UnpauseGame();
        }
    }
    private void SubmitCommand()
    {
        string command = inputField.text;
        ExecuteCommand(command);
        inputField.text = "";
        inputField.ActivateInputField();
    }
    private void ExecuteCommand(string input)
    {
        outputText.text += "> " + input + "\n";
        string[] parts = input.Split(' ');
        string command = parts[0];
        string[] args = parts[1..];
        if (commands.TryGetValue(command, out var action))
        {
            action(args);
        }
        else
        {
            outputText.text += "Unknown command\n";
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
    //---COMMMANDS---
    private void AddBlood(string[] args)
    {
        if (args.Length == 0)
        {
            outputText.text += "Usage: blood <amount>\n";
            return;
        }
        int amount = int.Parse(args[0]);
        PlayerHealth.Instance.GainBlood(amount);
        outputText.text += $"Added {amount} HP\n";
    }
    private void AddBlackBile(string[] args)
    {
        if (args.Length == 0)
        {
            outputText.text += "Usage: blackbile <amount>\n";
            return;
        }
        int amount = int.Parse(args[0]);
        PlayerHealth.Instance.TakeBlackDamage(amount);
        outputText.text += $"Added {amount} black bile\n";
    }
    private void Die(string[] args)
    {
        PlayerHealth.Instance.Die();
    }
    private void ToggleInvulnerability(string[] args) //ignores possible visual effects we may add later
    {
        bool inv = PlayerHealth.Instance.isInvulnerable;
        inv = !inv;
        PlayerHealth.Instance.isInvulnerable = inv;
        if(inv)
        {
            outputText.text += "Only an instant death can kill you now\n";
        } else
        {
            outputText.text += "You are mortal once more\n";
        }
    }
}
