using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using System.Collections.Generic;
using System;

/// <summary>
/// This file contains the logic related to "Controls" section of settings in the OptionsMenu.
/// </summary>
public partial class OptionsMenu
{
    // These dictionaries hold "the input name that Unity will detect", "what to write in the TMPRO text"
    private static readonly Dictionary<string, string> xboxButtonNames = new() // Why doesn't this shit format properly?
{
    { "buttonSouth", "<sprite name=\"xbox_a\">" }, // If we update the engine I'm pretty sure we can [SerializeField] this instead
    { "buttonNorth", "<sprite name=\"xbox_y\">" },
    { "buttonEast", "<sprite name=\"xbox_b\">" },
    { "buttonWest", "<sprite name=\"xbox_x\">" },
    { "leftShoulder", "<sprite name=\"xbox_lb\">" },
    { "rightShoulder", "<sprite name=\"xbox_rb\">" },
    { "leftTrigger", "<sprite name=\"xbox_lt\">" },
    { "rightTrigger", "<sprite name=\"xbox_rt\">" },
    { "leftStickPress", "<sprite name=\"xbox_ls\">" },
    { "rightStickPress", "<sprite name=\"xbox_rs\">" },
    { "start", "<sprite name=\"xbox_menu\">" },
    { "select", "<sprite name=\"xbox_view\">" },
    { "dpad/up", "<sprite name=\"xbox_dpad_up\">" },
    { "dpad/down", "<sprite name=\"xbox_dpad_down\">" },
    { "dpad/left", "<sprite name=\"xbox_dpad_left\">" },
    { "dpad/right", "<sprite name=\"xbox_dpad_right\">" },
};
    private static readonly Dictionary<string, string> playstationButtonNames = new()
{
    { "buttonSouth", "<sprite name=\"ps_cross\">" },
    { "buttonNorth", "<sprite name=\"ps_triangle\">" },
    { "buttonEast", "<sprite name=\"ps_circle\">" },
    { "buttonWest", "<sprite name=\"ps_square\">" },
    { "leftShoulder", "<sprite name=\"ps_l1\">" },
    { "rightShoulder", "<sprite name=\"ps_r1\">" },
    { "leftTrigger", "<sprite name=\"ps_l2\">" },
    { "rightTrigger", "<sprite name=\"ps_r2\">" },
    { "leftStickPress", "<sprite name=\"ps_l3\">" },
    { "rightStickPress", "<sprite name=\"ps_r3\">" },
    { "start", "<sprite name=\"ps_options\">" },
    { "select", "<sprite name=\"ps_share\">" },
    { "dpad/up", "<sprite name=\"ps_dpad_up\">" },
    { "dpad/down", "<sprite name=\"ps_dpad_down\">" },
    { "dpad/left", "<sprite name=\"ps_dpad_left\">" },
    { "dpad/right", "<sprite name=\"ps_dpad_right\">" },
};
    // Do this thing to a string to make change "ExampleExample" to "EXAMPLE EXAMPLE"
    private static readonly System.Text.RegularExpressions.Regex camelCaseSpacer =
        new(@"(?<!^)([A-Z])", System.Text.RegularExpressions.RegexOptions.Compiled);
    private enum GamepadType { Xbox, PlayStation, Generic }
    private enum DeviceType { Keyboard, Gamepad }
    private static string DevicePath(DeviceType device) => device == DeviceType.Keyboard ? "<Keyboard>" : "<Gamepad>"; // Convert DeviceType to string

    // Detect what type of gamepad player uses: xbox, playstation or generic. Used in checking which sprite to use in GetBindingDisplayName(string bindingPath)
    private GamepadType DetectGamepadType()
    {
        var pad = Gamepad.current;
        if (pad == null) return GamepadType.Generic;

        // Check the type of the gamepad using the Input System's device classes
        if (pad is XInputController) return GamepadType.Xbox;
        if (pad is DualShockGamepad) return GamepadType.PlayStation;

        // Fallback: check by name/manufacturer reported by the system
        string name = pad.device.description.product ?? string.Empty;
        string manufacturer = pad.device.description.manufacturer ?? string.Empty;

        if (name.Contains("Xbox", StringComparison.OrdinalIgnoreCase)) return GamepadType.Xbox;

        // Check for PlayStation controllers by name or manufacturer
        if (name.Contains("DualSense", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Wireless Controller", StringComparison.OrdinalIgnoreCase) ||
            manufacturer.Contains("Sony", StringComparison.OrdinalIgnoreCase))
            return GamepadType.PlayStation;

        return GamepadType.Generic;
    }
    private string GetBindingDisplayName(string bindingPath) // Input string example: "<Gamepad>/ButtonSouth"
    {
        if (bindingPath.StartsWith("<Gamepad>/"))
        {
            string controlPath = bindingPath["<Gamepad>/".Length..]; // That means "delete "<Gamepad>/" from the string. VS code auto-changes .Substring() into this
            GamepadType type = DetectGamepadType();
            if (type == GamepadType.Xbox && xboxButtonNames.TryGetValue(controlPath, out string xboxName)) return xboxName;
            if (type == GamepadType.PlayStation && playstationButtonNames.TryGetValue(controlPath, out string psName)) return psName;
            return camelCaseSpacer.Replace(controlPath, " $1").Replace("/", " ").ToUpper();
        }
        if (bindingPath.StartsWith("<Keyboard>/"))
        {
            // TO DO: Maybe add camelCaseSpacer here too? Check if needed
            return bindingPath["<Keyboard>/".Length..].ToUpper();
        }
        if (bindingPath.StartsWith("<Mouse>/"))
        {
            string controlPath = bindingPath["<Mouse>/".Length..];
            return camelCaseSpacer.Replace(controlPath, " $1").ToUpper();
        }
        return bindingPath.ToUpper().Replace("/", " ");
    }
    /// <summary>
    /// Find the index of the exact binding based on device
    /// </summary>
    /// <param name="action"></param>
    /// <param name="deviceGroup">"Keyboard" or "Gamepad"</param>
    /// <returns>the index</returns>
    /// <remarks>
    /// most action bindings look like this (exapmle: Interact)
    /// Interact (action)
    /// [0] <Keyboard/e> on device Keyboard or Mouse (treat keyboard and mouse as the same device. No one sane plays mouse + gamepad anyway)
    /// [1] <Gamepad/ButtonWest> on device Gamepad
    /// But the order may be reversed, we don't know from the code if [0] is for keyboard or gamepad, so you need to find it
    /// </remarks>
    private int FindBinding(InputAction action, DeviceType device)
    {
        string deviceGroup = DevicePath(device);
        for (int i = 0; i < action.bindings.Count; i++)
        {
            string path = action.bindings[i].path;
            bool matches = device == DeviceType.Keyboard
                ? path.StartsWith("<Keyboard>") || path.StartsWith("<Mouse>")
                : path.StartsWith(deviceGroup);

            if (matches) return i;
        }
        Debug.LogError("Input action path not found!");
        return -1;
    }
    /// <summary>
    /// Create one RebindSetting with the specified device. Do not use directly.
    /// </summary>
    private void CreateBindingSetting(InputAction action, DeviceType device, string key, RebindButton button)
    {
        int bindingIndex = FindBinding(action, device);
        new RebindSetting(key, action, bindingIndex, device, button);
    }
    /// <summary>
    /// Create a pair of RebindSetting, one for Keyboard and one for gamepad
    /// </summary>
    private void CreateBindingSettings(InputAction action, string keyBase, RebindButton button)
    {
        CreateBindingSetting(action, DeviceType.Keyboard, keyBase + "Keyboard", button);
        CreateBindingSetting(action, DeviceType.Gamepad, keyBase + "Gamepad", button);
    }
    /// <summary>
    /// Perform an interactive rebind
    /// </summary>
    /// <param name="setting">The setting that holds the action you want to rebind</param>
    private void BeginRebind(RebindSetting setting)
    {
        string cancelKey = setting.Device == DeviceType.Keyboard ? "<Keyboard>/escape" : "<Gamepad>/start";

        int bindingIndex = FindBinding(setting.Action, setting.Device);
        if (bindingIndex < 0) return;

        bool wasEnabled = setting.Action.enabled;
        ChangeBindingsText(true);
        setting.Action.Disable();

        // Perform an interactive rebind. Man, just look at the Unity documentation
        setting.Action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsHavingToMatchPath(DevicePath(setting.Device))
            .WithCancelingThrough(cancelKey)
            .OnComplete(operation =>
            {
                string newBinding = setting.Action.bindings[bindingIndex].overridePath;
                setting.CurrentValue = newBinding;
                Debug.Log($"{setting.Action.name} rebound to {newBinding}");
                setting.Button.UpdateButton(GetBindingDisplayName(newBinding), setting.NotDefault());
                operation.Dispose();
                if (wasEnabled) setting.Action.Enable();
                ChangeBindingsText(false);
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                if (wasEnabled) setting.Action.Enable();
                ChangeBindingsText(false);
                Debug.Log("Rebinding cancelled.");
            })
            .Start();
    }
    private void ChangeBindingsText(bool rebindStatus)
    {
        string newText = rebindStatus ? "REBIND" : "CHANGE BINDINGS";
        changeBindingsLocalizedText.StringReference.TableEntryReference = newText;
        changeBindingsLocalizedText.RefreshString();
    }
    public void UpdateKeyRebindButtons()
    {
        bool isGamepad = playerInput.currentControlScheme == "Gamepad";
        // Handle Rebind section
        DeviceType activeDevice = isGamepad ? DeviceType.Gamepad : DeviceType.Keyboard;
        deviceDetectedLocalizedText.StringReference.TableEntryReference = isGamepad ? "GAMEPAD DETECTED" : "KEYBOARD AND MOUSE DETECTED";
        foreach (var rebind in rebinds)
        {
            if (rebind.Device != activeDevice) continue;

            rebind.Button.ChangeOnClickAction(
                () => BeginRebind(rebind),
                () => ResetSetting(rebind.Key));
            rebind.Button.UpdateButton(GetBindingDisplayName(rebind.CurrentValue), rebind.NotDefault());
        }
        deviceDetectedLocalizedText.RefreshString();
        // Decide which section (keyboard or gamepad) to show
        gamepadSection.SetActive(isGamepad);
        keyboardSection.SetActive(!isGamepad);
    }
}