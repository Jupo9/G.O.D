using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public GameObject objectToDestroy;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(objectToDestroy);
        }
    }
}
