using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float floatSpeed = 1f;         
    public float floatHeight = 0.5f;      
    public float rotationSpeed = 45f;     

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
