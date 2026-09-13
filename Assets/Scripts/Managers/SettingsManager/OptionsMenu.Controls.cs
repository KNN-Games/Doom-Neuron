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
    private static readonly Dictionary<string, string> xboxButtonNames = new() // Why doesn't this shit format properly?
{
    { "buttonSouth", "<sprite name=\"xbox_a\">" },
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
    private static readonly System.Text.RegularExpressions.Regex camelCaseSpacer =
        new(@"(?<!^)([A-Z])", System.Text.RegularExpressions.RegexOptions.Compiled);
    private enum GamepadType { Xbox, PlayStation, Generic }
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
    private int FindBinding(InputAction action, string deviceGroup)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            string path = action.bindings[i].path;
            bool matches = deviceGroup == "<Keyboard>"
                ? path.StartsWith("<Keyboard>") || path.StartsWith("<Mouse>")
                : path.StartsWith(deviceGroup);

            if (matches) return i;
        }
        Debug.LogError("Input action path not found!");
        return -1;
    }
    private void CreateBindingSetting(InputAction action, string device, string key, RebindButton button)
    {
        int bindingIndex = FindBinding(action, device);
        new RebindSetting(key, action, bindingIndex, device, button); // Automatically adds to rebinds List
    }
    private void CreateBindingSettings(InputAction action, string keyBase, RebindButton button) // Creates a pair of rebind Settings
    {
        CreateBindingSetting(action, "<Keyboard>", keyBase + "Keyboard", button);
        CreateBindingSetting(action, "<Gamepad>", keyBase + "Gamepad", button);
    }
    private string GetBindingDisplayName(string bindingPath)
    {
        if (bindingPath.StartsWith("<Gamepad>/"))
        {
            string controlPath = bindingPath["<Gamepad>/".Length..];
            GamepadType type = DetectGamepadType();
            if (type == GamepadType.Xbox && xboxButtonNames.TryGetValue(controlPath, out string xboxName)) return xboxName;
            if (type == GamepadType.PlayStation && playstationButtonNames.TryGetValue(controlPath, out string psName)) return psName;
            return camelCaseSpacer.Replace(controlPath, " $1").Replace("/", " ").ToUpper();
        }
        if (bindingPath.StartsWith("<Keyboard>/"))
        {
            return bindingPath["<Keyboard>/".Length..].ToUpper();
        }
        if (bindingPath.StartsWith("<Mouse>/"))
        {
            string controlPath = bindingPath["<Mouse>/".Length..];
            return camelCaseSpacer.Replace(controlPath, " $1").ToUpper();
        }
        return bindingPath.ToUpper().Replace("/", " ");
    }
    private void BeginRebind(RebindSetting setting)
    {
        string cancelKey = setting.Device == "<Keyboard>" ? "<Keyboard>/escape" : "<Gamepad>/start";

        int bindingIndex = FindBinding(setting.Action, setting.Device);
        if (bindingIndex < 0) return;

        bool wasEnabled = setting.Action.enabled;
        setting.Action.Disable();

        setting.Action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsHavingToMatchPath(setting.Device)
            .WithCancelingThrough(cancelKey)
            .OnComplete(operation =>
            {
                Debug.Log($"Selected control: {operation.selectedControl.path} (device: {operation.selectedControl.device.name})");
                string newBinding = setting.Action.bindings[bindingIndex].overridePath;
                setting.CurrentValue = newBinding;
                Debug.Log($"{setting.Action.name} rebound to {newBinding}");
                setting.Button.UpdateButton(GetBindingDisplayName(newBinding), setting.NotDefault());
                operation.Dispose();
                if (wasEnabled) setting.Action.Enable();
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                if (wasEnabled) setting.Action.Enable();
                Debug.Log("Rebinding cancelled.");
            })
            .Start();
    }
    public void UpdateKeyRebindButtons()
    {
        bool isGamepad = playerInput.currentControlScheme == "Gamepad";
        string activeDevice = isGamepad ? "<Gamepad>" : "<Keyboard>";
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
    }
}