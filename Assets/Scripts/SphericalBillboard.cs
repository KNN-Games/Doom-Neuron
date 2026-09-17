using UnityEngine;

/// <summary>
/// Rotates the whole sprite in all directions, always facing the player.
/// Uses: particle effects(?)
/// </summary>
public class SphericalBillboard : MonoBehaviour
{
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = PlayerController.Instance.camera.transform;
    }
    private void LateUpdate()
    {
        transform.forward = cameraTransform.forward;
    }
}
