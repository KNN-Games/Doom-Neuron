using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string displayName;
    public AnimatorOverrideController animatorOverride;
    //Used by weapons
    [HideInInspector] public Animator animator;
    protected static readonly int Secondary = Animator.StringToHash("FireSecondary");
    protected static readonly int Primary = Animator.StringToHash("FirePrimary");
    public abstract void StartPrimary();
    public abstract void EndPrimary();
    public abstract void StartSecondary();
    public abstract void EndSecondary();
}
