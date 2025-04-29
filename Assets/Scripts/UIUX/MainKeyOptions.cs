using UnityEngine;

public class MainKeyOptions : MonoBehaviour
{
    /// <summary>
    /// THIS SCRIPT CONTAINS:
    /// Pause Game with Key: Space 
    /// </summary>
    
    private void Update()
    {
        PauseGame();
    }

    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
        }
    }
}
