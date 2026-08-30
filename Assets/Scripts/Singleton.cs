using UnityEngine;

/// <summary>
/// Most managers and player related stuff are Singletons to make references to them easier.
/// Instead of finding by tag or some other things like that just make a reference to these script by
/// ScriptName.Instance.FunctionName()
/// </summary>
/// <typeparam name="T">Ensure that the parameter is the same as class name, for example GameManager : Singleton<GameManager> </typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else if (Instance != this)
        {
            Debug.LogWarning($"Multiple instances of {typeof(T).Name} detected. Destroying duplicate."); //this will cause a lot of warnings when u go back to main menu
            Destroy(gameObject);
        }
    }
}