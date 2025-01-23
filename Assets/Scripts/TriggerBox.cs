using UnityEngine;

public class TriggerBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Devil"))
        {
            Debug.Log("Hello");
        }
    }
}
