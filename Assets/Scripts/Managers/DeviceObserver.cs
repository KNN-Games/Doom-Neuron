using System;
using System.Linq;
using UnityEngine.InputSystem;

public enum InputDeviceType
{
    None,
    KeyboardMouse,
    Gamepad,
    Other
}

// Thank you Swagamaleous (reddit) for this code
// I might integrate this into game manager later
public class DeviceObserver : Singleton<DeviceObserver>, IObserver<InputControl>, IDisposable
{
    public InputDeviceType ActiveDeviceType => GetDeviceType(ActiveDevice);
    public InputDevice ActiveDevice { get; private set; }
    private IDisposable _subscription;
    protected override void Awake()
    {
        base.Awake();

        // initialize with first device in list, probably you want to make this more intelligent
        ActiveDevice = InputSystem.devices.FirstOrDefault();
        _subscription = InputSystem.onAnyButtonPress.Subscribe(this);
    }
    private void OnDestroy()
    {
        _subscription?.Dispose();
    }
    public void OnCompleted()
    {
    }
    public void OnError(Exception error)
    {
    }
    public void OnNext(InputControl value)
    {
        ActiveDevice = value.device;
    }
    public void Dispose()
    {
        _subscription?.Dispose();
    }
    private InputDeviceType GetDeviceType(InputDevice device)
    {
        if (device == null)
        {
            UnityEngine.Debug.LogError("NO DEVICE DETECTED");
            return InputDeviceType.None;
        }

        if (device is Keyboard || device is Mouse) return InputDeviceType.KeyboardMouse;
        if (device is Gamepad) return InputDeviceType.Gamepad;
        
        UnityEngine.Debug.LogError("UNSUPPORTED DEVICE DETECTED");
        return InputDeviceType.Other;
    }
}