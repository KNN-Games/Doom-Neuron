using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Base class for every weapon prefab. Each weapon is its own prefab with its own Animator
/// and its own subclass of this script - some duplicated code between subclasses is expected.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [Header("Identity")]
    public string displayName;
    public string description;
    public AnimatorController animatorController;
    // References
    protected Transform firePoint;
    protected Camera playerCamera;
    protected Animator animator;
    protected static readonly int FirePrimaryHash = Animator.StringToHash("FirePrimary"); // Bool

    public virtual void StartPrimary()
    {
        animator.SetBool(FirePrimaryHash, true);
    }
    public virtual void EndPrimary()
    {
        animator.SetBool(FirePrimaryHash, false);
    }
    public abstract void StartSecondary();
    public abstract void EndSecondary();
    public abstract void Attack(); // Used in animation events

    protected virtual void Start()
    {
        firePoint = WeaponsManager.Instance.firePoint.transform;
        animator = WeaponsManager.Instance.animator;
        playerCamera = PlayerController.Instance.camera;
    }
    // Generic methods you can use to calculate stuff
    protected Vector3 CalculateDirectionVectorWithCamera(float inaccuracyInDegrees) // Calculate velocity of the projectile - that means "where it should go"
    {
        Vector3 dirFromCam = GetSpreadDirectionFromCamera(inaccuracyInDegrees); // direction from camera
        Ray ray = new(firePoint.transform.position, dirFromCam); // Ray from firePoint pointing in direction from camera
        Vector3 targetPoint = ray.origin + ray.direction * 100f; // Point in ray where the projectile should go to.
        return (targetPoint - firePoint.position).normalized; // Final direction
    }
    private Vector3 GetSpreadDirectionFromCamera(float angleDeg) // Generate random not-perfect accuracy
    {
        // Picks a random direction inside a cone around the camera's forward vector.
        // angleDeg = 0 always returns the exact forward direction (no spread).
        float angleRad = angleDeg * Mathf.Deg2Rad;

        // Random angle around the cone's axis (0-360 deg)
        float theta = Random.Range(0f, Mathf.PI * 2f);
        // Random distance from the cone's axis, biased so the cone fills evenly (not just its edge)
        float z = Mathf.Cos(angleRad * Random.value);
        float r = Mathf.Sqrt(1 - z * z);

        float x = r * Mathf.Cos(theta);
        float y = r * Mathf.Sin(theta);

        // z is "forward" in this local space, x/y are the sideways/up offset
        Vector3 localDir = new(x, y, z);

        // Rotate the local cone direction into world space using the camera's orientation
        return playerCamera.transform.TransformDirection(localDir).normalized;
    }
}