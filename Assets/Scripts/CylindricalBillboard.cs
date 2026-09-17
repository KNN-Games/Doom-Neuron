using UnityEngine;

/// <summary>
/// Gives the object the classic DOOM "2D sprite in a 3D world" effect.
/// Only rotates around the Y axis, so the sprite always stays upright.
/// Uses: enemies, map objects
/// </summary>
public class CylindricalBillboard : MonoBehaviour
{
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = PlayerController.Instance.camera.transform;
    }
    private void LateUpdate()
    {
        Vector3 direction = transform.position - cameraTransform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
