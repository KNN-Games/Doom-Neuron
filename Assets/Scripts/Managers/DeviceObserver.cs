using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

//Thank you Swagamaleous (reddit) for this code
public class DeviceObserver : Singleton<MonoBehaviour>, IObserver<InputControl>, IDisposable
{
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
}