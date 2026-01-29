using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance { get; private set; }

    public string playerName;
    public int level = 1;
    public int experiencePoints;

    public int maxShopsCount = 1;
    public int currentShopsCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
