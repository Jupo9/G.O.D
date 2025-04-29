using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Mouse movement with WASD/Arrow-Keys
    /// Mouse Zoom (min/max)
    /// Mouse movement area with dynamic borders
    /// Mouse Rotation around Y
    /// Mouse Rotation reset
    /// </summary>
    
    [Header("Movement")]
    [SerializeField] public float movementSpeed = 10f;
    [SerializeField] public float rotationSpeed = 10f;

    [Header("Zoom")]
    [SerializeField] public float zoomSpeed = 5f;
    [SerializeField] public float maxZoomY = 20f;
    [SerializeField] public float minZoomY = 5f;

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

    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;

    private float clickTime = 0f;
    private float doubleClickTime = 0.3f;

    private void Update()
    {
        MouseMovement();
        MouseRotation();
        MouseRotationReset();

    }

    /// <summary>
    /// Adds mouse movement and zoom funciton
    /// </summary>
    private void MouseMovement()
    {
        UpdateBorders();

        //Implement movement on X- and Z-Achses with WASD or Arrows
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        //Add movement speed and transfrom position for Camera Object and stable movement for Camera rotation
        Vector3 camForward = transform.forward;
        Vector3 camRight = transform.right;
        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();
        Vector3 move = (camRight * moveX + camForward * moveZ) * movementSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + move;

        //This section is for the definition where are the borders for the Camera on X and Z
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        //This section is for mouse zoom and limits for the zoom 
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        newPosition.y -= scroll * zoomSpeed;
        newPosition.y = Mathf.Clamp(newPosition.y, minZoomY, maxZoomY);

        transform.position = newPosition;
    }

    /// <summary>
    /// Change borderarea for Camera dynamic to zoom for less empty space
    /// </summary>
    private void UpdateBorders()
    {
        float t = Mathf.InverseLerp(minZoomY, maxZoomY, transform.position.y);

        minX = Mathf.Lerp(closeMinX, farMinX, t);
        maxX = Mathf.Lerp(closeMaxX, farMaxX, t);
        minZ = Mathf.Lerp(closeMinZ, farMinZ, t);
        maxZ = Mathf.Lerp(closeMaxZ, farMaxZ, t);
    }

    /// <summary>
    /// Add rotation possibility to mouse 
    /// </summary>
    private void MouseRotation()
    {
        if (Input.GetMouseButton(2))
        {
            float rotateMouse = Input.GetAxis("Mouse X");

            transform.Rotate(Vector3.up, rotateMouse * rotationSpeed, Space.World);
        }
    }

    /// <summary>
    /// Add rotation reset with double click on mouse wheel
    /// </summary>
    private void MouseRotationReset()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("Click");

            float timeBetweenClicks = Time.time - clickTime;

            if (timeBetweenClicks <= doubleClickTime)
            {
                Vector3 currentRotation = transform.eulerAngles;
                currentRotation.y = 0f;
                transform.eulerAngles = currentRotation;
            }

            clickTime = Time.time;
        }
    }
}
