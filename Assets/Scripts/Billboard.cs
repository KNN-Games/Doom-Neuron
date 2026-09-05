using UnityEngine;

/// <summary>
/// Gives the object the classic DOOM "2D sprite in a 3D world" effect
/// </summary>
public class Billboard : MonoBehaviour
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