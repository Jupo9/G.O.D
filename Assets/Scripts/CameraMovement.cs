using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    /// <summary>
    /// This Script is added to Camera to move the Camera with WASD and Mouse
    /// zomms with mousewheel and limits for zoom max and min, also for movement 
    /// </summary>
    [Header("Movement")]
    [SerializeField] public float movementSpeed = 10f;

    [Header("Zoom")]
    [SerializeField] public float zoomSpeed = 5f;
    [SerializeField] public float maxZoomY = 20f;
    [SerializeField] public float minZoomY = 5f;

    [Header("Movement Borders")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minZ = -10f;
    [SerializeField] private float maxZ = 10f;

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ) * movementSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + move;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        newPosition.y -= scroll * zoomSpeed;
        newPosition.y = Mathf.Clamp(newPosition.y, minZoomY, maxZoomY);

        transform.position = newPosition;
    }


}
