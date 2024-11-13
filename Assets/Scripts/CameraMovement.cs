using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float movementSpeed = 10f;

    [Header("Zoom")]
    [SerializeField] public float zoomSpeed = 5f;
    [SerializeField] public float maxY = 20f;
    [SerializeField] public float minY = 5f;

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveY);

        transform.Translate(move * movementSpeed * Time.deltaTime);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newY = transform.position.y - scroll * zoomSpeed;

        newY = Mathf.Clamp(newY, minY, maxY);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }


}
