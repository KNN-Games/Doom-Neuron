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
    private bool IsPlayerPresent => PlayerController.Instance != null;
    private void Start()
    {
        commands = new() // List of commands
        {
            {"blood", AddHealth},
            {"hp", AddHealth},
            {"blackbile", AddHardDamage},
            {"divine", ToggleInvulnerability},
            {"suicide", Die},
            {"die", Die},
            {"settinginfo", PrintSettingInfo},
            {"saveinfo", PrintSaveInfo},
            {"whole", Resurrect},
            {"difficulty", SetDifficulty},
            {"diff", SetDifficulty}
        };
    }
    //---CONSOLE MANAGEMENT---
    public void ToggleConsole()
    {
        isOpen = !isOpen;
        consolePanel.SetActive(isOpen);
        if (IsPlayerPresent)
        {
            PlayerUI ui = PlayerUI.Instance;
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
        else if (isOpen)
        {
            inputField.ActivateInputField();
        }
    }
    public void SubmitCommand()
    {
        if (!isOpen) return;
        string command = inputField.text;
        ExecuteCommand(command);
        inputField.text = "";
        inputField.ActivateInputField();
    }
    private void ExecuteCommand(string input)
    {
        Write("> " + input);
        string[] parts = input.Split(' ');
        string command = parts[0];
        string[] args = parts[1..];
        if (commands.TryGetValue(command, out var action))
        {
            action(args);
        }
        else
        {
            Write("Unknown command");
        }
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
    private void Write(string text) // This ensures that you don't make a format mistake while writing something to console
    {
        outputText.text += text + "\n";
    }
    //---COMMMANDS---
    private void AddHealth(string[] args) // Add blood (as in: HP) to the player. Usage: blood <amount>
    {
        if (!IsPlayerPresent)
        {
            Write("You cannot heal yourself when you don't exist");
            return;
        }
        if (args.Length == 0)
        {
            Write("Usage: blood <amount>");
            return;
        }
        int amount = int.Parse(args[0]);
        PlayerHealth.Instance.GainBlood(amount);
        Write($"Added {amount} HP");
    }
    private void AddHardDamage(string[] args) // Add black bile (as in: black damage) to the player. Usage: blackbile <amount>
    {
        if (!IsPlayerPresent)
        {
            Write("You cannot give yourself black bile when you don't exist");
            return;
        }
        if (args.Length == 0)
        {
            Write("Usage: blackbile <amount>");
            return;
        }
        int amount = int.Parse(args[0]);
        PlayerHealth.Instance.TakeHardDamage(amount);
        Write($"Added {amount} black bile");
    }
    private void Die(string[] args) // Instantly kills the player. Usage: suicide
    {
        if (!IsPlayerPresent)
        {
            Write("You cannot die when you don't exist");
            return;
        }
        ToggleConsole(); // Close console, I can't bother to make console + death screen compatible
        PlayerHealth.Instance.Die();
    }
    private void Resurrect(string[] args) // Revives the player
    {
        if (!IsPlayerPresent)
        {
            Write("You cannot be whole if you don't exist");
            return;
        }
        ToggleConsole(); // Close console for the same reason as in Die()
        PlayerHealth.Instance.Resurrect();
    }
    private void ToggleInvulnerability(string[] args) // Toggles invulnerability. Usage: invulnerable
    {
        if (!IsPlayerPresent)
        {
            Write("You cannot be invincible if you don't exist");
            return;
        }
        bool inv = PlayerHealth.Instance.isInvulnerable;
        inv = !inv;
        PlayerHealth.Instance.isInvulnerable = inv; // Ignores possible visual effects we may add later
        if (inv)
        {
            Write("Only an instant death can kill you now");
        }
        else
        {
            Write("You are mortal once more");
        }
    }
    private void PrintSettingInfo(string[] args)
    {
        OptionsMenu.Instance.PrintAllSettingValues();
        Write("Look in the console to see the values");
    }
    private void PrintSaveInfo(string[] args)
    {
        if (!IsPlayerPresent)
        {
            Write("You cannot print save info if you don't exist");
            return;
        }
        SaveManager.Instance.PrintCurrentSaveInfo();
        Write("Look in the console to see the values");
    }
    private void SetDifficulty(string[] args) // Sets the difficulty. Usage: difficulty <level>
    {
        if (args.Length == 0 || !int.TryParse(args[0], out int level) || level < 1 || level > 3)
        {
            Write("Usage: difficulty <level>");
            Write("1 - Easy, 2 - Medium, 3 - Hard");
            return;
        }

        GameManager.Instance.SetDifficulty(level);
        Write($"Difficulty set to {GameManager.Instance.Difficulty}");
    }
}
