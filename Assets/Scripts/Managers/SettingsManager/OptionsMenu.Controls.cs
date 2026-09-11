using TMPro;
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
    private static readonly Dictionary<string, string> xboxButtonNames = new()
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
    };
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
    private int FindBinding(InputAction action, string device)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            string path = action.bindings[i].path;
            if (path.StartsWith(device)) return i;
        }
        Debug.LogError("Input action path not found!");
        return -1;
    }
    private StringSetting CreateBindingSetting(InputAction action, string device, string key)
    {
        int bindingIndex = FindBinding(action, device);
        return new StringSetting(
            key,
            action.bindings[bindingIndex].path,
            value => action.ApplyBindingOverride(bindingIndex, value));
    }
    private string GetBindingDisplayName(string bindingPath)
    {
        if (bindingPath.StartsWith("<Keyboard>/"))
        {
            return bindingPath.Replace("<Keyboard>/", string.Empty).ToUpper();
        }
        if (bindingPath.StartsWith("<Gamepad>/"))
        {
            string controlPath = bindingPath.Replace("<Gamepad>/", string.Empty);
            return GetGamepadButtonDisplayName(controlPath);
        }
        return bindingPath.ToUpper();
    }
    private string GetGamepadButtonDisplayName(string controlPath)
    {
        GamepadType type = DetectGamepadType();
        if (type == GamepadType.Xbox && xboxButtonNames.TryGetValue(controlPath, out string xboxName)) return xboxName;
        if (type == GamepadType.PlayStation && playstationButtonNames.TryGetValue(controlPath, out string psName)) return psName;
        // Unknown or generic gamepad, format the control name
        string spaced = System.Text.RegularExpressions.Regex.Replace(controlPath, "(?<!^)([A-Z])", " $1");
        return spaced.ToUpper();
    }
    private void BeginRebind(InputAction action, StringSetting setting, bool isForKeyboard, TextMeshProUGUI buttonText)
    {
        string device = string.Empty;
        string cancelKey = string.Empty;
        if (isForKeyboard)
        {
            device = "<Keyboard>";
            cancelKey = "<Keyboard>/escape";
        }
        else // assume it's for Gamepad
        {
            device = "<Gamepad>";
            cancelKey = "<Gamepad>/start";
        }
        int bindingIndex = FindBinding(action, device);
        if (bindingIndex < 0) return;

        bool wasEnabled = action.enabled;
        action.Disable();

        // Oh my goodness gracious
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.InputActionRebindingExtensions.RebindingOperation.html
        action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsHavingToMatchPath(device)
            .WithCancelingThrough(cancelKey)
            .OnComplete(operation =>
            {
                string newBinding = action.bindings[bindingIndex].overridePath;
                setting.CurrentValue = newBinding;
                Debug.Log($"{action.name} rebound to {newBinding}");
                buttonText.text = GetBindingDisplayName(newBinding);
                operation.Dispose();
                if (wasEnabled) action.Enable();
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                if (wasEnabled) action.Enable();
                Debug.Log("Rebinding cancelled.");
            })
            .Start();
    }
    private void UpdateKeyRebindButtons()
    {
        // Check what device is detected. Game only supports gamepad and keyboardMouse, so either detected gamepad or use default keyboard
        jumpButton.onClick.RemoveAllListeners();
        interactButton.onClick.RemoveAllListeners();

        Debug.Log(playerInput.currentControlScheme);
        if (playerInput.currentControlScheme == "Gamepad")
        {
            deviceDetectedLocalizedText.StringReference.TableEntryReference = "GAMEPAD DETECTED";
            jumpButton.onClick.AddListener(() => RebindJump(false));
            interactButton.onClick.AddListener(() => RebindInteract(false));
            jumpButtonText.text = GetBindingDisplayName(jumpGamepad.CurrentValue);
            interactButtonText.text = GetBindingDisplayName(interactGamepad.CurrentValue);
        }
        else
        {
            deviceDetectedLocalizedText.StringReference.TableEntryReference = "KEYBOARD AND MOUSE DETECTED";
            jumpButton.onClick.AddListener(() => RebindJump(true));
            interactButton.onClick.AddListener(() => RebindInteract(true));
            jumpButtonText.text = GetBindingDisplayName(jumpKeyboard.CurrentValue);
            interactButtonText.text = GetBindingDisplayName(interactKeyboard.CurrentValue);
        }
        deviceDetectedLocalizedText.RefreshString();
    }
}