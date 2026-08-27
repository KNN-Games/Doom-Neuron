using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles cheat console when in level - player must exist for this to work.
/// Activate the console with ` key and type commands to execute cheats.
/// </summary>
public class CheatsManager : Singleton<CheatsManager>
{
    public bool IsConsoleActive => consolePanel.activeSelf;
    [Header("References")]
    [SerializeField] private GameObject consolePanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private ScrollRect scrollRect;
    // Dictionary to hold command names and their corresponding actions
    private Dictionary<string, System.Action<string[]>> commands;
    private bool isOpen = false;
    
    private void Start()
    {
        commands = new() // List of commands
        {
            {"blood", AddBlood},
            {"blackbile", AddBlackBile},
            {"divine", ToggleInvulnerability},
            {"suicide", Die},
        };
    }
    //---CONSOLE MANAGEMENT---
    public void ToggleConsole()
    {
        PlayerUI ui = PlayerUI.Instance;
        isOpen = !isOpen;
        consolePanel.SetActive(isOpen);
        if (isOpen) // Open console
        {
            inputField.ActivateInputField();
            ui.PauseGame();
        }
        else if (ui.isPaused && !ui.pauseMenu.activeSelf) // Close console
        {
            ui.UnpauseGame();
        }
    }
    public void SubmitCommand()
    {
        if(!isOpen) return;
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
    private void AddBlood(string[] args) // Add blood (as in: HP) to the player. Usage: blood <amount>
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
    private void AddBlackBile(string[] args) // Add black bile (as in: black damage) to the player. Usage: blackbile <amount>
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
    private void Die(string[] args) // Instantly kills the player. Usage: suicide
    {
        ToggleConsole(); // Close console, I can't bother to make console + death screen compatible
        PlayerHealth.Instance.Die();
    }
    private void ToggleInvulnerability(string[] args) // Toggles invulnerability. Usage: invulnerable
    {
        bool inv = PlayerHealth.Instance.isInvulnerable;
        inv = !inv;
        PlayerHealth.Instance.isInvulnerable = inv; // Ignores possible visual effects we may add later
        if(inv)
        {
            outputText.text += "Only an instant death can kill you now\n";
        } else
        {
            outputText.text += "You are mortal once more\n";
        }
    }
}
