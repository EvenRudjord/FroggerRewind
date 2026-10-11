using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;      // Assign your player transform in Inspector
    public float yOffset = 2f;    // Vertical offset for the camera
    public float smoothSpeed = 5f; // How fast the camera follows

    private float initialX;        // To keep the camera's initial X position

    void Start()
    {
        // Store the initial X position of the camera so it stays fixed
        initialX = transform.position.x;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Target Y position with offset
        float targetY = player.position.y + yOffset;

        // Smoothly interpolate the camera's Y position towards the target
        float smoothY = Mathf.Lerp(transform.position.y, targetY, smoothSpeed * Time.deltaTime);

        // Update camera position: keep X fixed, smooth Y, keep Z as is
        transform.position = new Vector3(initialX, smoothY, transform.position.z);
    }
}
