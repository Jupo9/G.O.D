using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
    }
}
