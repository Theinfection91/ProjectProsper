using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static bool IsGamePaused { get; private set; }

    [Header("Game Data")]
    public GameDatabase database;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Easy access throughout your game
    public static GameDatabase Database => Instance.database;

    public void PauseGame()
    {
        if (IsGamePaused) return;
        IsGamePaused = true;
    }

    public void ResumeGame()
    {
        if (!IsGamePaused) return;
        IsGamePaused = false;
    }
}