using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Instance to InputManager
    /// Camera Zoom with Mouse wheel
    /// Camera Reset with Mouse wheel click
    /// Camera Movement with WASD or Arrow-Keys
    /// Camera Rotation with Mouse wheel holding and moving right or loeft direction
    /// </summary>

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float maxZoomY = 20f;
    [SerializeField] private float minZoomY = 5f;

    [Header("Movement Borders Zoom Max")]
    [SerializeField] private float farMinX = -10f;
    [SerializeField] private float farMaxX = 10f;
    [SerializeField] private float farMinZ = -10f;
    [SerializeField] private float farMaxZ = 10f;

    [Header("Movement Borders Zoom Min")]
    [SerializeField] private float closeMinX = -10f;
    [SerializeField] private float closeMaxX = 10f;
    [SerializeField] private float closeMinZ = -10f;
    [SerializeField] private float closeMaxZ = 10f;

    private float minX, maxX, minZ, maxZ;
    private float lastClickTime = 0f;
    private float doubleClickTime = 0.3f;

    private void OnEnable()
    {
        InputManager input = InputManager.Instance;
        input.OnCameraMove += MoveCamera;
        input.OnCameraZoom += ZoomCamera;
        input.OnCameraRotate += RotateCamera;
        input.OnCameraResetRotation += ResetRotation;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        InputManager input = InputManager.Instance;
        input.OnCameraMove -= MoveCamera;
        input.OnCameraZoom -= ZoomCamera;
        input.OnCameraRotate -= RotateCamera;
        input.OnCameraResetRotation -= ResetRotation;
    }

    private void UpdateBorders()
    {
        float t = Mathf.InverseLerp(minZoomY, maxZoomY, transform.position.y);
        minX = Mathf.Lerp(closeMinX, farMinX, t);
        maxX = Mathf.Lerp(closeMaxX, farMaxX, t);
        minZ = Mathf.Lerp(closeMinZ, farMinZ, t);
        maxZ = Mathf.Lerp(closeMaxZ, farMaxZ, t);
    }

    private void MoveCamera(Vector2 input)
    {
        UpdateBorders();

        Vector3 camForward = transform.forward;
        Vector3 camRight = transform.right;
        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camRight * input.x + camForward * input.y) * movementSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + move;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        transform.position = newPosition;
    }

    private void ZoomCamera(float scrollAmount)
    {
        Vector3 newPosition = transform.position;
        newPosition.y -= scrollAmount * zoomSpeed;
        newPosition.y = Mathf.Clamp(newPosition.y, minZoomY, maxZoomY);
        transform.position = newPosition;
    }

    private void RotateCamera(float amount)
    {
        transform.Rotate(Vector3.up, amount * rotationSpeed, Space.World);
    }

    private void ResetRotation()
    {
        float timeBetweenClicks = Time.time - lastClickTime;

        if (timeBetweenClicks <= doubleClickTime)
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y = 0f;
            transform.eulerAngles = currentRotation;
        }

        lastClickTime = Time.time;
    }
}
