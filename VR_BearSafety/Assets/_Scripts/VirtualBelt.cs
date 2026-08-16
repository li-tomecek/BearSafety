using UnityEngine;

public class VirtualBelt : MonoBehaviour
{
    [SerializeField] private Transform targetCamera;
    [SerializeField] private float heightOffset = -0.35f;

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        // Position: Follow X and Z of camera, keep fixed offset for Y
        Vector3 newPos = targetCamera.position;
        newPos.y += heightOffset;
        transform.position = newPos;

        // Rotation: Only copy the Y (yaw) rotation of the head, zero out pitch and roll
        Vector3 eulerRotation = targetCamera.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, eulerRotation.y, 0f);
    }
}
