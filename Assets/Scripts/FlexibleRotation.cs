using UnityEngine;

public class FlexibleRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public bool rotateX = false;
    public bool rotateY = true;
    public bool rotateZ = false;

    public float rotationSpeed = 50f;

    private Vector3 rotationAxis;

    void Start()
    {
        rotationAxis = new Vector3(
                      rotateX ? 1f : 0f,
                      rotateY ? 1f : 0f,
                      rotateZ ? 1f : 0f);
    }

    void Update()
    { 
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
