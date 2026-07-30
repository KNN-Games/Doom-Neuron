using UnityEngine;

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